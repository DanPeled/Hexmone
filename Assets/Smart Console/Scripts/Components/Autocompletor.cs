/* 
  Package Name: Smart Console
  Version: 2.1.3
  Author: EdgarDev
  Unity Asset Profile: https://assetstore.unity.com/publishers/64126
  Date: 2023-03-05 
  Script Name: Autocompletor.cs

  Description:
  This script implements an autocompletion system for the console.
  It uses a dictionary of command names and their descriptions to suggest
  completions as the user types, making it easier and faster to enter commands.
*/

using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using System.Reflection;
using TMPro;

namespace ED.SC.Components
{
	public class Autocompletor : MonoBehaviour
	{
		[SerializeField] private SmartConsoleCache m_ConsoleCache;
		[SerializeField] private TextMeshProUGUI m_ParametersText;

		public event Action<LogMessage> OnCopyAutocompletion;
		public event Action OnBreakAutocompletion;

		// TMPro italic tags
		private const string k_SelectedParameterStartTag = "<u>";
		private const string k_SelectedParameterEndTag = "</u>";

		private InputHandler m_InputHandler;

		private string m_CommandInput;
		private int m_AutocompletionIndex;
		private int m_SelectedParameterIndex;

		private void Awake()
		{
			m_InputHandler = gameObject.GetComponentInParent<InputHandler>();

			m_CommandInput = "";
			m_AutocompletionIndex = -1;
			m_SelectedParameterIndex = -1;
		}

		private void OnEnable()
		{
			SmartConsole.OnReset += ResetStates;

			m_InputHandler.OnInputFieldValueChange += EnsureAutocompletionGeneration;
			m_InputHandler.OnAutocompleteInput += EnsureAutocompletion;
			m_InputHandler.OnSelectParameterInput += SelectNextParameterIndex;
		}

		private void OnDisable()
		{
			SmartConsole.OnReset -= ResetStates;

			m_InputHandler.OnInputFieldValueChange -= EnsureAutocompletionGeneration;
			m_InputHandler.OnAutocompleteInput -= EnsureAutocompletion;
			m_InputHandler.OnSelectParameterInput -= SelectNextParameterIndex;
		}

		/// <summary>
		/// Find commands that start with text
		/// </summary>
		/// <param name="startWith">start with text</param>
		/// <param name="checkDuplication">check for autocompletion duplication</param>
		/// <returns>commands log message</returns>
		private List<LogMessage> FindNewAutocompletions(string startWith, bool checkDuplication)
		{
			List<Command> commands = m_ConsoleCache.AvailableCommands.FindAll(cmd => cmd.Name.ToLower().StartsWith(startWith.ToLower()));

			if (checkDuplication)
			{
				commands = commands.FindAll(cmd =>
				{
					bool isNewCommand = true;

					for (int i = 0; i < SmartConsole.AutocompletionLogMessages.Count && isNewCommand; i++)
					{
						if (string.Equals(cmd.Name, SmartConsole.AutocompletionLogMessages[i].Text))
						{
							isNewCommand = false;
						}
					}

					return isNewCommand;
				});
			}

			var newAutocompletions = new List<LogMessage>();

			if (commands.Count > 0)
			{
				for (int i = 0; i < commands.Count; i++)
				{
					newAutocompletions.Add(new LogMessage(commands[i].Name, LogMessageType.Autocompletion, commands[i].Method.GetParameters()));
				}
			}

			return newAutocompletions;
		}

