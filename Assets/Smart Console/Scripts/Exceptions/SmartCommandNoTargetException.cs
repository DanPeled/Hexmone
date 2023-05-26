/*
  Package Name: Smart Console
  Version: 2.1.3
  Author: EdgarDev
  Unity Asset Profile: https://assetstore.unity.com/publishers/64126
  Date: 2023-02-27
  Script Name: SmartCommandNoTargetException.cs

  Description:
  Custom exception for handling execution of command with no target in the Smart Console.
*/

namespace ED.SC
{
	internal class SmartCommandNoTargetException : SmartException
	{
		internal SmartCommandNoTargetException(Command command)
		: base($"Command '{command.Name}' could not be executed because it has no target available.")
		{
		}
	}
}
