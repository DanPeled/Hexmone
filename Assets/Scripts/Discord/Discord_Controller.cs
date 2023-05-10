using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;
using UnityEngine.SceneManagement;
public static class Scenes
{
    public static readonly string World = "World";
    public static readonly string Menu = "MainMenu";
}
public class Discord_Controller : MonoBehaviour
{
    public long applicationID;

    [Space]
    public string details = "Exploring...";
    public string state = "Party : ";

    [Space]
    public string largeImage = "game_logo";
    public string largeText = "Hexmone";

    private long time;
    private static bool instanceExists;
    public Discord.Discord discord;

    public CreaturesParty playerParty;

    // Start is called before the first frame update
    void Awake()
    {
        gameObject.SetActive(true);

        playerParty = CreaturesParty.GetPlayerParty();
        if (!instanceExists)
        {
            instanceExists = true;
            DontDestroyOnLoad(gameObject);
        }
        else if (FindObjectsOfType(GetType()).Length > 1)
        {
            (gameObject).SetActive(false);
        }
    }

    void Start()
    {
        ConnectToDiscord();
    }

    public void ConnectToDiscord()
    {
        gameObject.SetActive(true);
        discord = new Discord.Discord(
            applicationID,
            (System.UInt64)Discord.CreateFlags.NoRequireDiscord
        );
        time = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();

        UpdateStatus();
    }

    void Update()
    {
        try
        {
            discord.RunCallbacks();
        }
        catch
        {
            (gameObject).SetActive(false);
        }
    }

    void LateUpdate()
    {
        UpdateStatus();
    }

    void UpdateStatus()
    {
        try
        {
            var activityManager = discord.GetActivityManager();
            string status = "";
            if (SceneManager.GetActiveScene().name == Scenes.World)
            {
                status = $"{playerParty.GetPartyDiscordStatus()}";
                state = "Party: ";
            }
            else if (SceneManager.GetActiveScene().name == Scenes.Menu)
            {
                status = $"In main menu";
                state = "";
            }
            var activity = new Discord.Activity
            {
                Details = details,
                State = state + status,
                Assets = { LargeImage = largeImage, LargeText = largeText },
                Timestamps = { Start = time },
            };
            activityManager.UpdateActivity(
                activity,
                (res) =>
                {
                    if (res != Discord.Result.Ok)
                    {
                        Debug.LogWarning("Failed connecting to discord!");
                    }
                }
            );
        }
        catch
        {
            gameObject.SetActive(false);
        }
    }


    public void Disconnect()
    {
        if (discord == null)
        {
            Debug.LogWarning("Discord SDK not initialized. Disconnect aborted.");
            return;
        }

        var activityManager = discord.GetActivityManager();
        if (activityManager == null)
        {
            Debug.LogWarning("Activity manager not found. Disconnect aborted.");
            return;
        }

        activityManager.ClearActivity((result) =>
        {
            if (result == Discord.Result.Ok)
            {
                discord.Dispose();
                Debug.Log("Disconnected from Discord.");
            }
            else
            {
                Debug.LogWarning("Failed to clear Discord activity. Error code: " + result);
            }
        });
    }
}
