/* 
  Package Name: Smart Console
  Version: 2.1.3
  Author: EdgarDev
  Unity Asset Profile: https://assetstore.unity.com/publishers/64126
  Date: 2023-04-08 
  Script Name: PreferencesEditorWindow.cs

  Description:
  This script implements a preferences menu to customize the console.
*/

using UnityEditor;
using UnityEngine;
using TMPro;

namespace ED.SC.Editor
{
	public class PreferencesEditorWindow : EditorWindow
	{
		[SerializeField] private Texture2D m_SmartConsoleIcon;
		[SerializeField] private SmartConsolePreferences m_Preferences;
		[SerializeField] private SmartConsoleCache m_Cache;

		private GUIContent k_GlobalFontContent = new GUIContent("Global Font", "This represents the font used in the Smart Console.");
		private GUIContent k_LogFontSizeContent = new GUIContent("Log", "This represents the font size of logs in the Smart Console.");
		private GUIContent k_AutocompleteFontSizeContent = new GUIContent("Autocomplete", "This represents the font size of autocompletes in the Smart Console.");
		private GUIContent k_LogColorContent = new GUIContent("Log", "This represents the color of logs in the Smart Console.");
		private GUIContent k_CommandColorContent = new GUIContent("Command", "This represents the color of commands in the Smart Console.");
		private GUIContent k_AutocompleteColorContent = new GUIContent("Autocomplete", "This represents the color of autocompletes in the Smart Console.");
		private GUIContent k_AutocompleteParameterColorContent = new GUIContent("Autocomplete Parameter", "This represents the color of parameters in the Smart Console.");
		private GUIContent k_WarningColorContent = new GUIContent("Warning", "This represents the color of warnings in the Smart Console.");
		private GUIContent k_ErrorColorContent = new GUIContent("Error", "This represents the color of errors in the Smart Console.");
		private GUIContent k_HighlightColorContent = new GUIContent("Highlight", "This represents the color of highlight in the Smart Console.");
		private GUIContent k_MainBackgroundColorContent = new GUIContent("Main", "This represents the color of the background in the Smart Console.");
		private GUIContent k_EvenBackgroundColorContent = new GUIContent("Even Line", "This represents the color of even line in the Smart Console.");
		private GUIContent k_SelectionBackgroundColorContent = new GUIContent("Selection", "This represents the color of selection in the Smart Console.");
		private GUIContent k_AutocompleteBackgroundColorContent = new GUIContent("Autocomplete", "This represents the color of autocompletes in the Smart Console.");
		private GUIContent k_CacheReloadOnPlayModeContent = new GUIContent("Cache Reload on Play", "This represents the state of automatic reload of cache on entering in play mode in the Smart Console.");
		private GUIContent k_LoadCacheContent = new GUIContent("Load Cache", "Manual load cache of the Smart Console.");
		private GUIContent k_ClearCacheContent = new GUIContent("Clear Cache", "Manual clear cache of the Smart Console.");

		// Begin settings
		private TMP_FontAsset m_GlobalFont;

		private float m_Opacity;

		private float m_LogTextFontSize;
		private float m_AutocompleteTextFontSize;

		private Color m_LogTextColor;
		private Color m_CommandTextColor;
		private Color m_WarningTextColor;
		private Color m_ErrorTextColor;
		private Color m_AutocompleteTextColor;
		private Color m_AutocompleteParameterTextColor;
		private Color m_HighlightTextColor;

		private Color m_BackgroundMainColor;
		private Color m_BackgroundEvenColor;
		private Color m_BackgroundSelectionColor;
		private Color m_BackgroundAutocompleteColor;

		private bool m_CacheReloadOnPlayMode;
		// End settings

		private bool m_ShowSaveMessage;
		private float m_ShowSaveTime;

		private static PreferencesEditorWindow m_Window;
		private static Texture2D m_Icon;

