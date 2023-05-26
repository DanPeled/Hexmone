/*
  Package Name: Smart Console
  Version: 2.1.3
  Author: EdgarDev
  Unity Asset Profile: https://assetstore.unity.com/publishers/64126
  Date: 2023-02-12
  Script Name: Highlightor.cs

  Description:
  This script implements a highlighting system for the console.
  It is used to highlight the selected autocomplete log message as the user
  navigates through the list of suggestions provided by the autocompletion system.
*/

using System;
using UnityEngine;

namespace ED.SC.Components
{
	[RequireComponent(typeof(Autocompletor))]
	public class Highlightor : MonoBehaviour
	{
		private Autocompletor m_AutocompletionManager;
		private LogMessage m_HighlightAutocomplete;

		private void Awake()
		{
			m_AutocompletionManager = gameObject.GetComponent<Autocompletor>();
		}

		private void OnEnable()
		{
			m_AutocompletionManager.OnCopyAutocompletion += HighlightAutocomplete;
			m_AutocompletionManager.OnBreakAutocompletion += UnhighlightAutocomplete;
		}

		private void OnDisable()
		{
			m_AutocompletionManager.OnCopyAutocompletion -= HighlightAutocomplete;
			m_AutocompletionManager.OnBreakAutocompletion -= UnhighlightAutocomplete;
		}

		/// <summary>
		/// Highlight autocomplete log message, unhighlight highlighted autcomplete if exists
		/// </summary>
		/// <param name="logMessage">the log message to highlight</param>
		private void HighlightAutocomplete(LogMessage logMessage)
		{
			if (m_HighlightAutocomplete != null)
			{
				if (m_HighlightAutocomplete.LogMessageSetup == null)
				{
					// autcomplete has been cleared
					m_HighlightAutocomplete = null;
				}
				else
				{
					m_HighlightAutocomplete.LogMessageSetup.UnhighlightText();
				}
			}

			if (logMessage.LogMessageSetup == null)
			{
				throw new NullReferenceException("LogMessageSetup");
			}

			logMessage.LogMessageSetup.HighlightText();
			m_HighlightAutocomplete = logMessage;
		}

		/// <summary>
		/// Unhighlight highlighted autcomplete if exists
		/// </summary>
		private void UnhighlightAutocomplete()
		{
			if (m_HighlightAutocomplete == null)
			{
				return;
			}

			if (m_HighlightAutocomplete.LogMessageSetup == null)
			{
				// autcomplete has been cleared
				m_HighlightAutocomplete = null;
				return;
			}

			m_HighlightAutocomplete.LogMessageSetup.UnhighlightText();
		}
	}
}
