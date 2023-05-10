using System.Net.Mime;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public Player player;
    public static LevelLoader i;
    void Awake()
    {
        i = this;
    }
    void Start()
    {
        EnablePlayer();
    }
    public string transitionID;
    public float loadDelay;
    public EasyTransition.TransitionManager transitionManager;

    public void Load(string _sceneName)
    {
        transitionManager.LoadScene(_sceneName, transitionID, loadDelay);
    }
    public void Load()
    {
        DisablePlayer();
        transitionManager.LoadScene(SceneManager.GetActiveScene().name, transitionID, loadDelay);
    }
    public void DisablePlayer()
    {
        if (player != null)
            player.playerActive = false;
    }
    public IEnumerator EnablePlayer()
    {
        if (player != null)
        {
            yield return new WaitForSeconds(0.4f);
            player.playerActive = true;
        }
    }
}