		private void OnEnable()
		{
			minSize = new Vector2(300, 600);

			if (m_SmartConsoleIcon == null)
			{
				m_Icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Smart Console/Sprites/Smart Console Logo.png");

				if (m_Icon == null)
				{
					Debug.LogError("Smart Error: Cannot load preferences window icon");
				}
			}
			else
			{
				m_Icon = m_SmartConsoleIcon;
			}

			if (m_Preferences == null)
			{
				m_Preferences = AssetDatabase.LoadAssetAtPath<SmartConsolePreferences>("Assets/Smart Console/Themes/Smart Console Preferences.asset");

				if (m_Preferences == null)
				{
					Debug.LogError("Smart Error: Cannot load preferences scriptable object");
				}
			}
		}

		[MenuItem("Tools/Smart Console/Preferences", false, 21)]
		public static void ShowWindow()
		{
			m_Window = GetWindow<PreferencesEditorWindow>();

			if (m_Icon != null)
			{
				m_Window.titleContent = new GUIContent("Preferences", m_Icon);
			}
			else
			{
				m_Window.titleContent = new GUIContent("Preferences");
			}

			m_Window.LoadPreferences();
		}

		private void OnGUI()
		{
			EditorGUILayout.LabelField($"Smart Console Version {SmartConsole.Version}", EditorStyles.miniLabel);

			if (m_Preferences == null)
			{
				EditorGUILayout.LabelField("Smart Error: Cannot load preferences scriptable object", EditorStyles.label);
				EditorGUILayout.LabelField("Please assign so references in PreferencesEditorWindow.cs", EditorStyles.label);

				return;
			}

			EditorGUILayout.Space();

			m_GlobalFont = (TMP_FontAsset)EditorGUILayout.ObjectField(k_GlobalFontContent, m_GlobalFont, typeof(TMP_FontAsset), false);

			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Opacity", EditorStyles.boldLabel);

			m_Opacity = EditorGUILayout.Slider(m_Opacity, 0.0f, 1.0f);

			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Font Sizes", EditorStyles.boldLabel);

			m_LogTextFontSize = EditorGUILayout.FloatField(k_LogFontSizeContent, m_LogTextFontSize);
			m_AutocompleteTextFontSize = EditorGUILayout.FloatField(k_AutocompleteFontSizeContent, m_AutocompleteTextFontSize);

			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Text Colors", EditorStyles.boldLabel);

			m_LogTextColor = EditorGUILayout.ColorField(k_LogColorContent, m_LogTextColor);
			m_CommandTextColor = EditorGUILayout.ColorField(k_CommandColorContent, m_CommandTextColor);
			m_WarningTextColor = EditorGUILayout.ColorField(k_WarningColorContent, m_WarningTextColor);
			m_ErrorTextColor = EditorGUILayout.ColorField(k_ErrorColorContent, m_ErrorTextColor);
			m_AutocompleteTextColor = EditorGUILayout.ColorField(k_AutocompleteColorContent, m_AutocompleteTextColor);
			m_AutocompleteParameterTextColor = EditorGUILayout.ColorField(k_AutocompleteParameterColorContent, m_AutocompleteParameterTextColor);
			m_HighlightTextColor = EditorGUILayout.ColorField(k_HighlightColorContent, m_HighlightTextColor);

			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Background Colors", EditorStyles.boldLabel);

			m_BackgroundMainColor = EditorGUILayout.ColorField(k_MainBackgroundColorContent, m_BackgroundMainColor);
			m_BackgroundEvenColor = EditorGUILayout.ColorField(k_EvenBackgroundColorContent, m_BackgroundEvenColor);
			m_BackgroundSelectionColor = EditorGUILayout.ColorField(k_SelectionBackgroundColorContent, m_BackgroundSelectionColor);
			m_BackgroundAutocompleteColor = EditorGUILayout.ColorField(k_AutocompleteBackgroundColorContent, m_BackgroundAutocompleteColor);

			EditorGUILayout.Space();

			EditorGUILayout.LabelField("System Behaviours", EditorStyles.boldLabel);

			m_CacheReloadOnPlayMode = EditorGUILayout.Toggle(k_CacheReloadOnPlayModeContent, m_CacheReloadOnPlayMode);

			GUILayout.BeginHorizontal();

			if (GUILayout.Button(k_LoadCacheContent, GUILayout.MaxWidth(position.width / 2)))
			{
				m_Cache.Load();
			}

			GUILayout.FlexibleSpace();

			if (GUILayout.Button(k_ClearCacheContent, GUILayout.MaxWidth(position.width / 2)))
			{
				m_Cache.Clear();
			}

			GUILayout.EndHorizontal();

			EditorGUILayout.Space();

			if (GUILayout.Button("Save"))
			{
				SavePreferences();

				m_ShowSaveMessage = true;
				m_ShowSaveTime = Time.realtimeSinceStartup;
				EditorApplication.update += ShowLabelTime;
			}

			if (m_ShowSaveMessage)
			{
				EditorGUILayout.LabelField("Saved!", EditorStyles.boldLabel);
			}

			GUILayout.FlexibleSpace();

			if (GUILayout.Button("Reset To Defaults"))
			{
				m_Preferences.Reset();

				EditorUtility.SetDirty(this);
				AssetDatabase.SaveAssets();

				if (m_Window == null)
				{
					m_Window = GetWindow<PreferencesEditorWindow>();
				}

				m_Window.LoadPreferences();
				m_Window.Repaint();

				m_ShowSaveMessage = true;
				m_ShowSaveTime = Time.realtimeSinceStartup;
				EditorApplication.update += ShowLabelTime;
			}
		}