		/// <summary>
		/// Submit the commands that fits input text as autocompletions log messages
		/// </summary>
		/// <param name="text">input text</param>
		private void EnsureAutocompletionGeneration(string input)
		{
			string commandInput = input.Split(' ')[0];

			if (string.IsNullOrEmpty(commandInput))
			{
				// input field is empty
				SmartConsole.ClearAutocompletions();
				m_AutocompletionIndex = -1;
			}
			else if (SmartConsole.AutocompletionLogMessages.Count == 0 && string.IsNullOrEmpty(m_CommandInput))
			{
				// modified last character input or is first input

				SmartConsole.ClearAutocompletions();
				m_AutocompletionIndex = -1;

				// find new autocompletions that match command input
				// no need to check for duplication
				List<LogMessage> autocompletions = FindNewAutocompletions(commandInput, false);

				if (autocompletions.Count > 0)
				{
					// generate autocompletions
					for (int i = 0; i < autocompletions.Count; i++)
					{
						SmartConsole.LogAutocompletion(autocompletions[i]);
					}
				}
			}
			else if (m_CommandInput.Length < commandInput.Length)
			{
				// new character input

				// find instantiated autocompletions that doesn't match command input anymore
				List<LogMessage> obsoleteAutocompletions = SmartConsole.AutocompletionLogMessages.FindAll(lm => !lm.Text.ToLower().StartsWith(commandInput.ToLower()));

				if (obsoleteAutocompletions.Count > 0)
				{
					// clean obsolete autocompletion
					for (int i = 0; i < obsoleteAutocompletions.Count; i++)
					{
						Destroy(obsoleteAutocompletions[i].LogMessageSetup.gameObject);
						SmartConsole.AutocompletionLogMessages.Remove(obsoleteAutocompletions[i]);
					}
				}
			}
			else if (m_CommandInput.Length > commandInput.Length)
			{
				// removed last character input

				// find new autocompletions that match command input
				List<LogMessage> newAutocompletions = FindNewAutocompletions(commandInput, true);

				if (newAutocompletions.Count > 0)
				{
					// generate new autocompletions
					for (int i = 0; i < newAutocompletions.Count; i++)
					{
						SmartConsole.LogAutocompletion(newAutocompletions[i]);
					}
				}
			}

			m_CommandInput = commandInput;

			m_ParametersText.text = "";
			m_SelectedParameterIndex = -1;

			OnBreakAutocompletion?.Invoke();
		}

		/// <summary>
		/// Autocomplete next log message index into the input
		/// </summary>
		private void EnsureAutocompletion()
		{
			if (SmartConsole.AutocompletionLogMessages.Count == 0)
			{
				m_AutocompletionIndex = -1;

				return;
			}

			if (m_SelectedParameterIndex > -1)
			{
				AutocompleteParametersIntoInput();

				return;
			}

			// reset selected parameter indexes
			m_SelectedParameterIndex = -1;

			m_AutocompletionIndex++;

			if (m_AutocompletionIndex >= SmartConsole.AutocompletionLogMessages.Count)
			{
				m_AutocompletionIndex = 0;
			}

			LogMessage toCopy = SmartConsole.AutocompletionLogMessages[m_AutocompletionIndex];
			SmartConsole.SetInputText(toCopy.Text);

			CopyAutocompletionParametersIntoInputPlaceholder();

			OnCopyAutocompletion?.Invoke(toCopy);
		}

		/// <summary>
		/// Copy current log message parameter into the input placeholder
		/// </summary>
		private void CopyAutocompletionParametersIntoInputPlaceholder()
		{
			ParameterInfo[] parameters = SmartConsole.AutocompletionLogMessages[m_AutocompletionIndex].Parameters;
			string paramText = "";

			if (parameters.Length > 0)
			{
				paramText = $"<color=#FFFFFF00>{SmartConsole.GetInputText()}</color>";

				for (int i = 0; i < parameters.Length; i++)
				{
					paramText += $" {parameters[i].Name}";

					if (!string.IsNullOrEmpty(parameters[i].DefaultValue.ToString()))
					{
						paramText += $"\u00A0=\u00A0{parameters[i].DefaultValue}";
					}
				}
			}

			m_ParametersText.text = paramText;
		}

