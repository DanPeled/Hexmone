/* 
  Package Name: Smart Console
  Version: 2.1.3
  Author: EdgarDev
  Unity Asset Profile: https://assetstore.unity.com/publishers/64126
  Date: 2023-03-07
  Script Name: PlayerPrefsCommands.cs

  Description:
  This script implements player prefs related commands.
*/

using UnityEngine;

namespace ED.SC.Extra
{
	public static class PlayerPrefsCommands
	{
		[Command("get_playerprefs_key", "Gets the value of a players prefs key")]
		public static void GetPlayerPrefsKey(string key)
		{
			if (!PlayerPrefs.HasKey(key))
			{
				SmartConsole.Log($"Key {key} does not exist");
				return;
			}

			if (PlayerPrefs.GetInt(key) != 0)
			{
				// Key exists and stores an integer value
				int value = PlayerPrefs.GetInt(key);
				SmartConsole.Log($"Key {key} stores an integer value of {value}");
			}
			else if (PlayerPrefs.GetFloat(key) != 0f)
			{
				// Key exists and stores a float value
				float value = PlayerPrefs.GetFloat(key);
				SmartConsole.Log($"Key {key} stores an float value of {value}");
			}
			else if (!string.IsNullOrEmpty(PlayerPrefs.GetString(key)))
			{
				// Key exists and stores a string value
				string value = PlayerPrefs.GetString(key);
				SmartConsole.Log($"Key {key} stores an string value of {value}");
			}
			else
			{
				SmartConsole.Log($"Key {key} stores the default type's value");
			}
		}

		[Command("set_playerprefs_key_float", "Sets the value of a players prefs float key")]
		public static void SetPlayerPrefsKeyFloat(string key, float value)
		{
			PlayerPrefs.SetFloat(key, value);
			SmartConsole.Log($"Key {key} stores an float value of {value}.");
		}

		[Command("set_playerprefs_key_int", "Sets the value of a players prefs int key")]
		public static void SetPlayerPrefsKeyInt(string key, int value)
		{
			PlayerPrefs.SetInt(key, value);
			SmartConsole.Log($"Key {key} stores an integer value of {value}.");
		}

		[Command("set_playerprefs_key_string", "Sets the value of a players prefs string key")]
		public static void SetPlayerPrefsKeyString(string key, string value)
		{
			PlayerPrefs.SetString(key, value);
			SmartConsole.Log($"Key {key} stores a string value of {value}.");
		}

		[Command("delete_playerprefs_key", "Deletes a players prefs key")]
		public static void DeletePlayerPrefsKey(string key)
		{
			PlayerPrefs.DeleteKey(key);
			SmartConsole.Log($"Key {key} has been deleted.");
		}

		[Command("delete_playerprefs", "Deletes all the players prefs")]
		public static void DeleteAllPlayerPrefs()
		{
			PlayerPrefs.DeleteAll();
			SmartConsole.Log("All keys has been deleted.");
		}
	}
}

