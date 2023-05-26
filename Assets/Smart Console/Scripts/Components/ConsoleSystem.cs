/*
  Package Name: Smart Console
  Version: 2.1.3
  Author: EdgarDev
  Unity Asset Profile: https://assetstore.unity.com/publishers/64126
  Date: 2023-02-20
  Script Name: ConsoleSystem.cs

  Description:
  This script is the system that ensure commands execution.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ED.SC.Components
{
	[RequireComponent(typeof(InputHandler))]
	public class ConsoleSystem : MonoBehaviour
	{
		[SerializeField] private SmartConsoleCache m_Cache;
		[SerializeField] private Canvas m_Console;
		[SerializeField] private bool m_ShowApplicationLogs;
		[SerializeField] private bool m_OpenAtStart;
		[SerializeField] private bool m_OpenUnlockCursor;
		[SerializeField] private bool m_CloseLockCursor;

        #region Events

		// Changement events
		public event Action OnActivate;
		public event Action OnDeactivate;

		#endregion

		private InputHandler m_InputHandler;

		private int m_LogMessageCopiedIndex;

		private void Awake()
		{
			m_InputHandler = gameObject.GetComponent<InputHandler>();

			m_LogMessageCopiedIndex = -1;

#if !UNITY_EDITOR
			SmartConsole.RefreshAutocomplete();
#endif
		}

		private void OnEnable()
        {
            m_InputHandler.OnOpenCloseInput += ToggleConsole;
			m_InputHandler.OnCopyNextLogInput += CopyNextSentLogMessage;
            m_InputHandler.OnCopyPreviousLogInput += CopyPreviousSentLogMessage;

			SmartConsole.OnSubmit += EnsureCommandExecution;

			if (m_ShowApplicationLogs)
            {
                Application.logMessageReceived += SubmitLog;
			}

			if (m_OpenUnlockCursor)
			{
				OnActivate += UnlockCursor;
			}

			if (m_CloseLockCursor)
			{
				OnDeactivate += LockCursor;
			}
		}
        
        private void OnDisable()
        {
            m_InputHandler.OnOpenCloseInput -= ToggleConsole;
			m_InputHandler.OnCopyNextLogInput -= CopyNextSentLogMessage;
            m_InputHandler.OnCopyPreviousLogInput -= CopyPreviousSentLogMessage;

			SmartConsole.OnSubmit -= EnsureCommandExecution;

			if (m_ShowApplicationLogs)
            {
                Application.logMessageReceived -= SubmitLog;
            }

			if (m_OpenUnlockCursor)
			{
				OnActivate -= UnlockCursor;
			}

			if (m_CloseLockCursor)
			{
				OnDeactivate -= LockCursor;
			}
		}

		private void Start()
		{
			if (m_OpenAtStart)
			{
				m_InputHandler.ToggleInputDispatcher();
			}
		}

		/// <summary>
		/// Find command that fits a specific text
		/// </summary>
		/// <param name="text">specific text</param>
		private Command FindCommandByText(string text)
		{
			Command command = null;

			for (int i = 0; i < m_Cache.CommandRegistry.Length && command == null; i++)
			{
				if (string.Equals(m_Cache.CommandRegistry[i].Name.ToLower(), text.ToLower()))
				{
					// found command occurence
					command = m_Cache.CommandRegistry[i];
				}
			}

			return command;
		}

        /// <summary>
        /// Submit the command that fits the input
        /// </summary>
        /// <param name="inputText">text field</param>
        private void EnsureCommandExecution(string inputText)
        {
            if (string.IsNullOrEmpty(inputText))
            {
                return;
            }
            
			// find the command by input text
            string[] inputParts = inputText.Split(' ');
            Command command = FindCommandByText(inputParts[0]);

			// create and log the log message
			LogMessage logMessage = new LogMessage(inputText, LogMessageType.Command);
			SmartConsole.LogMessagesHistory.Insert(0, logMessage);
			SmartConsole.LogCommand(logMessage);

			// try to execute the command if exists
            if (command != null)
            {
				if (command.Method.GetParameters().Length > 0)
                {
					Exception exception = null;
					object[] targets = null;

					// get the parameters
					object[] parameters = null;
					string[] paramParts = new string[inputParts.Length - 1];
					Array.Copy(inputParts, 1, paramParts, 0, paramParts.Length);

					try
					{
						targets = FindCommandTargets(command);

						// cast the param into their params
						command.GetParameters(paramParts, out parameters);
					}
					catch (Exception e)
					{
						exception = e;
					}

					if (exception == null)
					{
						// execute command with parameter
						ExecuteCommandOnTargets(command, targets, parameters);
					}
					else
					{
						SmartConsole.LogError(exception.Message);
					}
				}
                else
                {
					Exception exception = null;
					object[] targets = null;

					try
					{
						targets = FindCommandTargets(command);

						if (inputParts.Length > 1)
						{
							// too many parameters
							exception = new SmartParameterTooManyException(command, inputParts.Length - 1);
						}
					}
					catch (Exception e)
					{
						exception = e;
					}

					if (exception == null)
					{
						// execute command without parameter
						ExecuteCommandOnTargets(command, targets);
					}
					else
					{
						SmartConsole.LogError(exception.Message);
					}
				}
			}
			else
			{
				SmartConsole.LogError($"Smart Error: Command '{inputText}' could not be found.");
			}

			SmartConsole.Reset();
			m_LogMessageCopiedIndex = -1;
		}

		/// <summary>
		/// Find the command targets according to its MonoTargetType
		/// </summary>
		/// <param name="command">the command</param>
		/// <returns>object array of target</returns>
		private object[] FindCommandTargets(Command command)
		{
			if (command.Method.IsStatic)
			{
				return null;
			}

			// find target(s)
			List<object> targets = new List<object>();

			if (command.TargetType == MonoTargetType.Single)
			{
				targets.Add(FindObjectOfType(command.Method.DeclaringType));
			}
			else if (command.TargetType == MonoTargetType.Active)
			{
				targets.AddRange(FindObjectsOfType(command.Method.DeclaringType, false));
			}
			else if (command.TargetType == MonoTargetType.All)
			{
				targets.AddRange(FindObjectsOfType(command.Method.DeclaringType, true));
			}

			if (targets.Count == 0)
			{
				throw new SmartCommandNoTargetException(command);
			}

			if (targets.Count == 1 && targets[0] == null)
			{
				throw new SmartCommandNoTargetException(command);
			}

			return targets.ToArray();
		}

		/// <summary>
		/// Execute command on targets
		/// </summary>
		/// <param name="command">the command to execute</param>
		/// <param name="targets">the targets on which to execute</param>
		/// <param name="parameters">the command params if there is</param>
		private void ExecuteCommandOnTargets(Command command, object[] targets, object[] parameters = null)
		{
			if (command.Method.IsStatic)
			{
				// command is static and must not have target
				command.Use(null, parameters);
				return;
			}

			// use command on target(s)
			for (int i = 0; i < targets.Length; i++)
			{
				var result = command.Use(targets[i], parameters);

				if (typeof(IEnumerator).IsAssignableFrom(command.Method.ReturnType))
				{
					// If the method returns a coroutine, start it using StartCoroutine
					StartCoroutine((IEnumerator)result);
				}
			}
		}

		/// <summary>
		/// Submit the command as application log message
		/// </summary>
		private void SubmitLog(string logString, string stackTrace, LogType type)
        {
            LogMessageType messagetype = type switch
            {
                LogType.Log => LogMessageType.Log,
                LogType.Error => LogMessageType.Error,
                LogType.Assert => LogMessageType.Error,
                LogType.Warning => LogMessageType.Warning,
                LogType.Exception => LogMessageType.Error,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
            
            LogMessage logMessage = new LogMessage(logString, messagetype);
			SmartConsole.LogCommand(logMessage);
        }

		/// <summary>
		/// Copy the next sent log message into the input.
		/// Move the caret after a frame to go over MoveUp input field's function call
		/// </summary>
		private void CopyNextSentLogMessage()
        {
            CopyLogMessageIndex(1);
			StartCoroutine(SmartConsole.MoveCaretCoroutine());
		}

		/// <summary>
		/// Copy the previous sent log message into the input
		/// </summary>
		private void CopyPreviousSentLogMessage() => CopyLogMessageIndex(-1);

		/// <summary>
		/// Copy log message history index
		/// </summary>
		/// <param name="addToIndex">the index to add to the current copy index</param>
        private void CopyLogMessageIndex(int addToIndex)
        {
            if (SmartConsole.LogMessagesHistory.Count == 0)
            {
                return;
            }

			m_LogMessageCopiedIndex += addToIndex;

			if (m_LogMessageCopiedIndex < 0)
			{
				m_LogMessageCopiedIndex = -1;
				SmartConsole.SetInputText("", true);
				return;
			}

			if (m_LogMessageCopiedIndex > SmartConsole.LogMessagesHistory.Count - 1)
			{
				m_LogMessageCopiedIndex = SmartConsole.LogMessagesHistory.Count - 1;
				return;
			}

			SmartConsole.Reset();
			SmartConsole.SetInputText(SmartConsole.LogMessagesHistory[m_LogMessageCopiedIndex].Text, true);
        }

		/// <summary>
		/// Open the console if disabled or close it if enabled
		/// </summary>
		private void ToggleConsole()
		{
			GameObject ui = m_Console.gameObject;
			ui.SetActive(!ui.activeInHierarchy);

			if (ui.activeInHierarchy)
			{
				// console canvas is enabled
				SmartConsole.ActivateInputField();

				string inputFieldText = SmartConsole.GetInputText();

				if (!string.IsNullOrEmpty(inputFieldText))
				{
					SmartConsole.SetInputText(inputFieldText.Remove(inputFieldText.Length - 1));
				}

				OnActivate?.Invoke();
			}
			else
			{
				// console canvas is disabled
				SmartConsole.Reset();
				m_LogMessageCopiedIndex = -1;

				OnDeactivate?.Invoke();
			}
		}

		/// <summary>
		/// Lock the cursor and hide it
		/// </summary>
		private void LockCursor()
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

		/// <summary>
		/// Unlock the cursor and show it
		/// </summary>
		private void UnlockCursor()
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}

		/// <summary>
		/// Clear for the button's UnityEvent assignation
		/// </summary>
		public void ClearDispatcher()
		{
			SmartConsole.Clear();
		}

		[Command("get_commands", "Gets available commands")]
		private void GetCommands()
		{
			string commands = "";

			for (int i = 0; i < m_Cache.AvailableCommands.Count; i++)
			{
				Command command = m_Cache.AvailableCommands[i];
				commands += $"- {command.Name}";

				if (!string.IsNullOrEmpty(command.Description))
				{
					commands += $": {command.Description}";
				}

				commands += "\r\n";
			}

			SmartConsole.Log($"List of available commands ({m_Cache.AvailableCommands.Count}):\r\n{commands}");
		}

		[Command("get_all_commands", "Gets all commands")]
		private void GetAllCommands()
		{
			string commands = "";

			for (int i = 0; i < m_Cache.CommandRegistry.Length; i++)
			{
				Command command = m_Cache.CommandRegistry[i];
				commands += $"- {command.Name}";

				if (!string.IsNullOrEmpty(command.Description))
				{
					commands += $": {command.Description}";
				}

				commands += "\r\n";
			}

			SmartConsole.Log($"List of all commands ({m_Cache.CommandRegistry.Length}):\r\n{commands}");
		}
	}
}
