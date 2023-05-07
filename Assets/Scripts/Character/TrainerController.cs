using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerController : MonoBehaviour, Interactable, ISavable
{
    [Header("Dialouge")]
    public string Name;
    public Sprite dialougeSprite;
    public string dialog, dialogAfterBattle;
    [Header("Refrences")]
    public GameObject exclamation, fov;
    public Sprite sprite;
    Character character;
    Vector2 originalPos;
    public AudioClip trainerAppearClip;

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
    public IEnumerator Interact(Transform initiator = null)
    {
        if (CreaturesParty.GetPlayerParty().GetHealthyCreature() == null) { yield break; }
        else if (!battleLost && FindObjectOfType<Player>().GetComponent<CreaturesParty>().GetHealthyCreature() != null)
        {
            yield return (DialogManager.instance.ShowDialogText(dialog));
            AudioManager.i.PlayMusic(trainerAppearClip);
            DialogManager.instance.dialogBox.SetActive(false);
            GameController.instance.StartTrainerBattle(this);
        }
        else
        {
            yield return (DialogManager.instance.ShowDialogText(dialogAfterBattle));
        }
    }
    public IEnumerator TriggerTrainerBattle(Player player)
    {
        if (player.GetComponent<CreaturesParty>().GetHealthyCreature() != null)
        {
            DialogManager.instance.SetTrainerDetails(this);
            yield return (DialogManager.instance.ShowDialogText($"{dialog}", init:transform));
            AudioManager.i.PlayMusic(trainerAppearClip);
            Player.playerActive = false;
            player.isMoving = false;
            // Show exclamation
            exclamation.SetActive(true);
            yield return new WaitForSeconds(0.2f);
            exclamation.SetActive(false);
            // Walk to player
            // var diff = player.transform.position - transform.position;
            // var moveVec = diff - diff.normalized;
            // moveVec = new Vector3(Mathf.Round(moveVec.x), Mathf.Round(moveVec.y));
            // StartCoroutine(character.Move(moveVec));
            //Show dialog
            GameController.instance.StartTrainerBattle(this);
        }
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
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position - new Vector3(4,0,0));
    }
}
