using System;
using System.IO.Pipes;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
public class MainMenuSelect : MonoBehaviour
{
    public int currentAction = 0;
    public LevelLoader levelLoader;
    public GameObject[] buttons;
    public bool isMobile;
    public GameObject cover;
    void Start()
    {
        cover.SetActive(false);
    }
    public void SetMobile()
    {
        MobileControls.isMobilePersisted = !isMobile;
        isMobile = MobileControls.isMobilePersisted;
    }
    void Update()
    {

        // Action change
        if (InputSystem.instance.down.isClicked())
        {
            currentAction++;
        }
        else if (InputSystem.instance.up.isClicked())
        {
            currentAction--;
        }
        currentAction = Mathf.Clamp(currentAction, 0, buttons.Length - 1);

        // action handeling
        if (InputSystem.instance.action.isClicked())
        {
            switch (currentAction)
            {
                case 0:
                    // Play
                    Play();
                    break;
                case 1:
                    // World Editor
                    WorldEditor();
                    break;
                case 2:
                    // Settings
                    break;
                case 3:
                    Quit();
                    break;
            }
        }
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].GetComponentInChildren<TextMeshProUGUI>().color = i == currentAction ? GlobalSettings.i.highlightedColor : Color.black;
        }
    }

    public void Play()
    {
        cover.gameObject.SetActive(true);
        SceneManager.LoadScene("world");
    }
    public void WorldEditor(){
        cover.gameObject.SetActive(true);
        SceneManager.LoadScene("WorldEditor");
    }
    public void Settings()
    {
        // todo: implement settings
    }
    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

}