using System;
using System.Collections;
using UnityEngine;
using TMPro;

public enum BattleState
{
    Start,
    PlayerAction,
    PlayerMove,
    EnemyMove,
    Busy,
    PartyScreen,
    BattleOver
}

public class BattleSystem : MonoBehaviour
{
    public BattleUnit playerUnit,
        enemyUnit;

    public BattleDialogBox dialogBox;
    public PartyScreen partyScreen;
    public BattleState battleState;
    public int currentAction,
        currentMove,
        currentMember;
    bool actionPossible = false;
    CreaturesParty playerParty;
    Creature wildCreature;

    public void StartBattle(CreaturesParty creaturesParty, Creature wildCreature)
    {
        this.playerParty = creaturesParty;
        this.wildCreature = wildCreature;
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        GameObject.FindObjectOfType<Player>().playerActive = false;
        playerUnit.Setup(playerParty.GetHealthyCreature());
        enemyUnit.Setup(wildCreature);
        partyScreen.Init();
        dialogBox.SetMoveNames(playerUnit.creature.moves);

        yield return dialogBox.TypeDialog(
            $"A Wild {enemyUnit.creature._base.creatureName} Appeared",
            dialogBox.dialogText
        );

        StartCoroutine(PlayerAction());
    }

    void BattleOver(bool won)
    {
        battleState = BattleState.BattleOver;
        StartCoroutine(OnBattleOver(won));
    }

    IEnumerator PlayerAction()
    {
        battleState = BattleState.PlayerAction;
        StartCoroutine(dialogBox.TypeDialog("Choose An Action", dialogBox.dialogText));
        yield return new WaitForSeconds(0.53f);
        this.actionPossible = true;
        dialogBox.ToggleActionSelector(true);
    }

    public void OpenPartyScreen()
    {
        battleState = BattleState.PartyScreen;
        partyScreen.SetPartyData(playerParty.creatures);
        partyScreen.gameObject.SetActive(true);
        partyScreen.SetMessageText("Choose A Creature");
        battleState = BattleState.PartyScreen;
    }

    void MoveSelection()
    {
        battleState = BattleState.PlayerMove;
        dialogBox.ToggleActionSelector(false);
        dialogBox.ToggleDialogText(false);
        dialogBox.ToggleMoveSelector(true);
    }

