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
    Toggle up = new Toggle(), down = new Toggle();

    public GameObject[] buttons;

    void Update()
    {
        // Limits the input axis to a click
        up.update(Input.GetAxisRaw("Vertical") > 0);
        down.update(Input.GetAxisRaw("Vertical") < 0);


        // Action change
        if (down.isClicked())
        {
            currentAction++;
        }
        else if (up.isClicked())
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
                    // Settings
                    break;
                case 2:
                    // Quit
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
        SceneManager.LoadScene("world");
    }
    public void Settings()
    {

    }
    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

}