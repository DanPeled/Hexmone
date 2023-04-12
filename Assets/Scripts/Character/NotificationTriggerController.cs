using System.Collections;
using System;
using UnityEngine;
using System.Collections.Generic;
public class NotificationTriggerController : MonoBehaviour, Interactable
{
    public string dialog;
    public bool showTime;
    void Start()
    {

    }
    void Update()
    {
    }
    public IEnumerator Interact(Transform initiator = null)
    {
        var time = DateTime.Now.ToString("HH:mm:ss");
        yield return DialogManager.instance.ShowDialogText((!showTime ? this.dialog : time));
    }
}