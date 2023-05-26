/*
  Package Name: Smart Console
  Version: 2.1.3
  Author: EdgarDev
  Unity Asset Profile: https://assetstore.unity.com/publishers/64126
  Date: 2023-04-09
  Script Name: ConsoleTests.cs

  Description:
  Tests for the Smart Console.
  Please uncomment INCLUDE_TESTS definition to enable tests.
  It should be uncommented in both ConsoleTests.cs and TestCommands.cs files.
  Note that this is by default disabled to not load tests commands.*/

// uncomment this
//#define INCLUDE_TESTS

#if INCLUDE_TESTS
using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace ED.SC.Tests
{
    public class ConsoleTests
    {
        private GameObject m_ConsolePrefab;
        private GameObject m_CommandTests;

        [OneTimeSetUp]
        public void SetupScene()
        {
			SmartConsoleCache cache = AssetDatabase.LoadAssetAtPath<SmartConsoleCache>("Assets/Smart Console/Data/SC.Cache.asset");

			if (cache == null)
			{
				Debug.LogError("Smart Error: Cannot reload cache at setup time.");
				return;
			}

			cache.Clear();
			cache.Load();

			GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Smart Console/Prefabs/Smart Console.prefab");
            m_ConsolePrefab = GameObject.Instantiate(prefab);

			m_CommandTests = new GameObject("CommandTests", typeof(TestCommands));
        }

        [OneTimeTearDown]
        public void CleanScene()
        {
            GameObject.Destroy(m_ConsolePrefab);
            GameObject.Destroy(m_CommandTests);
        }

        [TearDown]
        public void ClearConsole()
        {
			SmartConsole.Clear();
        }

		#region Hello World tests
		[UnityTest]
        public IEnumerator Print_hello_world()
        {
            // Arrange
            const string message = "Hello World!";

			SmartConsole.Log(message);
            
            yield return null;

			// Act
			LogMessage[] logs = SmartConsole.GetLogs();
            LogMessage lastLog = logs[logs.Length - 1];

			// Assert
			Assert.IsTrue(
                string.Equals(lastLog.Text, message), 
                $"Log message is \"{lastLog.Text}\" instead of \"{message}\"");
        }

		[UnityTest]
		public IEnumerator Use_hello_world_command()
		{
			// Arrange
			const string command = "print_hello_world";
			const string log = "Hello World!";

			SmartConsole.Send(command);

			yield return null;

			yield return new WaitForSeconds(1);

			// Act
			LogMessage[] historyLogs = SmartConsole.GetHistoryLogs();
			LogMessage lastHistoryLog = historyLogs[historyLogs.Length - 1];

			LogMessage[] logs = SmartConsole.GetLogs();
			LogMessage lastLog = logs[logs.Length - 1];

			// Assert
			Assert.IsTrue(
				lastHistoryLog.Type == LogMessageType.Command,
				$"\"{command}\" is detected as a {lastHistoryLog.Type} instead of a {LogMessageType.Command}");
			
			Assert.IsTrue(
				string.Equals(lastLog.Text, log),
				$"Log message is \"{lastLog.Text}\" instead of \"{log}\"");
		}
		#endregion

		#region Bool tests
		[UnityTest]
		public IEnumerator Use_logging_argument_bool_command()
		{
			// Arrange
			const string argument = "True";
			const string command = "print_bool" + " " + argument;

			SmartConsole.Send(command);

			yield return null;

			// Act
			LogMessage[] historyLogs = SmartConsole.GetHistoryLogs();
			LogMessage lastHistoryLog = historyLogs[historyLogs.Length - 1];

			LogMessage[] logs = SmartConsole.GetLogs();
			LogMessage lastLog = logs[logs.Length - 1];

			// Assert
			Assert.IsTrue(
				lastHistoryLog.Type == LogMessageType.Command,
				$"\"{command}\" is detected as a {lastHistoryLog.Type} instead of a {LogMessageType.Command}");
			
			Assert.IsTrue(
				string.Equals(lastLog.Text, argument),
				$"Log message is \"{lastLog.Text}\" instead of \"{argument}\"");
		}

		[UnityTest]
		public IEnumerator Use_logging_argument_bool_optional_command()
		{
			// Arrange
			const string argument = "True";
			const string command = "print_bool_optional";

			SmartConsole.Send(command);

			yield return null;

			// Act
			LogMessage[] historyLogs = SmartConsole.GetHistoryLogs();
			LogMessage lastHistoryLog = historyLogs[historyLogs.Length - 1];

			LogMessage[] logs = SmartConsole.GetLogs();
			LogMessage lastLog = logs[logs.Length - 1];

			// Assert
			Assert.IsTrue(
				lastHistoryLog.Type == LogMessageType.Command,
				$"\"{command}\" is detected as a {lastHistoryLog.Type} instead of a {LogMessageType.Command}");
			
			Assert.IsTrue(
				string.Equals(lastLog.Text, argument),
				$"Log message is \"{lastLog.Text}\" instead of \"{argument}\"");
		}
		#endregion

		#region Enum tests
		[UnityTest]
		public IEnumerator Use_logging_argument_enum_command()
		{
			// Arrange
			const string argument = "Three";
			const string command = "print_enum" + " " + argument;

			SmartConsole.Send(command);

			yield return null;

			// Act
			LogMessage[] historyLogs = SmartConsole.GetHistoryLogs();
			LogMessage lastHistoryLog = historyLogs[historyLogs.Length - 1];

			LogMessage[] logs = SmartConsole.GetLogs();
			LogMessage lastLog = logs[logs.Length - 1];

			// Assert
			Assert.IsTrue(
				lastHistoryLog.Type == LogMessageType.Command,
				$"\"{command}\" is detected as a {lastHistoryLog.Type} instead of a {LogMessageType.Command}");
			
			Assert.IsTrue(
				string.Equals(lastLog.Text, argument),
				$"Log message is \"{lastLog.Text}\" instead of \"{argument}\"");
		}

		[UnityTest]
		public IEnumerator Use_logging_argument_enum_optional_command()
		{
			// Arrange
			const string argument = "Three";
			const string command = "print_enum_optional";

			SmartConsole.Send(command);

			yield return null;

			// Act
			LogMessage[] historyLogs = SmartConsole.GetHistoryLogs();
			LogMessage lastHistoryLog = historyLogs[historyLogs.Length - 1];

			LogMessage[] logs = SmartConsole.GetLogs();
			LogMessage lastLog = logs[logs.Length - 1];

			// Assert
			Assert.IsTrue(
				lastHistoryLog.Type == LogMessageType.Command,
				$"\"{command}\" is detected as a {lastHistoryLog.Type} instead of a {LogMessageType.Command}");

			Assert.IsTrue(
				string.Equals(lastLog.Text, argument),
				$"Log message is \"{lastLog.Text}\" instead of \"{argument}\"");
		}
		#endregion

		#region Float tests
		[UnityTest]
		public IEnumerator Use_logging_argument_float_command()
		{
			// Arrange
			const string argument = "1.2";
			const string command = "print_float" + " " + argument;

			SmartConsole.Send(command);

			yield return null;

			// Act
			LogMessage[] historyLogs = SmartConsole.GetHistoryLogs();
			LogMessage lastHistoryLog = historyLogs[historyLogs.Length - 1];

			LogMessage[] logs = SmartConsole.GetLogs();
			LogMessage lastLog = logs[logs.Length - 1];

			// Assert
			Assert.IsTrue(
				lastHistoryLog.Type == LogMessageType.Command,
				$"\"{command}\" is detected as a {lastHistoryLog.Type} instead of a {LogMessageType.Command}");

			string normalizeArgument = argument.Replace('.', ',');

			Assert.IsTrue(
				string.Equals(lastLog.Text, normalizeArgument),
				$"Log message is \"{lastLog.Text}\" instead of \"{normalizeArgument}\"");
		}

		[UnityTest]
		public IEnumerator Use_logging_argument_float_optional_command()
		{
			// Arrange
			const string argument = "1.2";
			const string command = "print_float_optional";

			SmartConsole.Send(command);

			yield return null;

			// Act
			LogMessage[] historyLogs = SmartConsole.GetHistoryLogs();
			LogMessage lastHistoryLog = historyLogs[historyLogs.Length - 1];

			LogMessage[] logs = SmartConsole.GetLogs();
			LogMessage lastLog = logs[logs.Length - 1];

			// Assert
			Assert.IsTrue(
				lastHistoryLog.Type == LogMessageType.Command,
				$"\"{command}\" is detected as a {lastHistoryLog.Type} instead of a {LogMessageType.Command}");

			string normalizeArgument = argument.Replace('.', ',');

			Assert.IsTrue(
				string.Equals(lastLog.Text, normalizeArgument),
				$"Log message is \"{lastLog.Text}\" instead of \"{normalizeArgument}\"");
		}
		#endregion

		#region Int tests
		[UnityTest]
		public IEnumerator Use_logging_argument_int_command()
		{
			// Arrange
			const string argument = "1";
			const string command = "print_int" + " " + argument;

			SmartConsole.Send(command);

			yield return null;

			// Act
			LogMessage[] historyLogs = SmartConsole.GetHistoryLogs();
			LogMessage lastHistoryLog = historyLogs[historyLogs.Length - 1];

			LogMessage[] logs = SmartConsole.GetLogs();
			LogMessage lastLog = logs[logs.Length - 1];

			// Assert
			Assert.IsTrue(
				lastHistoryLog.Type == LogMessageType.Command,
				$"\"{command}\" is detected as a {lastHistoryLog.Type} instead of a {LogMessageType.Command}");
			
			Assert.IsTrue(
				string.Equals(lastLog.Text, argument),
				$"Log message is \"{lastLog.Text}\" instead of \"{argument}\"");
		}

		[UnityTest]
		public IEnumerator Use_logging_argument_int_optional_command()
		{
			// Arrange
			const string argument = "1";
			const string command = "print_int_optional";

			SmartConsole.Send(command);

			yield return null;

			// Act
			LogMessage[] historyLogs = SmartConsole.GetHistoryLogs();
			LogMessage lastHistoryLog = historyLogs[historyLogs.Length - 1];

			LogMessage[] logs = SmartConsole.GetLogs();
			LogMessage lastLog = logs[logs.Length - 1];

			// Assert
			Assert.IsTrue(
				lastHistoryLog.Type == LogMessageType.Command,
				$"\"{command}\" is detected as a {lastHistoryLog.Type} instead of a {LogMessageType.Command}");
			
			Assert.IsTrue(
				string.Equals(lastLog.Text, argument),
				$"Log message is \"{lastLog.Text}\" instead of \"{argument}\"");
		}
		#endregion

		#region String tests
		[UnityTest]
		public IEnumerator Use_logging_argument_string_command()
		{
			// Arrange
			const string argument = "success";
			const string command = "print_string" + " " + argument;

			SmartConsole.Send(command);

			yield return null;

			// Act
			LogMessage[] historyLogs = SmartConsole.GetHistoryLogs();
			LogMessage lastHistoryLog = historyLogs[historyLogs.Length - 1];

			LogMessage[] logs = SmartConsole.GetLogs();
			LogMessage lastLog = logs[logs.Length - 1];

			// Assert
			Assert.IsTrue(
				lastHistoryLog.Type == LogMessageType.Command,
				$"\"{command}\" is detected as a {lastHistoryLog.Type} instead of a {LogMessageType.Command}");
			
			Assert.IsTrue(
				string.Equals(lastLog.Text, argument),
				$"Log message is \"{lastLog.Text}\" instead of \"{argument}\"");
		}

		[UnityTest]
		public IEnumerator Use_logging_argument_string_optional_command()
		{
			// Arrange
			const string argument = "success";
			const string command = "print_string_optional";

			SmartConsole.Send(command);

			yield return null;

			// Act
			LogMessage[] historyLogs = SmartConsole.GetHistoryLogs();
			LogMessage lastHistoryLog = historyLogs[historyLogs.Length - 1];

			LogMessage[] logs = SmartConsole.GetLogs();
			LogMessage lastLog = logs[logs.Length - 1];

			// Assert
			Assert.IsTrue(
				lastHistoryLog.Type == LogMessageType.Command,
				$"\"{command}\" is detected as a {lastHistoryLog.Type} instead of a {LogMessageType.Command}");
			
			Assert.IsTrue(
				string.Equals(lastLog.Text, argument),
				$"Log message is \"{lastLog.Text}\" instead of \"{argument}\"");
		}
		#endregion
        
        [UnityTest]
        public IEnumerator Use_logging_multiple_argument_command()
        {
            // Arrange
            const string argument = "FirstString 1 SecondString 2";
            const string command = "print_strings_and_ints" + " " + argument;

			SmartConsole.Send(command);

			yield return null;

			// Act
			LogMessage[] historyLogs = SmartConsole.GetHistoryLogs();
			LogMessage lastHistoryLog = historyLogs[historyLogs.Length - 1];

			LogMessage[] logs = SmartConsole.GetLogs();
			LogMessage lastLog = logs[logs.Length - 1];

			// Assert
			Assert.IsTrue(
				lastHistoryLog.Type == LogMessageType.Command, 
                $"\"{command}\" is detected as a {lastHistoryLog.Type} instead of a {LogMessageType.Command}");
            
			Assert.IsTrue(
                string.Equals(lastLog.Text, argument), 
                $"Log message is \"{lastLog.Text}\" instead of \"{argument}\"");
        }
        
        [UnityTest]
        public IEnumerator Use_logging_argument_x_time_command()
        {
            // Arrange
            const string str = "string";
            const int n = 3;
            
            const string firstArgument = str;
            string secondArgument = n.ToString();
            string command = "print_string_x_time" + " " + firstArgument + " " + secondArgument;

			SmartConsole.Send(command);

			yield return null;

			// Act
			LogMessage[] historyLogs = SmartConsole.GetHistoryLogs();
			LogMessage lastHistoryLog = historyLogs[historyLogs.Length - 1];
			
			LogMessage[] logs = SmartConsole.GetLogs();
			LogMessage[] lastLogs = logs.Skip(Math.Max(0, logs.Count() - n)).ToArray();

			// Assert
			Assert.IsTrue(
				lastHistoryLog.Type == LogMessageType.Command, 
                $"\"{command}\" is detected as a {lastHistoryLog.Type} instead of a {LogMessageType.Command}");

            Assert.IsTrue(
				lastLogs.Length == n, 
                $"Log messages length is {lastLogs.Length} instead of {n}");
            
            for (int i = 0; i < n; i++)
            {
                Assert.IsTrue(
                    string.Equals(lastLogs[i].Text, str), 
                    $"Log message is \"{lastLogs[i].Text}\" instead of \"{str}\"");
            }
        }
    }
}
#endif
