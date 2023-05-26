/*
  Package Name: Smart Console
  Version: 2.1.3
  Author: EdgarDev
  Unity Asset Profile: https://assetstore.unity.com/publishers/64126
  Date: 2023-03-14
  Script Name: UIBackgroundSetter.cs

  Description:
  This script sets background UI following preferences colors.
*/

using UnityEngine;
using UnityEngine.UI;

namespace ED.SC.Components
{
	public class UIBackgroundSetter : MonoBehaviour
	{
		[SerializeField] private SmartConsolePreferences m_Preferences;
		[SerializeField] CanvasGroup m_CanvasGroup;
		[SerializeField] Image[] m_Backgrounds;

		private void Start()
		{
			m_CanvasGroup.alpha = m_Preferences.Opacity;

			foreach (var background in m_Backgrounds )
			{
				background.color = m_Preferences.BackgroundMainColor;
			}
		}
	}
}