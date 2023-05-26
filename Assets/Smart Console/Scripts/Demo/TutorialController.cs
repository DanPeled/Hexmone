using ED.SC.Components;
using System.Collections;
using UnityEngine;

namespace ED.SC.Demo
{
	public class TutorialController : MonoBehaviour
	{
		[SerializeField] private ConsoleSystem m_ConsoleSystem;

		private bool m_IsRunningTutorial;

		private void Start()
		{
			SmartConsole.Log("Welcome to Smart Console!\nA flexible and intuitive in-game command console that will elevate your game development experience.");
			SmartConsole.Log("Enter the command <b>start_tutorial</b> to start the tutorial.");
		}

		[Command("start_tutorial", "start the tutorial", MonoTargetType.Single)]
		private void StartTutorial()
		{
			StopAllCoroutines();

			m_ConsoleSystem.OnActivate -= TutorialEnterCommandDispatcher;
			SmartConsole.LogMessageReceived -= TutorialSpawnCubeDispatcher;
			Application.logMessageReceived -= TutorialSpinCubeDispatcher;
			Application.logMessageReceived -= TutorialDestroyCubeDispatcher;
			Application.logMessageReceived -= TutorialEndDispatcher;

			StartCoroutine(TutorialToggleCoroutine());

			m_IsRunningTutorial = true;
		}

		[Command("stop_tutorial", "stop the tutorial", MonoTargetType.Single)]
		private void StopTutorial()
		{
			if (!m_IsRunningTutorial)
			{
				SmartConsole.LogWarning("Tutorial is not running.\nEnter the command <b>start_tutorial</b> to start the tutorial.");
				return;
			}

			StopAllCoroutines();

			m_ConsoleSystem.OnActivate -= TutorialEnterCommandDispatcher;
			SmartConsole.LogMessageReceived -= TutorialSpawnCubeDispatcher;
			Application.logMessageReceived -= TutorialSpinCubeDispatcher;
			Application.logMessageReceived -= TutorialDestroyCubeDispatcher;
			Application.logMessageReceived -= TutorialEndDispatcher;

			SmartConsole.Log("Tutorial has been stopped. You are free to try-out the 37 included commands.\n\nNote that you will be able to create your own commands using this tool.");

			m_IsRunningTutorial = false;
		}

		private IEnumerator TutorialToggleCoroutine()
		{
			SmartConsole.Log("Welcome to the tutorial of the Smart Console!\nPlease follow the instructions to move forward in the experience.");

			yield return new WaitForSeconds(3.0f);
			SmartConsole.Log("Try to toggle the console using the <b>input key Escape</b>.");

			m_ConsoleSystem.OnActivate += TutorialEnterCommandDispatcher;
		}

		private void TutorialEnterCommandDispatcher()
		{
			m_ConsoleSystem.OnActivate -= TutorialEnterCommandDispatcher;

			StartCoroutine(TutorialEnterCommandCoroutine());
		}

		private IEnumerator TutorialEnterCommandCoroutine()
		{
			yield return new WaitForSeconds(1.0f);
			SmartConsole.Log("Bravo! You successfully toggled the console.");

			yield return new WaitForSeconds(2.0f);
			SmartConsole.Log("Now try to use the command <b>get_commands</b> to print all the available commands.");

			SmartConsole.LogMessageReceived += TutorialSpawnCubeDispatcher;
		}

		private void TutorialSpawnCubeDispatcher(LogMessage logMessage)
		{
			if (logMessage.Type != LogMessageType.Command || !string.Equals(logMessage.Text, "get_commands"))
			{
				return;
			}

			SmartConsole.LogMessageReceived -= TutorialSpawnCubeDispatcher;

			StartCoroutine(TutorialSpawnCubeCoroutine());
		}

		private IEnumerator TutorialSpawnCubeCoroutine()
		{
			yield return new WaitForSeconds(2.0f);
			SmartConsole.Log("You successfully printed all the available command, printed just above.");

			yield return new WaitForSeconds(2.0f);
			SmartConsole.Log("Try to spawn a cube using the command <b>spawn_cube</b>, you can spawn up to 3 cubes");

			Application.logMessageReceived += TutorialSpinCubeDispatcher;
		}

		private void TutorialSpinCubeDispatcher(string log, string stackTrace, LogType type)
		{
			if (type != LogType.Log || !string.Equals(log, "Spawned a cube."))
			{
				return;
			}

			Application.logMessageReceived -= TutorialSpinCubeDispatcher;

			StartCoroutine(TutorialSpinCubeCoroutine());
		}

		private IEnumerator TutorialSpinCubeCoroutine()
		{
			yield return new WaitForSeconds(1.0f);
			SmartConsole.Log("You successfully spawned a cube.");

			yield return new WaitForSeconds(2.0f);
			SmartConsole.Log("Now try to spin it using the command <b>spin_last_cube</b>, don't forget to add a <i>spinCount</i> parameter.");

			Application.logMessageReceived += TutorialDestroyCubeDispatcher;
		}

		private void TutorialDestroyCubeDispatcher(string log, string stackTrace, LogType type)
		{
			if (type != LogType.Log || !string.Equals(log, "Completed spin(s)"))
			{
				return;
			}

			Application.logMessageReceived -= TutorialDestroyCubeDispatcher;

			StartCoroutine(TutorialDestroyCubeCoroutine());
		}

		private IEnumerator TutorialDestroyCubeCoroutine()
		{
			yield return new WaitForSeconds(1.0f);
			SmartConsole.Log("Yes! You successfully spinned the cube. You can also spin all cubes using the command <b>spin_all_cube</b>");

			yield return new WaitForSeconds(2.0f);
			SmartConsole.Log("To end, try to destroy the cube index 0 using the command <b>destroy_cube_index</b>, don't forget to add the cube index to destroy!");

			Application.logMessageReceived += TutorialEndDispatcher;
		}

		private void TutorialEndDispatcher(string log, string stackTrace, LogType type)
		{
			if (type != LogType.Log || !string.Equals(log, "Destroyed cube."))
			{
				return;
			}

			Application.logMessageReceived -= TutorialEndDispatcher;

			StartCoroutine(TutorialEndCoroutine());
		}

		private IEnumerator TutorialEndCoroutine()
		{
			yield return new WaitForSeconds(1.0f);
			SmartConsole.Log("Congratulation! You have finished this demo of Smart Console.\nYou are free to try-out the 37 included commands.\n\nNote that you will be able to create your own commands using this tool.");
		}
	}
}