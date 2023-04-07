using System;
using UnityEngine;
using System.Collections.Generic;
public class NotificationTriggerController : MonoBehaviour, Interactable
{
    public Dialog dialog;
    public bool showTime;
    void Start()
    {

    }
    void Update()
    {
    }
    public void Interact(){
        var time = DateTime.Now.ToString("HH:mm:ss");
        Dialog dialog = new Dialog(time);
        StartCoroutine(DialogManager.instance.ShowDialog((!showTime ? this.dialog : dialog), null));
    }
}