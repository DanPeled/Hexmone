using UnityEngine.SceneManagement;
using UnityEngine;
public static class SceneUtils
{
    public static void ToggleMobileControls(bool state)
    {
        var mobileControlsObj = GameObject.FindObjectOfType<MobileControls>();
        if (mobileControlsObj != null)
        {
            mobileControlsObj.Toggle(state);
        }
        else
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            void OnSceneLoaded(Scene scene, LoadSceneMode mode)
            {
                mobileControlsObj = GameObject.FindObjectOfType<MobileControls>();
                if (mobileControlsObj != null)
                {
                    mobileControlsObj.Toggle(state);
                    SceneManager.sceneLoaded -= OnSceneLoaded;
                }
            }
        }
    }
}
