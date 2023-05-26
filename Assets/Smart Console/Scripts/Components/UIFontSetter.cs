/*
  Package Name: Smart Console
  Version: 2.1.3
  Author: EdgarDev
  Unity Asset Profile: https://assetstore.unity.com/publishers/64126
  Date: 2023-03-14
  Script Name: UIFontSetter.cs

  Description:
  This script sets font UI following preferences font.
*/

using UnityEngine;
using TMPro;

namespace ED.SC.Components
{
	public class UIFontSetter : MonoBehaviour
	{
		[SerializeField] private SmartConsolePreferences m_Preferences;
		[SerializeField] private TMP_InputField m_InputField;
		[SerializeField] private TextMeshProUGUI[] m_Texts;

		private void Start()
		{
			m_InputField.fontAsset = m_Preferences.GlobalFont;

			foreach (var textMeshPro in m_Texts)
			{
				textMeshPro.font = m_Preferences.GlobalFont;
			}
		}
	}
}