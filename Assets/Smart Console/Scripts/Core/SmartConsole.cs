/*
  Package Name: Smart Console
  Version: 2.1.3
  Author: EdgarDev
  Unity Asset Profile: https://assetstore.unity.com/publishers/64126
  Date: 2023-03-03
  Script Name: SmartConsole.cs

  Description:
  This script is the main entry point for actions and callbacks.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ED.SC
{
    public static class SmartConsole
    {
        /// <summary>
        /// Current version of the Smart Console.
        /// </summary>
        public const string Version = "2.1.3";

        /// <summary>
        /// Delegate method that takes in a LogMessage object and returns nothing.
        /// This is used to send a LogMessage object whenever a new log is added to the console.
        /// </summary>
        internal delegate void LogMessageCallback(LogMessage logMessage);

        /// <summary>
        /// Delegate method that takes no parameters and returns nothing.
        /// This is a callback raised after a clear of the console.
        /// </summary>
        internal delegate void OnClearCallback();

        /// <summary>
        /// Delegate method that takes no parameters and returns nothing.
        /// This is a callback raised after a reset of the console.
        /// </summary>
        internal delegate void OnResetCallback();

        /// <summary>
        /// Delegate method that takes in a string and returns nothing.
        /// This is used to handle input text submitted by the user.
        /// </summary>
        internal delegate void OnSubmitCallback(string inputText);

        /// <summary>
        /// Delegate method that takes no parameters and returns nothing.
        /// This is used to refresh the console autocompletes.
        /// </summary>
        internal delegate void RefreshAutocompleteCallback();

        /// <summary>
        /// Delegate method that takes in a type and returns nothing.
        /// This is used to update the console autocomplete.
        /// </summary>
        internal delegate void UpdateAutocompleteCallback(Type type);

        /// <summary>
        /// This event is raised whenever a new log is added to the console.
        /// It is subscribed by the console to display the new log in the console.
        /// </summary>
        internal static event LogMessageCallback LogMessageReceived;

        /// <summary>
        /// This event is raised whenever a new log is added to the console.
        /// It is subscribed by the console to display the new autocompletion log in the console.
        /// </summary>
        internal static event LogMessageCallback AutocompletionLogMessageReceived;

        /// <summary>
        /// This event is raised whenever the console is cleared.
        /// It is subscribed by the console to clear the console display.
        /// </summary>
        internal static event OnClearCallback OnClear;

        /// <summary>
        /// This event is raised whenever the console is cleared.
        /// It is subscribed by the console to clear the console's autocompletions display.
        /// </summary>
        internal static event OnClearCallback OnClearAutocompletions;

        /// <summary>
        /// This event is raised whenever the console is reset to its default state.
        /// It is subscribed by the console to reset the console display.
        /// </summary>
        internal static event OnResetCallback OnReset;

        /// <summary>
        /// This event is raised whenever the user submits input text.
        /// It is subscribed by the console to handle the input text.
        /// </summary>
        internal static event OnSubmitCallback OnSubmit;

        /// <summary>
        /// This event is raised at start on the console.
        /// It is subscribed by the console to refresh autocomplete.
        /// </summary>
        internal static event RefreshAutocompleteCallback RefreshAutocompleteReceived;

        /// <summary>
        /// This event is raised whenever a refresh autocomplete is needed on the console.
        /// It is subscribed by the console to add autocomplete.
        /// </summary>
        internal static event UpdateAutocompleteCallback AddAutocompleteReceived;

        /// <summary>
        /// This event is raised whenever a refresh autocomplete is needed on the console.
        /// It is subscribed by the console to remove autocomplete.
        /// </summary>
        internal static event UpdateAutocompleteCallback RemoveAutocompleteReceived;

        internal static readonly List<LogMessage> LogMessages = new List<LogMessage>();
        internal static readonly List<LogMessage> AutocompletionLogMessages =
            new List<LogMessage>();
        internal static readonly List<LogMessage> LogMessagesHistory = new List<LogMessage>();
        internal static TMP_InputField InputField;

        /// <summary>
        /// Adds a log to the console.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void Log(string message)
        {
            var logMessage = new LogMessage(message, LogMessageType.Log);
            LogMessageReceived?.Invoke(logMessage);
        }

        /// <summary>
        /// Adds a warning log to the console.
        /// </summary>
        /// <param name="message">The warning to log.</param>
        public static void LogWarning(string message)
        {
            var logMessage = new LogMessage(message, LogMessageType.Warning);
            LogMessageReceived?.Invoke(logMessage);
        }

        /// <summary>
        /// Adds a error log to the console.
        /// </summary>
        /// <param name="message">The error to log.</param>
        public static void LogError(string message)
        {
            var logMessage = new LogMessage(message, LogMessageType.Error);
            LogMessageReceived?.Invoke(logMessage);
        }

        /// <summary>
        /// Adds a command message to the console.
        /// </summary>
        /// <param name="commandMessage">The command message to log.</param>
        internal static void LogCommand(LogMessage commandMessage)
        {
            LogMessageReceived?.Invoke(commandMessage);
        }

        /// <summary>
        /// Adds an autocompletion log message to the console.
        /// </summary>
        /// <param name="autocompletionLogMessage">The autocompletion message to log.</param>
        internal static void LogAutocompletion(LogMessage autocompletionLogMessage)
        {
            AutocompletionLogMessageReceived?.Invoke(autocompletionLogMessage);
        }

        /// <summary>
        /// Sends a message to the console using its input.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public static void Send(string message)
        {
            SetInputText(message, false, false);
            SubmitInputField();
        }

        /// <summary>
        /// Gets the current input text.
        /// </summary>
        /// <returns>the input text</returns>
        internal static string GetInputText() => InputField.text;

        /// <summary>
        /// Sets the input text content.
        /// </summary>
        /// <param name="value">the text content</param>
        /// <param name="notify">notifies input to get autocompletions</param>
        /// <param name="moveCaret">moves the caret at the text content's end</param>
        internal static void SetInputText(string value, bool notify = false, bool moveCaret = true)
        {
            if (notify)
            {
                InputField.text = value;
            }
            else
            {
                InputField.SetTextWithoutNotify(value);
            }

            if (moveCaret)
            {
                MoveCaret();
            }
        }

        internal static void ActivateInputField()
        {
            InputField.ActivateInputField();
        }

        internal static void SubmitInputField()
        {
            OnSubmit?.Invoke(GetInputText());
        }

        /// <summary>
        /// Move caret to end of text after a frame
        /// </summary>
        internal static IEnumerator MoveCaretCoroutine()
        {
            yield return null;
            InputField.MoveTextEnd(false);
        }

        /// <summary>
        /// Move caret to end of text
        /// </summary>
        private static void MoveCaret() => InputField.MoveTextEnd(false);

        /// <summary>
        /// Clears the console.
        /// </summary>
        [Command("clear", "Clears the console")]
        public static void Clear()
        {
            if (LogMessages.Count == 0)
            {
                return;
            }

            for (int i = 0; i < LogMessages.Count; i++)
            {
                GameObject.Destroy(LogMessages[i].LogMessageSetup.gameObject);
            }

            LogMessages.Clear();
            OnClear?.Invoke();
        }
        /// <summary>
        /// Teleports the player to pos x and y
        /// </summary>
        /// <param name="x">X pos</param>
        /// <param name="y">Y pos</param>
        [Command("tp", "teleports the player to the specified location")]
        public static void Tp(int x, int y)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = new Vector3(x, y, player.transform.position.z);
        }

        /// <summary>
        /// Clears the console's autocompletions.
        /// </summary>
        internal static void ClearAutocompletions()
        {
            if (AutocompletionLogMessages.Count == 0)
            {
                return;
            }

            for (int i = 0; i < AutocompletionLogMessages.Count; i++)
            {
                GameObject.Destroy(AutocompletionLogMessages[i].LogMessageSetup.gameObject);
            }

            AutocompletionLogMessages.Clear();
            OnClearAutocompletions?.Invoke();
        }

        /// <summary>
        /// Resets the console to its default state.
        /// </summary>
        internal static void Reset()
        {
            SetInputText("", false, false);
            ClearAutocompletions();

            ActivateInputField();

            OnReset?.Invoke();
        }

        /// <summary>
        /// This function is used to refresh the autocomplete suggestions.
        /// It should be used on start at run time.
        /// </summary>
        public static void RefreshAutocomplete()
        {
            RefreshAutocompleteReceived?.Invoke();
        }

        /// <summary>
        /// This function is used to refresh the autocomplete suggestions.
        /// It should be used when a new command has been added or removed at run time.
        /// </summary>
        public static void AddAutocomplete(MonoBehaviour behaviour)
        {
            AddAutocompleteReceived?.Invoke(behaviour.GetType());
        }

        /// <summary>
        /// This function is used to refresh the autocomplete suggestions.
        /// It should be used when a new command has been added or removed at run time.
        /// </summary>
        public static void RemoveAutocomplete(MonoBehaviour behaviour)
        {
            RemoveAutocompleteReceived?.Invoke(behaviour.GetType());
        }

        /// <summary>
        /// Gets all logs of the console.
        /// </summary>
        public static LogMessage[] GetLogs()
        {
            return LogMessages.ToArray();
        }

        /// <summary>
        /// Gets user's history logs of the console from the start of the application
        /// </summary>
        public static LogMessage[] GetHistoryLogs()
        {
            return LogMessagesHistory.ToArray();
        }
    }
}
