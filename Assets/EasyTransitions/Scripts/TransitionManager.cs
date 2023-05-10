using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EasyTransition
{

    public class TransitionManager : MonoBehaviour
    {
        [Tooltip("The Transition Manager Settings store all the premade and your transitions.")]
        public TransitionManagerSettings transitionManagerSettings;

        [SerializeField] private GameObject transitionTemplate;

        bool runningTransition;

        /// <summary>
        /// Loads the new Scene asyncronosly with a transition.
        /// </summary>
        /// <param name="sceneName">The name of the scene you want to load.</param>
        /// <param name="transitionID">The name/ID of the transition you want to use to load you new scene. (Leave empty for no transition)</param>
        /// <param name="loadDelay">The delay before the transition starts.</param>
        public void LoadScene(string sceneName, string transitionID, float loadDelay, Action onStart = null, Action onEnd = null)
        {
            TransitionSettings transitionSettings = null;

            if (runningTransition) return;

            if (transitionID != "")
            {
                onStart?.Invoke();
                for (int i = 0; i < transitionManagerSettings.transitions.Length; i++)
                {
                    if (transitionManagerSettings.transitions[i].transitionID == transitionID)
                    {
                        transitionSettings = transitionManagerSettings.transitions[i];
                        break;
                    }
                }

                if (transitionSettings == null)
                    Debug.LogError("The transitionID: " + transitionID + " is not a valid id.");
            }

            runningTransition = true;
            StartCoroutine(Timer(sceneName, loadDelay, transitionSettings, onEnd));
        }
        public void Load(string transitionID, float loadDelay)
        {
            TransitionSettings transitionSettings = null;

            if (runningTransition) return;

            if (transitionID != "")
            {
                for (int i = 0; i < transitionManagerSettings.transitions.Length; i++)
                {
                    if (transitionManagerSettings.transitions[i].transitionID == transitionID)
                    {
                        transitionSettings = transitionManagerSettings.transitions[i];
                        break;
                    }
                }

                if (transitionSettings == null)
                    Debug.LogError("The transitionID: " + transitionID + " is not a valid id.");
            }

            runningTransition = true;
            StartCoroutine(Timer(loadDelay, transitionSettings));
        }

        /// <summary>
        /// Loads the new Scene asyncronosly with a transition.
        /// </summary>
        /// <param name="sceneName">The name of the scene you want to load.</param>
        /// <param name="transitionID">The name/ID of the transition you want to use to load you new scene. (Leave empty for no transition)</param>
        /// <param name="loadDelay">The delay before the transition starts.</param>
        public void LoadScene(int sceneIndex, string transitionID, float loadDelay)
        {
            TransitionSettings transitionSettings = null;

            if (runningTransition) return;

            if (transitionID != "")
            {
                for (int i = 0; i < transitionManagerSettings.transitions.Length; i++)
                {
                    if (transitionManagerSettings.transitions[i].transitionID == transitionID)
                    {
                        transitionSettings = transitionManagerSettings.transitions[i];
                        break;
                    }
                }

                if (transitionSettings == null)
                    Debug.LogError("The transitionID: " + transitionID + " is not a valid id.");
            }

            runningTransition = true;
            StartCoroutine(Timer(sceneIndex, loadDelay, transitionSettings));
        }

        /// <summary>
        /// Gets the index of a scene from its name.
        /// </summary>
        /// <param name="sceneName">The name of the scene you want to get the index of.</param>
        public int GetSceneIndex(string sceneName)
        {
            return SceneManager.GetSceneByName(sceneName).buildIndex;
        }

        IEnumerator Timer(string sceneName, float time, TransitionSettings transitionSettings, Action onEnd = null)
        {
            Player player = FindObjectOfType<Player>();
            if (player != null)
            {
                player.playerActive = false;
            }
            yield return new WaitForSecondsRealtime(time);

            GameObject template = Instantiate(transitionTemplate) as GameObject;
            template.GetComponent<Transition>().transitionSettings = transitionSettings;
            template.GetComponent<Transition>().fullSettings = transitionManagerSettings;

            float transitionTime = transitionSettings.transitionTime;
            if (transitionSettings.autoAdjustTransitionTime)
                transitionTime = transitionTime / transitionSettings.transitionSpeed;

            yield return new WaitForSecondsRealtime(transitionTime);

            SceneManager.LoadScene(sceneName);
            onEnd?.Invoke();
        }
        IEnumerator Timer(float time, TransitionSettings transitionSettings)
        {
            yield return new WaitForSecondsRealtime(time);

            GameObject template = Instantiate(transitionTemplate) as GameObject;
            template.GetComponent<Transition>().transitionSettings = transitionSettings;
            template.GetComponent<Transition>().fullSettings = transitionManagerSettings;

            float transitionTime = transitionSettings.transitionTime;
            if (transitionSettings.autoAdjustTransitionTime)
                transitionTime = transitionTime / transitionSettings.transitionSpeed;

            yield return new WaitForSecondsRealtime(transitionTime);
        }
        IEnumerator Timer(int sceneIndex, float time, TransitionSettings transitionSettings)
        {
            yield return new WaitForSecondsRealtime(time);

            GameObject template = Instantiate(transitionTemplate) as GameObject;
            template.GetComponent<Transition>().transitionSettings = transitionSettings;
            template.GetComponent<Transition>().fullSettings = transitionManagerSettings;

            float transitionTime = transitionSettings.transitionTime;
            if (transitionSettings.autoAdjustTransitionTime)
                transitionTime = transitionTime / transitionSettings.transitionSpeed;

            yield return new WaitForSecondsRealtime(transitionTime);

            SceneManager.LoadScene(sceneIndex);
        }

        private IEnumerator Start()
        {
            yield return new WaitForSecondsRealtime(1f);

            //Check for multiple instances of the Transition Manager component
            var managerCount = GameObject.FindObjectsOfType<TransitionManager>(true).Length;
            if (managerCount > 1)
                Debug.LogWarning($"There are {managerCount} Transition Managers in your scene. Please ensure there is only one Transition Manager in your scene or overlapping transitions may occur.");

            StartCoroutine("Start");
        }
    }
}
