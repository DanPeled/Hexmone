/*
  Package Name: Smart Console
  Version: 2.1.3
  Author: EdgarDev
  Unity Asset Profile: https://assetstore.unity.com/publishers/64126
  Date: 2023-03-05
  Script Name: SmartParameterInvalidCastException.cs

  Description:
  Custom exception for handling invalid parameters casting in the Smart Console.
*/

namespace ED.SC
{
	internal class SmartParameterInvalidCastException : SmartException
	{
		internal SmartParameterInvalidCastException(string inputParameter, Command command, string parameterTypeName)
		: base($"Parameter '{inputParameter}' of command '{command.Name}' could not be cast to type '{parameterTypeName}'.")
		{
		}
	}
}
