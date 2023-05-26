/*
  Package Name: Smart Console
  Version: 2.1.3
  Author: EdgarDev
  Unity Asset Profile: https://assetstore.unity.com/publishers/64126
  Date: 2023-03-03
  Script Name: LogMessageSelector.cs

  Description:
  This script ensures the selection of log message instances.
*/

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ED.SC.Components
{
    public class LogMessageSelector : MonoBehaviour, IPointerClickHandler
    {
		[SerializeField] private SmartConsolePreferences m_Preferences;
		[SerializeField] private Image m_Background;
        
        #region Events
        
        // Select event
        public static event Action<int> OnSelectLogMessage;
        
        #endregion
        
        private Color m_BackgroundColor;
        private int m_Hash;
        
        private void Start()
        {
            m_BackgroundColor = m_Background.color;
            m_Hash = gameObject.GetHashCode();
        }
        
        private void OnEnable() => OnSelectLogMessage += OnLogMessageSelected;

        private void OnDisable() => OnSelectLogMessage -= OnLogMessageSelected;
        
        public void OnPointerClick(PointerEventData eventData) => OnSelectLogMessage?.Invoke(m_Hash);

        private void OnLogMessageSelected(int hash)
        {
            if (hash == m_Hash)
            {
				m_Background.color = m_Preferences.BackgroundSelectionColor;
			}
            else if (m_Background.color == m_Preferences.BackgroundSelectionColor)
            {
                m_Background.color = m_BackgroundColor;
			}
        }
    }
}