		/// <summary>
		/// Autocomplete current log message parameter into the input and copy it into the parameters text
		/// </summary>
		private void AutocompleteParametersIntoInput()
		{
			ParameterInfo[] parameterInfos = SmartConsole.AutocompletionLogMessages[m_AutocompletionIndex].Parameters;

			if (m_SelectedParameterIndex >= parameterInfos.Length)
			{
				throw new ArgumentOutOfRangeException();
			}

			// get new input
			string newInputText = SmartConsole.GetInputText();
			int inputParamCount = newInputText.Split(' ').Length - 1;
			bool isEmpty = m_SelectedParameterIndex >= inputParamCount;

			if (isEmpty)
			{
				while (m_SelectedParameterIndex >= inputParamCount)
				{
					Type type = parameterInfos[inputParamCount].ParameterType;

					string newValue = TypeUtil.GetDefaultStringValue(type);

					newInputText += $" {newValue}";
					inputParamCount++;
				}
			}
			else
			{
				string parameter = newInputText.Split(' ')[m_SelectedParameterIndex + 1];

				Type type = parameterInfos[m_SelectedParameterIndex].ParameterType;

				object currentValue = TypeUtil.GetObjectValue(type, parameter);
				object updatedValue = TypeUtil.GetNextObjectValue(type, currentValue);
				string updatedValueString = TypeUtil.GetStringValue(type, updatedValue);

				newInputText = StringUtil.ReplaceWordAtIndex(newInputText, m_SelectedParameterIndex + 1, updatedValueString);
			}

			// set new input
			SmartConsole.SetInputText(newInputText);

			// get new parameters text
			string newParametersText = m_ParametersText.text;
			string[] inputWords = SmartConsole.GetInputText().Split(' ');

			for (int i = 1; i < inputWords.Length; i++)
			{
				string param = inputWords[i];

				if (i == m_SelectedParameterIndex + 1)
				{
					param = $"{k_SelectedParameterStartTag}{param}{k_SelectedParameterEndTag}";
				}

				newParametersText = StringUtil.ReplaceWordAtIndex(newParametersText, i, param);
			}

			// set new parameters text
			m_ParametersText.text = newParametersText;
		}

		/// <summary>
		/// Select next parameter index
		/// </summary>
		private void SelectNextParameterIndex()
		{
			string inputPlaceholder = m_ParametersText.text;

			// delete previous parameter underline tag
			int indexOpeningTag = inputPlaceholder.IndexOf(k_SelectedParameterStartTag);

			if (indexOpeningTag != -1)
			{
				inputPlaceholder = inputPlaceholder.Remove(indexOpeningTag, 3);
			}

			int indexClosingTag = inputPlaceholder.IndexOf(k_SelectedParameterEndTag);

			if (indexClosingTag != -1)
			{
				inputPlaceholder = inputPlaceholder.Remove(indexClosingTag, 4);
			}

			string[] parameters = inputPlaceholder.Split(' ').Skip(1).ToArray();

			if (parameters.Length == 0)
			{
				return;
			}

			if (m_SelectedParameterIndex >= parameters.Length - 1)
			{
				// outside bounds should reset index
				m_SelectedParameterIndex = -1;
				m_ParametersText.text = inputPlaceholder;

				return;
			}

			m_SelectedParameterIndex++;

			int paramStartIndex = inputPlaceholder.IndexOf(' ') + 1;

			for (int i = 0; i < m_SelectedParameterIndex; i++)
			{
				paramStartIndex = inputPlaceholder.IndexOf(' ', paramStartIndex) + 1;
			}

			int wordIndex = inputPlaceholder.IndexOf(parameters[m_SelectedParameterIndex], paramStartIndex);
			int wordLastIndex = wordIndex + parameters[m_SelectedParameterIndex].Length;

			inputPlaceholder = inputPlaceholder.Insert(wordIndex, k_SelectedParameterStartTag);
			inputPlaceholder = inputPlaceholder.Insert(wordLastIndex + 3, k_SelectedParameterEndTag);

			m_ParametersText.text = inputPlaceholder;
		}

		/// <summary>
		/// Reset all the autocompletor states
		/// </summary>
		private void ResetStates()
		{
			m_CommandInput = "";
			m_ParametersText.text = "";
			m_AutocompletionIndex = -1;
			m_SelectedParameterIndex = -1;
		}
	}
}
