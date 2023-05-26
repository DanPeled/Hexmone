/*
  Package Name: Smart Console
  Version: 2.1.3
  Author: EdgarDev
  Unity Asset Profile: https://assetstore.unity.com/publishers/64126
  Date: 2023-03-03
  Script Name: LogMessageSetup.cs

  Description:
  This script sets up the instances of log messages.
*/

using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ED.SC.Components
{
    public class LogMessageSetup : MonoBehaviour
    {
		[SerializeField] private SmartConsolePreferences m_Preferences;
		[SerializeField] private TextMeshProUGUI m_Text;
        [SerializeField] private Image m_Background;

        private Color m_TextColor;

        private void Start()
        {
            m_TextColor = m_Text.color;
        }

		public void SetText(string text, Color textColor, TMP_FontAsset font, float fontSize)
        {
			m_Text.text = $"{text}";
			m_Text.color = textColor;
            m_Text.font = font;
            m_Text.fontSize = fontSize;
		}

        public void HighlightText()
        {
            m_Text.color = m_Preferences.HighlightTextColor;
        }
        
        public void UnhighlightText()
        {
            m_Text.color = m_TextColor;
        }
        
        public void SetTextParameter(ParameterInfo[] parameters)
        {
            string colorHex = ColorUtility.ToHtmlStringRGBA(m_Preferences.AutocompleteParameterTextColor);

            for (int i = 0; i < parameters.Length; i++)
            {
                m_Text.text += $"<i><color=#{colorHex}> {parameters[i].Name}</color></i>";

                if (parameters[i].HasDefaultValue)
                {
					m_Text.text += $"<i><color=#{colorHex}> = {parameters[i].DefaultValue.ToString()}</color></i>";
				}
            }
        }

        public void SetBackgroundColor(Color color)
        {
			m_Background.color = color;
        }

        public void DisableSelection()
        {
            LogMessageSelector selector = gameObject.GetComponent<LogMessageSelector>();

            if (selector != null)
            {
                selector.enabled = false;
			}
        }
    }
}
