using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
public class DiscordWebHooks : MonoBehaviour
{
    public static DiscordWebHooks i;
    void Awake()
    {
        i = this;
    }
    string webhook_link = "https://discord.com/api/webhooks/1105420139760979998/KoBtf47S_rsIuaDXyOjk3CRfEqaEqxiU1v4afZzg5wvOpFqGz-gfmMukPYK14b990Qwv";

    public void SendDiscordMessage(string message)
    {
        StartCoroutine(SendWebhook(webhook_link, message: message, (success) =>
                {
                    if (success)
                        Debug.Log($"Message Sent With The Content Of : {message}");
                }));
    }
    public IEnumerator SendWebhook(string link, string message, System.Action<bool> action)
    {
        WWWForm form = new WWWForm();
        form.AddField("content", message);

        using (UnityWebRequest www = UnityWebRequest.Post(link, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www.error);
                action(false);
            }
            else
                action(true);
        }
    }
}