using System.Collections.Generic;
using UnityEngine;

public class DebugController : MonoBehaviour
{
    public KeyCode consoleKeyCode;
    bool showConsole,
        showHelp;
    string input;

    public static DebugCommand SAVE;
    public static DebugCommand HELP;

    public List<object> commandList;

    private void Awake()
    {
        SAVE = new(
            "save",
            "Saves the game",
            "save",
            () =>
            {
                SavingSystem.i.Save("saveSlot1");
            }
        );
        HELP = new(
            "help",
            "Shows a list of commands",
            "help",
            () =>
            {
                showHelp = true;
            }
        );

        commandList = new() { SAVE, HELP };
    }

    public void OnReturn()
    {
        if (showConsole)
        {
            HandleInput();
            input = "";
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(consoleKeyCode))
        {
            OnToggleDebug();
        }
        else if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            OnReturn();
        }
    }

    public void OnToggleDebug()
    {
        showConsole = !showConsole;
    }

    Vector2 scroll;

    private void OnGUI()
    {
        if (!showConsole)
        {
            return;
        }
        float y = 0f;
        if (showHelp)
        {
            GUI.Box(new Rect(0, y, Screen.width, 100), "");

            Rect viewport = new Rect(0, 0, Screen.width - 30, 20 * commandList.Count);

            scroll = GUI.BeginScrollView(new Rect(0, y + 5f, Screen.width, 90), scroll, viewport);

            for (int i = 0; i < commandList.Count; i++)
            {
                DebugCommandBase command = commandList[i] as DebugCommandBase;

                string label = $"{command.CommandFormat} - {command.CommandDescription}";

                Rect labelRect = new Rect(5, 20 * i, viewport.width - 100, 20);

                GUI.Label(labelRect, label);
            }
            GUI.EndScrollView();

            y += 100;
        }

        GUI.Box(new Rect(0, y, Screen.width, 30), "");
        GUI.backgroundColor = new Color(0, 0, 0, 0);
        input = GUI.TextField(new Rect(10f, y + 5f, Screen.width - 20f, 20f), input);
    }

    public void HandleInput()
    {
        string[] properties = input.Split(" ");
        for (int i = 0; i < commandList.Count; i++)
        {
            DebugCommandBase cmdBase = commandList[i] as DebugCommandBase;

            if (input.Contains(cmdBase.CommandId))
            {
                if (commandList[i] as DebugCommand != null)
                {
                    (commandList[i] as DebugCommand).Invoke();
                }
                else if (commandList[i] as DebugCommand<int> != null)
                {
                    (commandList[i] as DebugCommand<int>).Invoke(int.Parse(properties[1]));
                }
            }
        }
    }
}
