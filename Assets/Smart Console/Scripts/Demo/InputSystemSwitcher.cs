using UnityEngine;
using UnityEngine.EventSystems;

namespace ED.SC.Demo
{
	public class InputSystemSwitcher : MonoBehaviour
	{
		private void Awake()
		{
#if ENABLE_LEGACY_INPUT_MANAGER

#if !UNITY_INPUT_SYSTEM_ENABLE_UI
	var inputModule = gameObject.GetComponent<BaseInputModule>();
	Destroy(inputModule);
#endif

	gameObject.AddComponent<StandaloneInputModule>();
	Destroy(this);
#endif
		}
	}
}