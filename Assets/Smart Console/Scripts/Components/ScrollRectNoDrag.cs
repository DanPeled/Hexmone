/*
  Package Name: Smart Console
  Version: 2.1.3
  Author: EdgarDev
  Unity Asset Profile: https://assetstore.unity.com/publishers/64126
  Date: 2023-02-12
  Script Name: ScrollRectNoDrag.cs

  Description:
  This script is a ScrollRect that disables dragging.
*/

using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ED.SC.Components
{
    public class ScrollRectNoDrag : ScrollRect
    {
        public override void OnBeginDrag(PointerEventData eventData) { }
        public override void OnDrag(PointerEventData eventData) { }
        public override void OnEndDrag(PointerEventData eventData) { }
	}
}
