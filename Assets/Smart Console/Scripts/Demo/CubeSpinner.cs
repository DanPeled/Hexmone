using System.Collections;
using UnityEngine;

#pragma warning disable IDE0051

namespace ED.SC.Demo
{
	public class CubeSpinner : MonoBehaviour
	{
		[SerializeField] private float m_SpinInterpolation;

		private void Awake()
		{
			SmartConsole.AddAutocomplete(this);
		}

		[Command("spin_all_cube", "spin all cube x time", MonoTargetType.All)]
		private IEnumerator Spin(int spinCount)
		{
			for (int i = 0; i < spinCount; i++)
			{
				Vector3 startRotation = transform.eulerAngles;
				Vector3 endRotation = new Vector3(startRotation.x, startRotation.y + 180f, startRotation.z);

				float startTime = Time.time;

				while (Time.time < startTime + m_SpinInterpolation)
				{
					float delta = (Time.time - startTime) / m_SpinInterpolation;

					Vector3 interpolationValue = Vector3.Lerp(startRotation, endRotation, delta);
					transform.rotation = Quaternion.Euler(interpolationValue);

					yield return null;
				}

				transform.rotation = Quaternion.Euler(endRotation);

				yield return null;
			}

			Debug.Log($"Completed spin(s)");
		}

		[Command("spin_last_cube", "spin the last spawned cube x time", MonoTargetType.Single)]
		private void SpinLast(int spinCount)
		{
			StartCoroutine(Spin(spinCount));
		}

		private void OnDestroy()
		{
			SmartConsole.RemoveAutocomplete(this);
		}
	}
}