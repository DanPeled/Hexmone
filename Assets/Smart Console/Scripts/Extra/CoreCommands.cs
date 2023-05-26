/* 
  Package Name: Smart Console
  Version: 2.1.3
  Author: EdgarDev
  Unity Asset Profile: https://assetstore.unity.com/publishers/64126
  Date: 2023-02-22
  Script Name: CoreCommands.cs

  Description:
  This script implements core commands.
*/

using UnityEngine;

namespace ED.SC.Extra
{
	public static class CoreCommands
	{
		[Command("help", "Displays information about the Smart Console asset")]
		public static void PrintHelp()
		{
			SmartConsole.Log("Welcome to Smart Console - a flexible and intuitive in-game command console.\r\nType 'get_commands' for a list of available commands.");
		}

		[Command("version", "Displays the Smart Console version")]
		public static void PrintVersion()
		{
			SmartConsole.Log($"{SmartConsole.Version}\n");
		}

		[Command("quit", "Quits the application")]
		public static void Quit()
		{
			Application.Quit();
		}
	}
}
