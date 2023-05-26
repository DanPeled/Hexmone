/*
  Package Name: Smart Console
  Version: 2.1.3
  Author: EdgarDev
  Unity Asset Profile: https://assetstore.unity.com/publishers/64126
  Date: 2023-04-08
  Script Name: SmartConsoleCacheEditor.cs

  Description:
  This class is a custom editor for the cache scriptable object.
*/

using UnityEngine;
using UnityEditor;

namespace ED.SC
{
	[CustomEditor(typeof(SmartConsoleCache))]
	public class SmartConsoleCacheEditor : UnityEditor.Editor
	{
		private SmartConsoleCache m_SmartConsoleCache;

		private void OnEnable()
		{
			m_SmartConsoleCache = (SmartConsoleCache)target;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if (m_SmartConsoleCache != null)
			{
				EditorGUILayout.Space();

				GUILayout.BeginHorizontal();

				if (GUILayout.Button("Load"))
				{
					m_SmartConsoleCache.Load();
				}

				if (GUILayout.Button("Clear"))
				{
					m_SmartConsoleCache.Clear();
				}

				GUILayout.EndHorizontal();
			}
		}
	}
}