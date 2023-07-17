using System.Collections;
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

        [Command(
            "capture_screenshot",
            "Captures a screenshot and triggers a pop-up download screen."
        )]
        public static void CaptureScreenshot(int resolutionMultiplier = 1)
        {
            // Create a RenderTexture with the desired resolution
            int width = Screen.width * resolutionMultiplier;
            int height = Screen.height * resolutionMultiplier;
            RenderTexture renderTexture = new RenderTexture(width, height, 24);

            // Set the active RenderTexture for rendering
            RenderTexture.active = renderTexture;

            // Capture the current screen and store it in the RenderTexture
            Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
            screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            screenshot.Apply();

            // Encode the screenshot to PNG format
            byte[] bytes = screenshot.EncodeToPNG();

            // Save the PNG data to a temporary file path
            string temporaryFilePath = Path.Combine(
                Application.persistentDataPath,
                "screenshot_temp.png"
            );
            File.WriteAllBytes(temporaryFilePath, bytes);

            // Cleanup
            RenderTexture.active = null;
            GameObject.Destroy(renderTexture);
            GameObject.Destroy(screenshot);

            // Trigger the pop-up download screen using JavaScript via Unity's Application.OpenURL
            string script =
                $@"
        var link = document.createElement('a');
        link.href = '{temporaryFilePath}';
        link.download = 'screenshot.png';
        link.click();
        link.remove();
    ";
            string encodedScript = Uri.EscapeDataString(script);
            string url = $"javascript:(function(){{ {encodedScript} }})();";
            Application.OpenURL(url);

            // Delete the temporary file
            File.Delete(temporaryFilePath);

            SmartConsole.Log("Screenshot capture complete.");
        }
    }
}
