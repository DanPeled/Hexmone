using UnityEngine;

#pragma warning disable IDE0051

namespace ED.SC.Demo
{
	public class CubeSpawner : MonoBehaviour
	{
		[SerializeField] private Transform[] m_Spots;
		[SerializeField] private GameObject m_CubePrefab;

		private GameObject[] m_Cubes;

		private void Start()
		{
			m_Cubes = new GameObject[m_Spots.Length];
		}

		[Command("spawn_cube", "spawn a cube at available spot", MonoTargetType.Single)]
		private void Spawn()
		{
			int index = -1;

			for (int i = 0; i < m_Cubes.Length && index == -1; i++)
			{
				if (!m_Cubes[i])
				{
					// this spot is available
					index = i;
				}
			}

			if (index == -1)
			{
				SmartConsole.LogError($"Each spot is busy.");

				return;
			}

			GameObject gameObject = Instantiate(m_CubePrefab, m_Spots[index].position, Quaternion.identity);
			m_Cubes[index] = gameObject;

			Debug.Log($"Spawned a cube.");
		}

		[Command("spawn_cube_at_index", "spawn a cube at spot index x", MonoTargetType.Single)]
		private void SpawnAtIndex(int index)
		{
			if (index < 0 || index >= m_Spots.Length)
			{
				SmartConsole.LogError($"Index {index} is out of range.");

				return;
			}

			if (m_Cubes[index])
			{
				SmartConsole.LogError($"Cannot spawn at spot index {index} as it is busy.");

				return;
			}

			GameObject gameObject = Instantiate(m_CubePrefab, m_Spots[index].position, Quaternion.identity);
			m_Cubes[index] = gameObject;

			Debug.Log($"Spawned a cube.");
		}

		[Command("destroy_cube_index", "destroy the cube at spot index", MonoTargetType.Single)]
		private void DestroyAtIndex(int index)
		{
			if (index < 0 || index >= m_Spots.Length)
			{
				SmartConsole.LogError($"Index {index} is out of range.");

				return;
			}

			if (!m_Cubes[index])
			{
				SmartConsole.LogError($"Cannot destroy at spot index {index} as it is empty.");

				return;
			}

			Destroy(m_Cubes[index]);

			Debug.Log($"Destroyed cube.");
		}
	}
}