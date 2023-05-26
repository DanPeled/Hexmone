/*
  Package Name: Smart Console
  Version: 2.1.3
  Author: EdgarDev
  Unity Asset Profile: https://assetstore.unity.com/publishers/64126
  Date: 2023-03-05
  Script Name: SmartParameterNullException.cs

  Description:
  Custom exception for handling null parameters in the Smart Console.
*/

namespace ED.SC
{
	internal class SmartParameterNullException : SmartException
	{
		internal SmartParameterNullException(string parameterName, Command command)
		: base($"Parameter '{parameterName}' of command '{command.Name}' cannot be null.")
		{
		}
	}
}