    IEnumerator PerformPlayerMove()
    {
        battleState = BattleState.Busy;
        var move = playerUnit.creature.moves[currentMove];
        yield return RunMove(playerUnit, enemyUnit, move);

        // If the battle stat was not changed by RunMove, then go to next step
        StartCoroutine(PerformEnemyMove());
    }

    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        move.PP--;
        yield return dialogBox.TypeDialog(
            $"{sourceUnit.creature._base.creatureName} Used {move.base_.moveName}",
            dialogBox.dialogText
        );
        sourceUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);
        targetUnit.PlayHitAnimation();

        if (move.base_.category == MoveCategory.Status)
        {
            if (move.base_.effects.boosts != null)
            {
                if (move.base_.target == MoveTarget.Self)
                {
                    sourceUnit.creature.ApplyBoosts(move.base_.effects.boosts);
                }
                else
                {
                    targetUnit.creature.ApplyBoosts(move.base_.effects.boosts);
                }
            }
        }
        else
        {
            var damageDetails = targetUnit.creature.TakeDamage(move, sourceUnit.creature);
            yield return targetUnit.hud.UpdateHP();
            yield return ShowDamageDetails(damageDetails);
        }
        if (targetUnit.creature.HP <= 0)
        {
            targetUnit.creature.HP = 0;
            if (!targetUnit.isPlayerUnit)
            {
                yield return dialogBox.TypeDialog(
                    $"The Enemy {targetUnit.creature._base.creatureName} Fainted",
                    dialogBox.dialogText
                );
            }
            else
            {
                yield return dialogBox.TypeDialog(
                    $"Your {targetUnit.creature._base.creatureName} Fainted",
                    dialogBox.dialogText
                );
            }
            targetUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(0.5f);

            CheckForBattleOver(targetUnit);
        }
    }

    IEnumerator OnBattleOver(bool won)
    {
        if (won)
        {
            dialogBox.dialogText.text = "You Have Won The Battle!";
        }
        else
        {
            dialogBox.dialogText.text = "You Have Lost The Battle!";
        }
        yield return new WaitForSeconds(2);
        GameObject.FindObjectOfType<Player>().playerActive = true;
        StopCoroutine(SetupBattle());
        gameObject.SetActive(false);
    }

    void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.isPlayerUnit)
        {
            var nextCreature = playerParty.GetHealthyCreature();
            if (nextCreature != null)
            {
                battleState = BattleState.PartyScreen;
                OpenPartyScreen();
            }
            else
            {
                BattleOver(false);
            }
        }
        else
        {
            BattleOver(true);
        }
    }

    IEnumerator PerformEnemyMove()
    {
        battleState = BattleState.EnemyMove;
        var move = enemyUnit.creature.GetRandomMove();
        yield return RunMove(enemyUnit, playerUnit, move);
        if (battleState != BattleState.PartyScreen)
            StartCoroutine(PlayerAction());
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
        {
            yield return dialogBox.TypeDialog("A Critical Hit", dialogBox.dialogText);
        }
        if (damageDetails.TypeEffectiveness > 1f)
        {
            yield return dialogBox.TypeDialog("It's Super Effective!", dialogBox.dialogText);
        }
        if (damageDetails.TypeEffectiveness < 1f)
        {
            yield return dialogBox.TypeDialog("It's Not Very Effective", dialogBox.dialogText);
        }
    }

    void Update()
    {
        switch (battleState)
        {
            case BattleState.PlayerAction:
                HandleActionSelection();
                break;
            case BattleState.PlayerMove:
                HandleMoveSelection();
                break;
            case BattleState.PartyScreen:
                HandlePartySelection();
                break;
        }
    }

    public void HandleActionSelection()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentAction++;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentAction--;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentAction += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentAction -= 2;
        }

        currentAction = Mathf.Clamp(currentAction, 0, 3);
        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetButtonDown("Action") && this.actionPossible)
        {
            if (currentAction == 0)
            {
                // Fight
                MoveSelection();
            }
            else if (currentAction == 1)
            {
                // Bag
            }
            else if (currentAction == 2)
            {
                // Creature
                OpenPartyScreen();
                battleState = BattleState.PartyScreen;
            }
            else if (currentAction == 3)
            {
                // Run
            }
        }
    }

    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentMove++;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentMove--;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentMove += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentMove -= 2;
        }

        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.creature.moves.Count - 1);
        dialogBox.UpdateMoveSelection(currentMove, playerUnit.creature.moves[currentMove]);
        if (Input.GetButtonDown("Action"))
        {
            dialogBox.ToggleMoveSelector(false);
            dialogBox.ToggleDialogText(true);
            StartCoroutine(PerformPlayerMove());
        }
        else if (Input.GetButtonDown("Back"))
        {
            dialogBox.ToggleMoveSelector(false);
            dialogBox.ToggleDialogText(true);
            dialogBox.ToggleActionSelector(true);
            battleState = BattleState.PlayerAction;
            PlayerAction();
        }
    }

    public IEnumerator FinishBattle(bool playerIsWinner)
    {
        string winningText = playerIsWinner
            ? "Player"
            : $"Enemy {enemyUnit.creature._base.creatureName}";
        yield return dialogBox.TypeDialog($"{winningText} Has Won", dialogBox.dialogText);
        gameObject.SetActive(false);
    }

    public void HandlePartySelection()
    {
        dialogBox.ToggleActionSelector(false);
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentMember++;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentMember--;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentMember += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentMember -= 2;
        }
        currentMember = Mathf.Clamp(currentMember, 0, playerParty.creatures.Count - 1);

        partyScreen.UpdateMemberSelection(currentMember);

        if (Input.GetButtonDown("Action"))
        {
            dialogBox.ToggleActionSelector(false);
            dialogBox.ToggleMoveSelector(false);
            var selectedMember = playerParty.creatures[currentMember];
            if (selectedMember.HP <= 0)
            {
                partyScreen.SetMessageText("You can't send out a fainted creature");
                return;
            }
            if (selectedMember == playerUnit.creature)
            {
                partyScreen.SetMessageText("You can't switch with the same creature");
                return;
            }

            partyScreen.gameObject.SetActive(false);
            battleState = BattleState.Busy;
            StartCoroutine(SwitchCreature(selectedMember));
        }
        else if (Input.GetButtonDown("Back"))
        {
            partyScreen.gameObject.SetActive(false);

            StartCoroutine(PlayerAction());
        }
    }

    public IEnumerator SwitchCreature(Creature newCreature)
    {
        if (playerUnit.creature.HP > 0)
        {
            yield return dialogBox.TypeDialog(
                $"Come back {playerUnit.creature._base.creatureName}",
                dialogBox.dialogText
            );
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
        }
        playerUnit.Setup(newCreature);

        dialogBox.SetMoveNames(newCreature.moves);

        yield return dialogBox.TypeDialog(
            $"Go {newCreature._base.creatureName}!",
            dialogBox.dialogText
        );
        playerUnit.image.color = playerUnit.originalColor;

        StartCoroutine(PerformEnemyMove());
    }
}
