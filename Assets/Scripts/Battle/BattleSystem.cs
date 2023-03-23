using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum BattleState
{
    Start,
    ActionSelection,
    MoveSelection,
    RunningTurn,
    Busy,
    PartyScreen,
    BattleOver
}

public enum BattleAction
{
    Move,
    SwitchCreature,
    UseItem,
    Run
}

public class BattleSystem : MonoBehaviour
{
    public BattleUnit playerUnit,
        enemyUnit;

    public BattleDialogBox dialogBox;
    public PartyScreen partyScreen;
    public BattleState? battleState,
        prevState;
    public int currentAction,
        currentMove,
        currentMember;
    bool actionPossible = false;
    public Image playerImage, trainerImage;
    CreaturesParty playerParty, trainerParty;
    Creature wildCreature;
    Toggle up = new Toggle(),
        down = new Toggle(),
        left = new Toggle(),
        right = new Toggle();
    bool isTrainerBattle = false;
    Player player;
    TrainerController trainer;
    public void StartBattle(CreaturesParty playerParty, Creature wildCreature)
    {
        this.playerParty = playerParty;
        this.wildCreature = wildCreature;
        StartCoroutine(SetupBattle());
    }
    public void StartTrainerBattle(CreaturesParty playerParty, CreaturesParty trainerParty)
    {
        this.playerParty = playerParty;
        this.trainerParty = trainerParty;
        isTrainerBattle = true;

        player = playerParty.GetComponent<Player>();
        trainer = trainerParty.GetComponent<TrainerController>();
        StartCoroutine(SetupBattle());
    }


    public IEnumerator SetupBattle()
    {
        playerUnit.Clear();
        enemyUnit.Clear();
        GameObject.FindObjectOfType<Player>().playerActive = false;
        if (!this.isTrainerBattle)
        {
            // Wild Creature Battle
            playerUnit.Setup(playerParty.GetHealthyCreature());
            enemyUnit.Setup(wildCreature);
            dialogBox.SetMoveNames(playerUnit.creature.moves);

            yield return dialogBox.TypeDialog(
                $"A Wild {enemyUnit.creature._base.creatureName} Appeared",
                dialogBox.dialogText
            );
        }
        else
        {
            // Trainer Battle

            // Show player and trainer sprites
            playerUnit.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(false);

            playerImage.gameObject.SetActive(true);
            trainerImage.gameObject.SetActive(true);
            playerImage.sprite = player.sprite;
            trainerImage.sprite = trainer.sprite;

            yield return dialogBox.TypeDialog($"{trainer.trainerName} wants to battle");

            // Send out first creature of the trainer
            trainerImage.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(true);
            var enemyCreature = trainerParty.GetHealthyCreature();
            enemyUnit.Setup(enemyCreature);
            yield return dialogBox.TypeDialog($"{trainer.trainerName} sent out {enemyCreature._base.creatureName}");

            // Send out first creature of the player
            playerImage.gameObject.SetActive(false);
            playerUnit.gameObject.SetActive(true);
            var playerCreature = playerParty.GetHealthyCreature();
            playerUnit.Setup(playerCreature);
            yield return dialogBox.TypeDialog($"Go {playerCreature._base.creatureName}!");
            dialogBox.SetMoveNames(playerUnit.creature.moves);

        }

        partyScreen.Init();
        StartCoroutine(PlayerAction());
    }

    void BattleOver(bool won)
    {
        battleState = BattleState.BattleOver;
        playerParty.creatures.ForEach(p => p.OnBattleOver());
        StartCoroutine(OnBattleOver(won));
    }

    IEnumerator PlayerAction()
    {
        battleState = BattleState.ActionSelection;
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
        battleState = BattleState.MoveSelection;
        dialogBox.ToggleActionSelector(false);
        dialogBox.ToggleDialogText(false);
        dialogBox.ToggleMoveSelector(true);
    }

