using System.Net.Mime;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1.5f;

    public void Load(Room targetRoom)
    {
        StartCoroutine(LoadAnim("Transition", targetRoom));
    }

    public IEnumerator LoadAnim(string scene, Room targetRoom)
    {
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
        player.transform.position = targetRoom.roomPosition;

        foreach (var levelLoader in GameObject.FindObjectsOfType<LevelLoader>())
        {
            levelLoader.GetComponentInChildren<Image>().enabled = false;
        }
        SceneManager.UnloadSceneAsync(scene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
        GameObject.FindObjectOfType<Player>().playerActive = true;
    }
}
