/*
  Package Name: Smart Console
  Version: 2.1.3
  Author: EdgarDev
  Unity Asset Profile: https://assetstore.unity.com/publishers/64126
  Date: 2023-02-12
  Script Name: LogMessageTypes.cs

  Description:
  This script contains the enum definition for the
  different types of log messages in the Smart Console.
*/

namespace ED.SC
{
    public enum LogMessageType
    {
		/// <summary>
		/// LogMessageType used for regular log messages.
		/// </summary>
		Log,
		/// <summary>
		/// LogMessageType used for Commands.
		/// </summary>
		Command,
		/// <summary>
		/// LogMessageType used to Autocomplete commands.
		/// </summary>
		Autocompletion,
		/// <summary>
		/// LogMessageType used for Warnings.
		/// </summary>
		Warning,
		/// <summary>
		/// LogMessageType used for Errors.
		/// </summary>
		Error
	}
}