		private void LoadPreferences()
		{
			if (m_Preferences == null)
			{
				return;
			}

			m_GlobalFont = m_Preferences.GlobalFont;

			m_Opacity = m_Preferences.Opacity;

			m_LogTextFontSize = m_Preferences.LogTextFontSize;
			m_AutocompleteTextFontSize = m_Preferences.AutocompleteTextFontSize;

			m_LogTextColor = m_Preferences.LogTextColor;
			m_CommandTextColor = m_Preferences.CommandTextColor;
			m_WarningTextColor = m_Preferences.WarningTextColor;
			m_ErrorTextColor = m_Preferences.ErrorTextColor;
			m_AutocompleteTextColor = m_Preferences.AutocompleteTextColor;
			m_AutocompleteParameterTextColor = m_Preferences.AutocompleteParameterTextColor;
			m_HighlightTextColor = m_Preferences.HighlightTextColor;

			m_BackgroundMainColor = m_Preferences.BackgroundMainColor;
			m_BackgroundEvenColor = m_Preferences.BackgroundEvenColor;
			m_BackgroundSelectionColor = m_Preferences.BackgroundSelectionColor;
			m_BackgroundAutocompleteColor = m_Preferences.BackgroundAutocompleteColor;

			m_CacheReloadOnPlayMode = m_Preferences.CacheReloadOnPlayMode;
		}

		private void SavePreferences()
		{
			m_Preferences.GlobalFont = m_GlobalFont;

			m_Preferences.Opacity = m_Opacity;

			m_Preferences.LogTextFontSize = m_LogTextFontSize;
			m_Preferences.AutocompleteTextFontSize = m_AutocompleteTextFontSize;

			m_Preferences.LogTextColor = m_LogTextColor;
			m_Preferences.CommandTextColor = m_CommandTextColor;
			m_Preferences.WarningTextColor = m_WarningTextColor;
			m_Preferences.ErrorTextColor = m_ErrorTextColor;
			m_Preferences.AutocompleteTextColor = m_AutocompleteTextColor;
			m_Preferences.AutocompleteParameterTextColor = m_AutocompleteParameterTextColor;
			m_Preferences.HighlightTextColor = m_HighlightTextColor;

			m_Preferences.BackgroundMainColor = m_BackgroundMainColor;
			m_Preferences.BackgroundEvenColor = m_BackgroundEvenColor;
			m_Preferences.BackgroundSelectionColor = m_BackgroundSelectionColor;
			m_Preferences.BackgroundAutocompleteColor = m_BackgroundAutocompleteColor;

			m_Preferences.CacheReloadOnPlayMode = m_CacheReloadOnPlayMode;

			EditorUtility.SetDirty(this);
			AssetDatabase.SaveAssets();
		}

		private void ShowLabelTime()
		{
			float elapsedTime = Time.realtimeSinceStartup - m_ShowSaveTime;

			if (elapsedTime > 1)
			{
				m_ShowSaveMessage = false;
				EditorApplication.update -= ShowLabelTime;
			}

			Repaint();
		}
	}
}
