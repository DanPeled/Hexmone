using UnityEngine;

namespace EasyTransition
{

    public class DemoLoadScene : MonoBehaviour
    {
        public string transitionID;
        public float loadDelay;
        public EasyTransition.TransitionManager transitionManager;

        public void LoadScene(string _sceneName)
        {
            transitionManager.LoadScene(_sceneName, transitionID, loadDelay);
        }
    }

}