    IEnumerator RunTurns(BattleAction playerAction)
    {
        battleState = BattleState.RunningTurn;

        if (playerAction == BattleAction.Move)
        {
            playerUnit.creature.currentMove = playerUnit.creature.moves[currentMove];
            enemyUnit.creature.currentMove = enemyUnit.creature.GetRandomMove();


            int playerMovePriority = playerUnit.creature.currentMove.base_.priority;
            int enemyMovePriority = enemyUnit.creature.currentMove.base_.priority;

            // Check who goes first
            bool playerGoesFirst = true;
            if (enemyMovePriority > playerMovePriority)
            {
                playerGoesFirst = false;
            }
            else if (enemyMovePriority == playerMovePriority)
            {
                playerGoesFirst = playerUnit.creature.Speed >= enemyUnit.creature.Speed;
            }

            var firstUnit = (playerGoesFirst) ? playerUnit : enemyUnit;
            var secondUnit = (playerGoesFirst) ? enemyUnit : playerUnit;

            var secondCreature = secondUnit.creature;

            //First Turn
            yield return RunMove(firstUnit, secondUnit, firstUnit.creature.currentMove);
            yield return RunAfterTurn(firstUnit);
            if (battleState == BattleState.BattleOver)
            {
                yield break;
            }

            if (secondCreature.HP > 0)
            {
                //Second Turn
                yield return RunMove(secondUnit, firstUnit, secondUnit.creature.currentMove);
                yield return RunAfterTurn(secondUnit);
                if (battleState == BattleState.BattleOver)
                {
                    yield break;
                }
            }
        }
        else
        {
            if (playerAction == BattleAction.SwitchCreature)
            {
                var selectedCreature = playerParty.creatures[currentMember];
                battleState = BattleState.Busy;
                yield return SwitchCreature(selectedCreature);
            }

            // Enemy Turn
            var enemyMove = enemyUnit.creature.GetRandomMove();
            yield return RunMove(enemyUnit, playerUnit, enemyMove);
            yield return RunAfterTurn(enemyUnit);
            if (battleState == BattleState.BattleOver)
                yield break;
        }
        if (battleState != BattleState.BattleOver)
        {
            StartCoroutine(PlayerAction());
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
                yield return RunMoveEffects(
                    move.base_.effects,
                    sourceUnit.creature,
                    targetUnit.creature,
                    move.base_.target
                );
            }
            else
            {
                var damageDetails = targetUnit.creature.TakeDamage(move, sourceUnit.creature);
                yield return ShowDamageDetails(damageDetails);
                yield return targetUnit.hud.UpdateHP();
                yield return playerUnit.hud.UpdateHP();
            }
            if (
                move.base_.secondaryEffects != null
                && move.base_.secondaryEffects.Count > 0
                && targetUnit.creature.HP > 0
            )
            {
                foreach (var secondary in move.base_.secondaryEffects)
                {
                    var rnd = UnityEngine.Random.Range(1, 101);

                    if (rnd <= secondary.chance)
                    {
                        yield return RunMoveEffects(
                            secondary,
                            sourceUnit.creature,
                            targetUnit.creature,
                            secondary.target
                        );
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
            yield return dialogBox.TypeDialog(
                $"{sourceUnit.creature._base.creatureName}'s attack missed",
                dialogBox.dialogText
            );
        }
    }

    IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {
        if (battleState == BattleState.BattleOver)
        {
            yield break;
        }
        yield return new WaitUntil(() => battleState == BattleState.RunningTurn);
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
            sourceUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(0.5f);

            CheckForBattleOver(sourceUnit);
        }
    }

    IEnumerator RunMoveEffects(
        MoveEffects effects,
        Creature source,
        Creature target,
        MoveTarget moveTarget
    )
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
            yield return dialogBox.TypeDialog(message);
        }
    }

    IEnumerator OnBattleOver(bool won)
    {
        dialogBox.ToggleActionSelector(false);
        dialogBox.ToggleMoveSelector(false);
        if (won)
        {
            yield return dialogBox.TypeDialog("You Have Won The Battle!");
        }
        else
        {
            yield return dialogBox.TypeDialog("You Have Lost The Battle!");
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
            if (!isTrainerBattle)
            {
                BattleOver(true);
            }
            else
            {
                var nextCreature = trainerParty.GetHealthyCreature();
                if (nextCreature != null)
                {
                    // Send out next creature
                    StartCoroutine(SendNextTrainerCreature(nextCreature));
                }
                else
                {
                    BattleOver(true);
                }
            }
        }
    }

    IEnumerator PerformEnemyMove()
    {
        battleState = BattleState.RunningTurn;
        var move = enemyUnit.creature.GetRandomMove();
        yield return RunMove(enemyUnit, playerUnit, move);
        if (battleState != BattleState.PartyScreen)
            StartCoroutine(PlayerAction());
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
        {
            yield return dialogBox.TypeDialog("A Critical Hit");
        }
        if (damageDetails.TypeEffectiveness > 1f)
        {
            yield return dialogBox.TypeDialog("It's Super Effective!");
        }
        if (damageDetails.TypeEffectiveness < 1f)
        {
            yield return dialogBox.TypeDialog("It's Not Very Effective");
        }
    }

    void Update()
    {
        GameObject.FindObjectOfType<Player>().playerActive = false;
        switch (battleState)
        {
            case BattleState.ActionSelection:
                HandleActionSelection();
                break;
            case BattleState.MoveSelection:
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
                prevState = battleState;
                OpenPartyScreen();
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
            var move = playerUnit.creature.moves[currentMove];
            if (move.PP <= 0)
            {
                return;
            }
            dialogBox.ToggleMoveSelector(false);
            dialogBox.ToggleDialogText(true);
            StartCoroutine(RunTurns(BattleAction.Move));
        }
        else if (Input.GetButtonDown("Back"))
        {
            dialogBox.ToggleMoveSelector(false);
            dialogBox.ToggleDialogText(true);
            dialogBox.ToggleActionSelector(true);
            battleState = BattleState.ActionSelection;
            StartCoroutine(PlayerAction());
        }
    }

    public IEnumerator FinishBattle(bool playerIsWinner)
    {
        string winningText = playerIsWinner
            ? "Player"
            : $"Enemy {enemyUnit.creature._base.creatureName}";
        yield return dialogBox.TypeDialog($"{winningText} Has Won");
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

            if (prevState == BattleState.ActionSelection)
            {
                prevState = null;
                StartCoroutine(RunTurns(BattleAction.SwitchCreature));
            }
            else
            {
                battleState = BattleState.Busy;
                StartCoroutine(SwitchCreature(selectedMember));
            }
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
        battleState = BattleState.RunningTurn;
    }

    IEnumerator SendNextTrainerCreature(Creature nextCreature)
    {
        battleState = BattleState.Busy;
        enemyUnit.Setup(nextCreature);
        yield return dialogBox.TypeDialog($"{trainer.trainerName} send out {nextCreature._base.creatureName}");

        battleState = BattleState.RunningTurn;
    }
}
