/* 
  Package Name: Smart Console
  Version: 2.1.3
  Author: EdgarDev
  Unity Asset Profile: https://assetstore.unity.com/publishers/64126
  Date: 2023-03-05 
  Script Name: HelpEditor.cs

  Description:
  This script implements a help menu for the console.
*/

using UnityEditor;
using UnityEngine;

namespace ED.SC.Editor
{
	public class HelpEditor : UnityEditor.Editor
	{
		[MenuItem("Tools/Smart Console/Help/Discord", false, 50)]
		private static void DiscordLinkTo()
		{
			Application.OpenURL("https://discord.com/invite/NCcNP7jeCh");
		}

		[MenuItem("Tools/Smart Console/Help/Contact", false, 50)]
		private static void MailTo()
		{
			Application.OpenURL($"mailto:edgarcovarel@yahoo.fr");
		}
	}
}
