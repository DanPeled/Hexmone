/*
  Package Name: Smart Console
  Version: 2.1.3
  Author: EdgarDev
  Unity Asset Profile: https://assetstore.unity.com/publishers/64126
  Date: 2023-02-20
  Script Name: SmartParameterTooManyException.cs

  Description:
  Custom exception for handling too many parameters in the Smart Console.
*/

namespace ED.SC
{
	internal class SmartParameterTooManyException : SmartException
	{
		internal SmartParameterTooManyException(Command command, int inputParametersLength)
		: base($"Command '{command.Name}' takes {command.Method.GetParameters().Length} argument(s) but you are trying to execute it with {inputParametersLength} argument(s).")
		{
		}
	}
}
