using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerController : MonoBehaviour
{
    public Dialog dialog;
    public GameObject exclamation, fov;
    public Sprite sprite;
    public string trainerName;
    Character character;
    void Awake()
    {
        this.character = GetComponent<Character>();
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
}
