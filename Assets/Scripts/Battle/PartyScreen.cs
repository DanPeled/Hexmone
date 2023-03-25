using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartyScreen : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public PartyMemberUI[] memberSlots;
    List<Creature> creatures;

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
}
