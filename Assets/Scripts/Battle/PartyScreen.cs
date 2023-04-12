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
    CreaturesParty party;
    /// <summary>
    /// Party screen can be called from diffrent states
    /// </summary>
    public BattleState? CalledFrom { get; set; }

    public void Init()
    {
        party = CreaturesParty.GetPlayerParty();
        this.memberSlots = GetComponentsInChildren<PartyMemberUI>(true);
        SetPartyData();

        party.onUpdated += SetPartyData;
    }

    public void SetPartyData()
    {
        this.creatures = party.Creatures;
        for (int i = 0; i < this.memberSlots.Length; i++)
        {
            if (i < creatures.Count)
            {
                memberSlots[i].gameObject.SetActive(true);
                this.memberSlots[i].Init(creatures[i]);
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
        if (InputSystem.right.isClicked())
        {
            currentMember++;
        }
        else if (InputSystem.left.isClicked())
        {
            currentMember--;
        }
        else if (InputSystem.down.isClicked())
        {
            currentMember += 2;
        }
        else if (InputSystem.up.isClicked())
        {
            currentMember -= 2;
        }
        currentMember = Mathf.Clamp(currentMember, 0, creatures.Count - 1);
        if (currentMember != prevSelection)
            UpdateMemberSelection(currentMember);

        if (InputSystem.action.isClicked())
        {
            onSelected?.Invoke();
        }
        else if (InputSystem.back.isClicked())
        {
            onBack?.Invoke();
        }
    }
    public void ShowIfTmIsUsable(TMItem item){
        for(int i = 0; i < creatures.Count; i++){
            string msg = item.CanBeTaught(creatures[i]) ? "ABLE" :"NOT ABLE";
            memberSlots[i].SetMessage(msg);
        }
    }
    public void ClearMemberSlotMessages()
    {
        for (int i = 0; i < creatures.Count; i++)
        {
            string msg = "";
            memberSlots[i].SetMessage(msg);
        }
    }
}
