/*
  Package Name: Smart Console
  Version: 2.1.3
  Author: EdgarDev
  Unity Asset Profile: https://assetstore.unity.com/publishers/64126
  Date: 2023-03-05
  Script Name: UIDragger.cs

  Description:
  This script is ensures the movement of a RectTransform throught a RectTransform's drag.
*/

using UnityEngine;
using UnityEngine.EventSystems;

namespace ED.SC
{
	public class UIDragger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler
	{
		[SerializeField] private Canvas m_Canvas;
		[SerializeField] private RectTransform m_DragRectTransform;
		[SerializeField] private Texture2D m_HoverCursor;

		private Vector2 m_CursorHotspot;

		private void Start()
		{
			m_CursorHotspot = new Vector2(m_HoverCursor.width / 2, m_HoverCursor.height / 2);
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			Cursor.SetCursor(m_HoverCursor, m_CursorHotspot, CursorMode.Auto);
		}

		public void OnDrag(PointerEventData eventData)
		{
			m_DragRectTransform.anchoredPosition += eventData.delta / m_Canvas.scaleFactor;
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		}
	}
}