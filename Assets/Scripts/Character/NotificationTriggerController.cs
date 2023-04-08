using System.Collections;
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
    public IEnumerator Interact(Transform initiator = null)
    {
        var time = DateTime.Now.ToString("HH:mm:ss");
        Dialog dialog = new Dialog(time);
        yield return DialogManager.instance.ShowDialog((!showTime ? this.dialog : dialog));
    }
}