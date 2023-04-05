using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerController : MonoBehaviour, Interactable, ISavable
{
    public Dialog dialog, dialogAfterBattle;
    public GameObject exclamation, fov;
    public Sprite sprite;
    public string trainerName;
    Character character;
    Vector2 originalPos;

    //State
    public bool battleLost = false;
    void Awake()
    {
        this.character = GetComponent<Character>();
    }
    void Start()
    {
        this.originalPos = transform.position;
    }
    public void Interact()
    {
        if (!battleLost)
        {
            StartCoroutine(DialogManager.instance.ShowDialog(dialog, () =>
            {
                GameController.instance.StartTrainerBattle(this);
            }));
        }
        else
        {
            StartCoroutine(DialogManager.instance.ShowDialog(dialogAfterBattle));
        }
    }
    public IEnumerator TriggerTrainerBattle(Player player)
    {
        player.playerActive = false;
        player.isMoving = false;
        // Show exclamation
        exclamation.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclamation.SetActive(false);
        // Walk to player
        var diff = player.transform.position - transform.position;
        var moveVec = diff - diff.normalized;
        moveVec = new Vector3(Mathf.Round(moveVec.x), Mathf.Round(moveVec.y));
        StartCoroutine(character.Move(moveVec));

        //Show dialog
        Player.instance.ShowDialog();
        StartCoroutine(DialogManager.instance.ShowDialog(dialog, () =>
        {
            GameController.instance.StartTrainerBattle(this);
        }));
    }
    public void BattleLost()
    {
        battleLost = true;
        fov.gameObject.SetActive(false);
        this.transform.position = originalPos;
    }
    public object CaptureState()
    {
        return battleLost;
    }
    public void RestoreState(object state)
    {
        battleLost = (bool)state;

        if (battleLost)
        {
            fov.gameObject.SetActive(false);
        }
    }
}
