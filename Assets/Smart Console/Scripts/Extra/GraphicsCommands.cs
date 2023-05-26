/* 
  Package Name: Smart Console
  Version: 2.1.3
  Author: EdgarDev
  Unity Asset Profile: https://assetstore.unity.com/publishers/64126
  Date: 2023-02-22
  Script Name: GraphicsCommands.cs

  Description:
  This script implements graphics related commands.
*/

using UnityEngine;

namespace ED.SC.Extra
{
	public static class GraphicsCommands
	{
		[Command("get_max_fps", "Gets the frame rate at which Unity tries to render on the application")]
		public static void GetMaxFPS()
		{
			SmartConsole.Log($"Target frame rate is {Application.targetFrameRate}.");
		}

		[Command("set_max_fps", "Sets the frame rate at which Unity tries to render on the application. Set to -1 for unlimited")]
		public static void SetMaxFPS(int newTargetFrameRate)
		{
			Application.targetFrameRate = newTargetFrameRate;
			SmartConsole.Log($"Updated target frame rate to {Application.targetFrameRate}.");
		}

		[Command("set_vsync", "Enables or disables vsync for the application")]
		public static void SetVSync(bool enable)
		{
			QualitySettings.vSyncCount = enable ? 1 : 0;
			string enableMessage = enable ? "Enabled" : "Disabled";
			SmartConsole.Log($"{enableMessage} vsync.");
		}
	}
}
