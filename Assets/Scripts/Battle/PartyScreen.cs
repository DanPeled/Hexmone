using System.Security.AccessControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PartyScreen : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public PartyMemberUI[] memberSlots;
    List<Creature> creatures;
    int currentMember;
    public Creature SelectedMember => creatures[currentMember];

    public void Init()
    {
        this.memberSlots = GetComponentsInChildren<PartyMemberUI>(true);
    }

    public void SetPartyData(List<Creature> creatures)
    {
        this.creatures = creatures;
        for (int i = 0; i < this.memberSlots.Length; i++)
        {
            if (i < creatures.Count)
            {
                memberSlots[i].gameObject.SetActive(true);
                this.memberSlots[i].SetData(creatures[i]);
            }
            else
            {
                this.memberSlots[i].gameObject.SetActive(false);
            }
        }
        UpdateMemberSelection(currentMember);
    }

    public void UpdateMemberSelection(int selectedMember)
    {
        for (int i = 0; i < creatures.Count; i++)
        {
            if (i == selectedMember)
            {
                memberSlots[i].SetSelected(true);
            }
            else
            {
                memberSlots[i].SetSelected(false);
            }
        }
    }

    public void SetMessageText(object message)
    {
        messageText.text = message.ToString();
    }
    public void HandleUpdate(Action onSelected, Action onBack)
    {
        var prevSelection = currentMember;
        if (InputSystem.instance.right.isClicked())
        {
            currentMember++;
        }
        else if (InputSystem.instance.left.isClicked())
        {
            currentMember--;
        }
        else if (InputSystem.instance.down.isClicked())
        {
            currentMember += 2;
        }
        else if (InputSystem.instance.up.isClicked())
        {
            currentMember -= 2;
        }
        currentMember = Mathf.Clamp(currentMember, 0, creatures.Count - 1);
        if (currentMember != prevSelection)
        UpdateMemberSelection(currentMember);

        if (InputSystem.instance.action.isClicked())
        {
            onSelected?.Invoke();
        }
        else if (InputSystem.instance.back.isClicked())
        {
            onBack?.Invoke();
        }
    }
}
