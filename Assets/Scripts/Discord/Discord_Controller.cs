using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;
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
        discord = new Discord.Discord(applicationID, (System.UInt64)Discord.CreateFlags.NoRequireDiscord);
        time = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();

        UpdateStatus();
    }

    // Update is called once per frame
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
            var activity = new Discord.Activity
            {
                Details = details,
                State = state + $"{playerParty.GetPartyDiscordStatus()}",
                Assets = {
                    LargeImage=largeImage,
                    LargeText = largeText
                },
                Timestamps = {
                    Start = time
                },
            };
            activityManager.UpdateActivity(activity, (res) =>
            {
                if (res != Discord.Result.Ok)
                {
                    Debug.LogWarning("Failed connecting to discord!");
                }
            });
        }
        catch
        {
            gameObject.SetActive(false);
        }
    }
    public void Disconnect()
    {
    }
}
