using System.Net.Mime;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1.5f;
    public static string lastScene;
    public GameObject loadingScreen;
    public static LevelLoader i;
    public void Load(Room targetRoom=null)
    {
        transform.GetChild(0).gameObject.SetActive(true);
        StartCoroutine(LoadAnim("Transition", targetRoom));
    }
    public IEnumerator LoadAnim(string scene, Room targetRoom)
    {
        GameObject.FindObjectOfType<Player>().playerActive = false;

        string currentScene = SceneManager.GetActiveScene().name;
        foreach (var levelLoader in GameObject.FindObjectsOfType<LevelLoader>())
        {
            levelLoader.GetComponentInChildren<Image>().enabled = true;
            levelLoader.GetComponentInChildren<Animator>().Play("Crossfade_Start", 0, 0);
        }
        Player player = GameObject.FindObjectOfType<Player>();
        player.playerActive = false;
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(scene, LoadSceneMode.Additive);
        if (targetRoom != null)
            player.transform.position = targetRoom.roomPosition;

        foreach (var levelLoader in GameObject.FindObjectsOfType<LevelLoader>())
        {
            levelLoader.GetComponentInChildren<Image>().enabled = false;
        }
        SceneManager.UnloadSceneAsync(scene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
        GameObject.FindObjectOfType<Player>().playerActive = true;
    }
    public IEnumerator LoadAnim(string scene)
    {
        string currentScene = SceneManager.GetActiveScene().name;
        foreach (var levelLoader in GameObject.FindObjectsOfType<LevelLoader>())
        {
            levelLoader.GetComponentInChildren<Image>().enabled = true;
            levelLoader.GetComponentInChildren<Animator>().Play("Crossfade_Start", 0, 0);
        }
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(scene, LoadSceneMode.Additive);

        foreach (var levelLoader in GameObject.FindObjectsOfType<LevelLoader>())
        {
            levelLoader.GetComponentInChildren<Image>().enabled = false;
        }
        SceneManager.UnloadSceneAsync(scene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
    }
    public IEnumerator LoadAnim(float transitionTime)
    {
        foreach (var levelLoader in GameObject.FindObjectsOfType<LevelLoader>())
        {
            levelLoader.GetComponentInChildren<Image>().enabled = true;
            levelLoader.GetComponentInChildren<Animator>().Play("Crossfade_Start", 0, 0);
        }
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);

        foreach (var levelLoader in GameObject.FindObjectsOfType<LevelLoader>())
        {
            levelLoader.GetComponentInChildren<Image>().enabled = false;
        }
        GameObject.FindObjectOfType<Player>().playerActive = true;
    }

    IEnumerator LoadAsync(string scene)
    {
        string currentScene = SceneManager.GetActiveScene().name;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            loadingScreen.SetActive(true);
            yield return null;
        }

        if (asyncLoad.isDone)
        {
            loadingScreen.SetActive(false);
            SceneManager.UnloadSceneAsync(currentScene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
        }
    }
    public void Load(string scene)
    {
        StartCoroutine(LoadAsync(scene));
    }
    void Update()
    {
        i = this;
    }
}
