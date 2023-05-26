/*
  Package Name: Smart Console
  Version: 2.1.3
  Author: EdgarDev
  Unity Asset Profile: https://assetstore.unity.com/publishers/64126
  Date: 2023-02-12
  Script Name: LogMessage.cs

  Description:
  This class represent a log message in the Smart Console.
*/

using ED.SC.Components;
using System.Reflection;

namespace ED.SC
{
    public class LogMessage
    {
        public string Text;
        public LogMessageType Type;
		public ParameterInfo[] Parameters;
		public LogMessageSetup LogMessageSetup { get; set; }

		public LogMessage(string text, LogMessageType type)
		{
			Text = text;
			Type = type;
			Parameters = null;
		}

		public LogMessage(string text, LogMessageType type, ParameterInfo[] parameters)
        {
            Text = text;
            Type = type;
            Parameters = parameters;
        }
	}
}
