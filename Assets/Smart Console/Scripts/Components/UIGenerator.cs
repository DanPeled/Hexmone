/*
  Package Name: Smart Console
  Version: 2.1.3
  Author: EdgarDev
  Unity Asset Profile: https://assetstore.unity.com/publishers/64126
  Date: 2023-04-12
  Script Name: UIGenerator.cs

  Description:
  This script generate user interface for log messages and autocompletion log messages.
*/

using System;
using UnityEngine;

namespace ED.SC.Components
{
    public class UIGenerator : MonoBehaviour
    {
		[SerializeField] private SmartConsolePreferences m_Preferences;
		[SerializeField] private Transform m_Parent;
        [SerializeField] private GameObject m_LogMessagePrefab;

		private bool m_IsLastMessageEven;

		private void OnEnable()
        {
			SmartConsole.LogMessageReceived += GenerateLogMessage;
			SmartConsole.AutocompletionLogMessageReceived += GenerateAutocompletionLogMessage;
			SmartConsole.LogMessageReceived += EnsureAutocompleteSiblings;
		}
        
        private void OnDisable()
        {
			SmartConsole.LogMessageReceived -= GenerateLogMessage;
			SmartConsole.AutocompletionLogMessageReceived -= GenerateAutocompletionLogMessage;
			SmartConsole.LogMessageReceived -= EnsureAutocompleteSiblings;
		}



		/// <summary>
		/// Instantiate log message instance
		/// </summary>
		/// <param name="logMessage">the log message to generate</param>
		/// <returns>the log message instance setup component</returns>
		private LogMessageSetup InstantiateLogMessageInstance(LogMessage logMessage)
		{
			GameObject logInstance = Instantiate(m_LogMessagePrefab, m_Parent);

			if (!logInstance.TryGetComponent(out LogMessageSetup logMessageInstanceSetup))
			{
				throw new NullReferenceException("LogMessageSetup of logInstance");
			}

			// set instance name
			string logMessageName = logMessage.GetType().Name;
			logInstance.name = $"{logMessageName}({logMessage.Type})";

			return logMessageInstanceSetup;
		}

		/// <summary>
		/// Generate log message
		/// </summary>
		/// <param name="logMessage">the log message to generate</param>
		private void GenerateLogMessage(LogMessage logMessage)
        {
			LogMessageSetup logInstanceSetup = InstantiateLogMessageInstance(logMessage);
			string logMessageText = logMessage.Text;

			if (logMessage.Type == LogMessageType.Command)
			{
				logMessageText = logMessageText.Insert(0, "> ");
			}

			logInstanceSetup.SetText(logMessageText, GetLogMessageTextColor(logMessage.Type), m_Preferences.GlobalFont, m_Preferences.LogTextFontSize);

			if (m_IsLastMessageEven)
			{
				logInstanceSetup.SetBackgroundColor(m_Preferences.BackgroundEvenColor);
			}

			m_IsLastMessageEven = !m_IsLastMessageEven;

			logMessage.LogMessageSetup = logInstanceSetup;

			SmartConsole.LogMessages.Add(logMessage);
		}

		/// <summary>
		/// Generate Autocompletion log message
		/// </summary>
		/// <param name="logMessage">the log message to generate</param>
		private void GenerateAutocompletionLogMessage(LogMessage logMessage)
        {
			LogMessageSetup logInstanceSetup = InstantiateLogMessageInstance(logMessage);

			logInstanceSetup.SetText($"<i>{logMessage.Text}</i>", GetLogMessageTextColor(logMessage.Type), m_Preferences.GlobalFont, m_Preferences.AutocompleteTextFontSize);

			if (logMessage.Parameters != null)
			{
				logInstanceSetup.SetTextParameter(logMessage.Parameters);
			}

			logInstanceSetup.SetBackgroundColor(m_Preferences.BackgroundAutocompleteColor);
			logInstanceSetup.DisableSelection();
			logMessage.LogMessageSetup = logInstanceSetup;

			SmartConsole.AutocompletionLogMessages.Add(logMessage);
		}

		/// <summary>
		/// Get log message color according to its type
		/// </summary>
		/// <param name="logMessageType">the type of log message</param>
		/// <returns>the color</returns>
		private Color GetLogMessageTextColor(LogMessageType logMessageType)
		{
			return logMessageType switch
			{
				LogMessageType.Log => m_Preferences.LogTextColor,
				LogMessageType.Command => m_Preferences.CommandTextColor,
				LogMessageType.Warning => m_Preferences.WarningTextColor,
				LogMessageType.Error => m_Preferences.ErrorTextColor,
				LogMessageType.Autocompletion => m_Preferences.AutocompleteTextColor,
				_ => throw new ArgumentOutOfRangeException(nameof(logMessageType), logMessageType, null)
			};
		}

		private void EnsureAutocompleteSiblings(LogMessage logMessage)
		{
			if (SmartConsole.AutocompletionLogMessages.Count == 0)
			{
				return;
			}

			// sort autocomplete sibling to always go at the bottom of the console

			for (int i = 0; i < SmartConsole.AutocompletionLogMessages.Count; i++)
			{
				int siblingIndex = 
					SmartConsole.LogMessages.Count + SmartConsole.AutocompletionLogMessages.Count - 1 + i;
				SmartConsole.AutocompletionLogMessages[i].LogMessageSetup.transform.SetSiblingIndex(siblingIndex);
			}
		}
	}
}
