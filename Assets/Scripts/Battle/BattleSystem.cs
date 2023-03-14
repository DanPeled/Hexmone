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
    Toggle up = new Toggle(), down = new Toggle(), left = new Toggle(), right = new Toggle();

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

        ChooseFirstTurn();

    }
    void ChooseFirstTurn()
    {
        if (playerUnit.creature.Speed >= enemyUnit.creature.Speed)
        {
            StartCoroutine(PlayerAction());
        }
        else
        {
            StartCoroutine(PerformEnemyMove());
        }
    }

    void BattleOver(bool won)
    {
        battleState = BattleState.BattleOver;
        playerParty.creatures.ForEach(p => p.OnBattleOver());
        StartCoroutine(OnBattleOver(won));
    }

    IEnumerator PlayerAction()
    {
        battleState = BattleState.PlayerAction;
        dialogBox.SetDialog("Choose An Action");
        this.actionPossible = true;
        dialogBox.ToggleActionSelector(true);
        yield return null;
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
        if (battleState != BattleState.BattleOver)
        {
            StartCoroutine(PerformEnemyMove());
        }
    }

    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        bool canRunMove = sourceUnit.creature.OnBeforeMove();
        if (!canRunMove)
        {
            yield return ShowStatusChanges(sourceUnit.creature);
            yield return sourceUnit.hud.UpdateHP();
            yield break;
        }
        yield return ShowStatusChanges(sourceUnit.creature);

        int hp = targetUnit.creature.HP;
        move.PP--;
        string usedText = sourceUnit.isPlayerUnit ? "" : "Enemy";
        yield return dialogBox.TypeDialog(
            $"{usedText} {sourceUnit.creature._base.creatureName} Used {move.base_.moveName}",
            dialogBox.dialogText
        );

        if (CheckIfMoveHits(move, sourceUnit.creature, targetUnit.creature))
        {


            sourceUnit.PlayAttackAnimation();
            yield return new WaitForSeconds(1f);
            targetUnit.PlayHitAnimation();

            if (move.base_.category == MoveCategory.Status)
            {
                yield return RunMoveEffects(move.base_.effects, sourceUnit.creature, targetUnit.creature, move.base_.target);
            }
            else
            {
                var damageDetails = targetUnit.creature.TakeDamage(move, sourceUnit.creature);
                yield return ShowDamageDetails(damageDetails);
                yield return targetUnit.hud.UpdateHP();
                yield return playerUnit.hud.UpdateHP();
            }
            if (move.base_.secondaryEffects != null && move.base_.secondaryEffects.Count > 0 &&
             targetUnit.creature.HP > 0)
            {
                foreach (var secondary in move.base_.secondaryEffects)
                {
                    var rnd = UnityEngine.Random.Range(1, 101);

                    if (rnd <= secondary.chance)
                    {
                        yield return RunMoveEffects(secondary, sourceUnit.creature, targetUnit.creature, secondary.target);
                    }
                }
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
        else
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.creature._base.creatureName}'s attack missed", dialogBox.dialogText);
        }
        // Statuses like brn or psn will hurt the creature aftr the turn
        sourceUnit.creature.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.creature);
        yield return sourceUnit.hud.UpdateHP();
        if (sourceUnit.creature.HP <= 0)
        {
            sourceUnit.creature.HP = 0;
            if (!sourceUnit.isPlayerUnit)
            {
                yield return dialogBox.TypeDialog(
                    $"The Enemy {sourceUnit.creature._base.creatureName} Fainted",
                    dialogBox.dialogText
                );
            }
            else
            {
                yield return dialogBox.TypeDialog(
                    $"Your {sourceUnit.creature._base.creatureName} Fainted",
                    dialogBox.dialogText
                );
            }
            targetUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(0.5f);

            CheckForBattleOver(sourceUnit);
        }
    }
    IEnumerator RunMoveEffects(MoveEffects effects, Creature source, Creature target, MoveTarget moveTarget)
    {
        // Stat Boosting
        if (effects.boosts != null)
        {
            if (moveTarget == MoveTarget.Self)
            {
                source.ApplyBoosts(effects.boosts);
            }
            else
            {
                target.ApplyBoosts(effects.boosts);
            }
        }
        // Stat Condition
        if (effects.status != ConditionID.none)
        {
            target.SetStatus(effects.status);
        }
        // Volatile Stat Condition
        if (effects.volatileStatus != ConditionID.none)
        {
            target.SetVolatileStatus(effects.volatileStatus);
        }
        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }
    public bool CheckIfMoveHits(Move move, Creature source, Creature target)
    {
        if (move.base_.alwaysHits)
        {
            return true;
        }
        float moveAcc = move.base_.accuracy;
        int accuracy = source.statBoosts[Stat.Accuracy];
        int evasion = target.statBoosts[Stat.Evasion];

        var boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        if (accuracy > 0)
        {
            moveAcc *= boostValues[accuracy];
        }
        else
        {
            moveAcc /= boostValues[-accuracy];
        }
        if (evasion > 0)
        {
            moveAcc /= boostValues[evasion];
        }
        else
        {
            moveAcc *= boostValues[-evasion];
        }

        return UnityEngine.Random.Range(1, 101) <= moveAcc;
    }
    IEnumerator ShowStatusChanges(Creature creature)
    {
        while (creature.statusChanges.Count > 0)
        {
            var message = creature.statusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message, dialogBox.dialogText);
        }
    }

    IEnumerator OnBattleOver(bool won)
    {
        dialogBox.ToggleActionSelector(false);
        dialogBox.ToggleMoveSelector(false);
        if (won)
        {
            yield return dialogBox.TypeDialog("You Have Won The Battle!", dialogBox.dialogText);
        }
        else
        {
            yield return dialogBox.TypeDialog("You Have Lost The Battle!", dialogBox.dialogText);
        }
        yield return new WaitForSeconds(1f);
        GameObject.FindObjectOfType<Player>().playerActive = true;
        GameObject.FindObjectOfType<Player>().SwitchCamera(0);
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
        GameObject.FindObjectOfType<Player>().playerActive = false;
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
        up.update(Input.GetAxisRaw("Vertical") != 0 && Input.GetAxisRaw("Vertical") > 0);
        down.update(Input.GetAxisRaw("Vertical") != 0 && Input.GetAxisRaw("Vertical") < 0);
        left.update(Input.GetAxisRaw("Horizontal") != 0 && Input.GetAxisRaw("Horizontal") < 0);
        right.update(Input.GetAxisRaw("Horizontal") != 0 && Input.GetAxisRaw("Horizontal") > 0);
    }

    public void HandleActionSelection()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        if (right.isClicked())
        {
            currentAction++;
        }
        else if (left.isClicked())
        {
            currentAction--;
        }
        else if (down.isClicked())
        {
            currentAction += 2;
        }
        else if (up.isClicked())
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
        if (right.isClicked())
        {
            currentMove++;
        }
        else if (left.isClicked())
        {
            currentMove--;
        }
        else if (down.isClicked())
        {
            currentMove += 2;
        }
        else if (up.isClicked())
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
            StartCoroutine(PlayerAction());
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
        if (right.isClicked())
        {
            currentMember++;
        }
        else if (left.isClicked())
        {
            currentMember--;
        }
        else if (down.isClicked())
        {
            currentMember += 2;
        }
        else if (up.isClicked())
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
        bool currentCreatureFainted = true;
        if (playerUnit.creature.HP > 0)
        {
            currentCreatureFainted = false;
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
        if (currentCreatureFainted)
        {
            ChooseFirstTurn();
        }
        else
        {
            StartCoroutine(PerformEnemyMove());
        }
    }
}
