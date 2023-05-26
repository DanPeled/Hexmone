/* 
  Package Name: Smart Console
  Version: 2.1.3
  Author: EdgarDev
  Unity Asset Profile: https://assetstore.unity.com/publishers/64126
  Date: 2023-02-22
  Script Name: ScreenCommands.cs

  Description:
  This script implements screen related commands.
*/

using System;
using System.IO;
using UnityEngine;

namespace ED.SC.Extra
{
	public static class ScreenCommands
	{
		[Command("toggle_fullscreen", "Toggles the fullscreen state of the application")]
		public static void ToggleFullScreen()
		{
			Screen.fullScreen = !Screen.fullScreen;
		}

		[Command("get_screen_resolution", "Gets the current screen resolution")]
		public static void GetScreenResolution()
		{
			SmartConsole.Log($"Current screen resolution is {Screen.currentResolution}.");
		}

		[Command("set_screen_resolution", "Sets the current screen resolution")]
		public static void SetScreenResolution(int width, int height)
		{
			Screen.SetResolution(width, height, Screen.fullScreen);
			SmartConsole.Log($"Updated screen resolution to {Screen.currentResolution}.");
		}

		[Command("capture_screenshot", "Captures a screenshot and saves it to the supplied file path as a PNG.")]
		public static void CaptureScreenshot(int resolutionMultiplier = 1)
		{
			string folderName = "Screenshots";
			string fileName = $"screenshot-0.png";
			string filePath = Path.Combine(Application.persistentDataPath, folderName, fileName);

			int count = 1;

			while (File.Exists(filePath))
			{
				fileName = $"screenshot-{count}.png";
				filePath = Path.Combine(Application.persistentDataPath, folderName, fileName);
				count++;
			}

			// Create the directory if it doesn't exist
			Directory.CreateDirectory(Path.GetDirectoryName(filePath));

			try
			{
				// Take the screenshot and save it to the specified file path
				ScreenCapture.CaptureScreenshot(filePath, resolutionMultiplier);
				SmartConsole.Log($"Screenshot saved to {filePath}.\nPath has been copied to clipboard.");
				GUIUtility.systemCopyBuffer = filePath;
			}
			catch (Exception e)
			{
				SmartConsole.LogWarning(e.Message);
			}
		}
	}
}
