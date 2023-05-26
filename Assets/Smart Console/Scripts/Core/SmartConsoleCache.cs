/*
  Package Name: Smart Console
  Version: 2.1.3
  Author: EdgarDev
  Unity Asset Profile: https://assetstore.unity.com/publishers/64126
  Date: 2023-04-09
  Script Name: SmartConsoleCache.cs

  Description:
  This class is a scriptable object used store data cache.
*/

using System;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

using System.Reflection;
using System.Linq;
#endif

namespace ED.SC
{
	public class SmartConsoleCache : ScriptableObject
	{
		public Command[] CommandRegistry;
		public List<Command> AvailableCommands;

		private void OnEnable()
		{
			SmartConsole.RefreshAutocompleteReceived += RefreshAutocomplete;
			SmartConsole.AddAutocompleteReceived += AddAutocomplete;
			SmartConsole.RemoveAutocompleteReceived += RemoveAutocomplete;

#if UNITY_EDITOR
			EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
		}

		private void OnDisable()
		{
			SmartConsole.RefreshAutocompleteReceived -= RefreshAutocomplete;
			SmartConsole.AddAutocompleteReceived -= AddAutocomplete;
			SmartConsole.RemoveAutocompleteReceived -= RemoveAutocomplete;

#if UNITY_EDITOR
			EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
#endif
		}

		public void AddAutocomplete(Type type)
		{
			if (type == null)
			{
				Debug.LogWarning($"Smart Error: Cannot add null type to autocomplete.");
				return;
			}

			bool alreadyAdded = false;

			for (int i = 0; i < CommandRegistry.Length && !alreadyAdded; i++)
			{
				if (CommandRegistry[i].Method != null && CommandRegistry[i].Method.ReflectedType == type)
				{
					if (!AvailableCommands.Contains(CommandRegistry[i]))
					{
						// add it to available list
						AvailableCommands.Add(CommandRegistry[i]);
					}
					else
					{
						// this type has already been added to autocomplete
						// add should be ignored
						alreadyAdded = true;
					}
				}
			}
		}

		public void RemoveAutocomplete(Type type)
		{
			if (type == null)
			{
				Debug.LogWarning($"Smart Error: Cannot remove null type to autocomplete.");
				return;
			}

			for (int i = 0; i < CommandRegistry.Length; i++)
			{
				if (CommandRegistry[i].Method != null && CommandRegistry[i].Method.ReflectedType == type)
				{
					// this command needs target
					// try find target
					object target = GameObject.FindObjectOfType(type);

					if (target == null)
					{
						// target not found
						// remove it from available list
						AvailableCommands.Remove(CommandRegistry[i]);
					}
				}
			}
		}

		public void RefreshAutocomplete()
		{
			if (AvailableCommands.Count > 0)
			{
				AvailableCommands.Clear();
			}

			for (int i = 0; i < CommandRegistry.Length; i++)
			{
				if (CommandRegistry[i].Method == null)
				{
					Debug.LogWarning($"Smart Error: Command '{CommandRegistry[i].Name}' at index {i} could not be loaded.");
				}
				else
				{
					if (CommandRegistry[i].Method.IsStatic)
					{
						// this command is static and do not needs target
						// add it directly to available list
						AvailableCommands.Add(CommandRegistry[i]);
					}
					else
					{
						// this command is instance and do needs target
						// try find target
						Type targetType = CommandRegistry[i].Method.ReflectedType;
						object target = GameObject.FindObjectOfType(targetType);

						if (target != null)
						{
							// target found
							// add it to available list
							AvailableCommands.Add(CommandRegistry[i]);
						}
					}
				}
			}
		}

#if UNITY_EDITOR
		[SerializeField] private SmartConsolePreferences m_Preferences;

		private List<string> m_CommandRegistryNames = new List<string>();

		private void OnPlayModeStateChanged(PlayModeStateChange state)
		{
			if (state == PlayModeStateChange.EnteredPlayMode)
			{
				if (m_Preferences.CacheReloadOnPlayMode)
				{
					Clear();
					Load();
				}

				RefreshAutocomplete();
			}
		}

		public void Load()
		{
			var watch = System.Diagnostics.Stopwatch.StartNew();

			// reset name
			if (m_CommandRegistryNames.Count > 0)
			{
				m_CommandRegistryNames.Clear();
			}

			// get the name of the current assembly
			string currentAssemblyName = Assembly.GetExecutingAssembly().GetName().FullName;

			// find methods in all assemblies that has the [Command] attribute
			CommandRegistry = AppDomain.CurrentDomain.GetAssemblies()
				.Where(assembly => IsAssemblyConcerned(assembly, currentAssemblyName))
				.SelectMany(assembly => assembly.GetTypes())
				.SelectMany(type => type.GetMethods(
					BindingFlags.Public |
					BindingFlags.NonPublic |
					BindingFlags.Static |
					BindingFlags.Instance))
				.Where(IsValidMethod)
				.Select(CreateCommand)
				.ToArray();

			Array.Sort(CommandRegistry, (Command cmd1, Command cmd2) => string.Compare(cmd1.Name, cmd2.Name, true));

			watch.Stop();
			Debug.Log($"Cache loaded successfully in {watch.Elapsed.Milliseconds} ms.");

			EditorUtility.SetDirty(this);
			AssetDatabase.SaveAssets();
		}

		public void Clear()
		{
			Array.Clear(CommandRegistry, 0, CommandRegistry.Length);
			CommandRegistry = new Command[0];

			AvailableCommands.Clear();

			Debug.Log($"Cache cleared successfully.");

			EditorUtility.SetDirty(this);
			AssetDatabase.SaveAssets();
		}

		private bool IsAssemblyConcerned(Assembly assembly, string currentAssemblyName)
		{
			if (string.Equals(assembly.FullName, currentAssemblyName))
			{
				// is smart console assembly
				return true;
			}

			return assembly.GetReferencedAssemblies().Any(a => string.Equals(a.FullName, currentAssemblyName));
		}

		private bool IsValidMethod(MethodInfo method)
		{
			CommandAttribute commandAttribute = (CommandAttribute)method.GetCustomAttribute(typeof(CommandAttribute), false);

			if (commandAttribute == null)
			{
				return false;
			}

			if (m_CommandRegistryNames.Count == 0)
			{
				return true;
			}

			string commandName = GetCommandName(commandAttribute, method.Name);

			if (m_CommandRegistryNames.Contains(commandName))
			{
				// this command name is not available
				Debug.LogWarning($"Smart Error: Command '{commandName}' could not be added because its name is duplicated.");
				return false;
			}

			return true;
		}

		private Command CreateCommand(MethodInfo methodInfo)
		{
			CommandAttribute commandAttribute = (CommandAttribute)methodInfo.GetCustomAttribute(typeof(CommandAttribute), false);
			string commandName = GetCommandName(commandAttribute, methodInfo.Name);

			m_CommandRegistryNames.Add(commandName);

			return new Command(commandName, commandAttribute.Description, commandAttribute.MonoTargetType, new SerializableMethodInfo(methodInfo));
		}

		private string GetCommandName(CommandAttribute commandAttribute, string defaultName)
		{
			string commandName = defaultName;

			if (commandAttribute.HasName())
			{
				commandName = commandAttribute.Name;

				if (commandName.Contains(' '))
				{
					commandName = commandName.Replace(" ", "");
				}
			}

			return commandName;
		}
#endif
	}
}