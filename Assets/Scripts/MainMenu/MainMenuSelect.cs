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
    public GameObject playMenu,
        normalMenu;

    public void SetMobile()
    {
        MobileControls.isMobilePersisted = !isMobile;
        isMobile = MobileControls.isMobilePersisted;
    }

    void Update()
    {
        // Action change
        if (InputSystem.down.isClicked())
        {
            currentAction++;
        }
        else if (InputSystem.up.isClicked())
        {
            currentAction--;
        }
        currentAction = Mathf.Clamp(currentAction, 0, buttons.Length - 1);

        // action handeling
        if (InputSystem.action.isClicked())
        {
            switch (currentAction)
            {
                case 0:
                    // Play
                    PlayMenu();
                    break;
                case 1:
                    Settings();
                    break;
                case 2:
                    Quit();
                    break;
            }
        }
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].GetComponentInChildren<TextMeshProUGUI>().color =
                i == currentAction ? GlobalSettings.i.highlightedColor : Color.black;
        }
    }

    public void PlayMenu()
    {
        // playMenu.SetActive(true);
        // normalMenu.SetActive(false);
        Play();
    }

    public void Play()
    {
        levelLoader.Load("World");
    }

    public void WorldEditor()
    {
        SceneManager.LoadScene("WorldEditor");
    }

    public void Settings()
    {
        // TODO: implement settings function
    }

    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
