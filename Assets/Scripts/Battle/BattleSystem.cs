using System.Runtime.CompilerServices;
using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
public enum BattleState
{
    Start,
    ActionSelection,
    MoveSelection,
    RunningTurn,
    Busy,
    PartyScreen,
    BattleOver, AboutToUse,
    MoveToForget
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
    public MoveSelectionUI moveSelectionUI;
    public BattleState? battleState,
        prevState;
    public int currentAction,
        currentMove;
    bool actionPossible = false;
    public Image playerImage, trainerImage;

    CreaturesParty playerParty, trainerParty;
    Creature wildCreature;
    Toggle up = new Toggle(),
        down = new Toggle(),
        left = new Toggle(),
        right = new Toggle();
    public bool isTrainerBattle = false, aboutToUseChoice = true;
    Player player;
    TrainerController trainer;
    public GameObject hexoballSprite;
    int escapeAttempts;
    MoveBase moveToLearn;
    public void StartBattle(CreaturesParty playerParty, Creature wildCreature)
    {
        this.playerParty = playerParty;
        this.wildCreature = wildCreature;
        player = playerParty.GetComponent<Player>();
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
        moveSelectionUI.gameObject.SetActive(false);
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

        escapeAttempts = 0;

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
        prevState = battleState;
        partyScreen.SetPartyData();
        partyScreen.gameObject.SetActive(true);
        partyScreen.SetMessageText("Choose A Creature");
        battleState = BattleState.PartyScreen;
    }
    IEnumerator AboutToUse(Creature newCreature)
    {
        battleState = BattleState.Busy;
        yield return dialogBox.TypeDialog($"{trainer.trainerName} is about to use {newCreature._base.creatureName}. Do you want to change creature?");
        battleState = BattleState.AboutToUse;
        dialogBox.ToggleChoiceBox(true);
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
                var selectedCreature = partyScreen.SelectedMember;
                battleState = BattleState.Busy;
                yield return SwitchCreature(selectedCreature);
            }
            else if (playerAction == BattleAction.UseItem)
            {
                dialogBox.ToggleActionSelector(false);
                yield return ThrowHexoball();
            }
            else if (playerAction == BattleAction.Run)
            {
                dialogBox.ToggleActionSelector(false);
                yield return TryToEscape();
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
                yield return HandleCreatureFainted(targetUnit);
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
            yield return HandleCreatureFainted(sourceUnit);
            yield return new WaitUntil(() => battleState == BattleState.RunningTurn);
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
    IEnumerator HandleCreatureFainted(BattleUnit faintedUnit)
    {
        faintedUnit.creature.HP = 0;
        if (!faintedUnit.isPlayerUnit)
        {
            yield return dialogBox.TypeDialog(
                $"The Enemy {faintedUnit.creature._base.creatureName} Fainted",
                dialogBox.dialogText
            );
        }
        else
        {
            yield return dialogBox.TypeDialog(
                $"Your {faintedUnit.creature._base.creatureName} Fainted",
                dialogBox.dialogText
            );
        }
        faintedUnit.PlayFaintAnimation();
        yield return new WaitForSeconds(0.5f);
        if (!faintedUnit.isPlayerUnit)
        {
            // Exp gain
            int expYield = faintedUnit.creature._base.expYield;
            int enemyLvl = faintedUnit.creature.level;
            float trainerBonus = (isTrainerBattle) ? 1.5f : 1f;

            int expGain = Mathf.FloorToInt((expYield * enemyLvl * trainerBonus) / 7);
            playerUnit.creature.exp += expGain;
            yield return dialogBox.TypeDialog($"{playerUnit.creature._base.creatureName} gained {expGain} exp");
            yield return playerUnit.hud.SetExpSmooth();
            // Check lvl up
            while (playerUnit.creature.CheckForLevelUp())
            {
                playerUnit.hud.SetLevel();
                yield return dialogBox.TypeDialog($"{playerUnit.creature._base.creatureName} grew to level {playerUnit.creature.level}");

                //Try to learn new move
                var newMove = playerUnit.creature.GetLearnableMoveAtCurrLevel();
                if (newMove != null)
                {
                    if (playerUnit.creature.moves.Count < playerUnit.creature._base.maxNumberOfMoves)
                    {
                        playerUnit.creature.LearnMove(newMove);
                        yield return dialogBox.TypeDialog($"{playerUnit.creature._base.creatureName} learned {newMove.moveBase.moveName}");
                        dialogBox.SetMoveNames(playerUnit.creature.moves);
                    }
                    else
                    {
                        // Option to forget move
                        yield return dialogBox.TypeDialog($"{playerUnit.creature._base.creatureName} is trying to learn {newMove.moveBase.moveName}");
                        yield return dialogBox.TypeDialog($"But it cannot learn more than {playerUnit.creature._base.maxNumberOfMoves} moves");
                        yield return ChooseMoveToForget(playerUnit.creature, newMove.moveBase);
                        yield return new WaitUntil(() => battleState != BattleState.MoveToForget);
                        yield return new WaitForSeconds(2f);
                    }
                }

                yield return playerUnit.hud.SetExpSmooth(true);
            }

            yield return new WaitForSeconds(1f);
        }
        CheckForBattleOver(faintedUnit);
    }
    IEnumerator ChooseMoveToForget(Creature creature, MoveBase newMove)
    {
        battleState = BattleState.Busy;
        yield return dialogBox.TypeDialog($"Choose a move you want to forget");
        moveSelectionUI.gameObject.SetActive(true);
        moveSelectionUI.SetMoveData(creature.moves.Select(x => x.base_).ToList(), newMove);
        moveToLearn = newMove;
        battleState = BattleState.MoveToForget;
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
        GameController.instance.EndBattle(won);
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
                    StartCoroutine(AboutToUse(nextCreature));
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
            case BattleState.AboutToUse:
                HandleAboutToUse();
                break;
            case BattleState.MoveToForget:
                Action<int> onMoveSelected = (moveIndex) =>
                {
                    moveSelectionUI.gameObject.SetActive(false);
                    if (moveIndex == 4)
                    {
                        // Dont learn the new move
                        StartCoroutine(dialogBox.TypeDialog($"{playerUnit.creature._base.creatureName} did not learn {moveToLearn.moveName}"));
                    }
                    else
                    {
                        // forget the selected move and learn new move
                        var selectedMove = playerUnit.creature.moves[moveIndex].base_;
                        StartCoroutine(dialogBox.TypeDialog($"{playerUnit.creature._base.creatureName} forgot {selectedMove.moveName} and learned {moveToLearn.moveName}"));

                        playerUnit.creature.moves[moveIndex] = new Move(moveToLearn);
                    }
                    moveToLearn = null;
                    battleState = BattleState.RunningTurn;
                };
                moveSelectionUI.HandleMoveSelection(onMoveSelected);
                break;

        }
    }
    void HandleAboutToUse()
    {
        if (InputSystem.instance.up.isClicked() || InputSystem.instance.down.isClicked())
        {
            aboutToUseChoice = !aboutToUseChoice;
        }
        dialogBox.UpdateChoiceBoxSelection(aboutToUseChoice);

        if (InputSystem.instance.action.isClicked())
        {
            dialogBox.ToggleChoiceBox(false);
            dialogBox.choiceBox.SetActive(false);
            if (aboutToUseChoice)
            {
                // Yes Option
                prevState = BattleState.AboutToUse;
                OpenPartyScreen();

            }
            else
            {
                // No Option
                StartCoroutine(SendNextTrainerCreature());
            }
        }
        else if (InputSystem.instance.back.isClicked())
        {
            dialogBox.ToggleChoiceBox(false);
            StartCoroutine(SendNextTrainerCreature());
        }
    }
    public void HandleActionSelection()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        if (InputSystem.instance.right.isClicked())
        {
            currentAction++;
        }
        else if (InputSystem.instance.left.isClicked())
        {
            currentAction--;
        }
        else if (InputSystem.instance.down.isClicked())
        {
            currentAction += 2;
        }
        else if (InputSystem.instance.up.isClicked())
        {
            currentAction -= 2;
        }

        currentAction = Mathf.Clamp(currentAction, 0, 3);
        dialogBox.UpdateActionSelection(currentAction);

        if (InputSystem.instance.action.isClicked() && this.actionPossible)
        {
            if (currentAction == 0)
            {
                // Fight
                MoveSelection();
            }
            else if (currentAction == 1)
            {
                // Bag
                StartCoroutine(RunTurns(BattleAction.UseItem));
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
                StartCoroutine(RunTurns(BattleAction.Run));
            }
        }
    }

    void HandleMoveSelection()
    {
        if (InputSystem.instance.right.isClicked())
        {
            currentMove++;
        }
        else if (InputSystem.instance.left.isClicked())
        {
            currentMove--;
        }
        else if (InputSystem.instance.down.isClicked())
        {
            currentMove += 2;
        }
        else if (InputSystem.instance.up.isClicked())
        {
            currentMove -= 2;
        }

        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.creature.moves.Count - 1);
        dialogBox.UpdateMoveSelection(currentMove, playerUnit.creature.moves[currentMove]);
        if (InputSystem.instance.action.isClicked())
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
        else if (InputSystem.instance.back.isClicked())
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
        Action onSelected = () =>
        {
            dialogBox.ToggleActionSelector(false);
            dialogBox.ToggleMoveSelector(false);
            var selectedMember = partyScreen.SelectedMember;
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
        };
        Action onBack = () =>
        {

            if (playerUnit.creature.HP <= 0)
            {
                partyScreen.SetMessageText("You have to choose a creature to continue");
                return;
            }
            partyScreen.gameObject.SetActive(false);
            if (prevState == BattleState.AboutToUse)
            {
                prevState = null;
                StartCoroutine(SendNextTrainerCreature());
            }
            else
            {
                StartCoroutine(PlayerAction());
            }
        };
        dialogBox.ToggleActionSelector(false);
        partyScreen.HandleUpdate(onSelected, onBack);


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
        if (prevState == null)
        {
            battleState = BattleState.RunningTurn;
        }
        else if (prevState == BattleState.AboutToUse)
        {
            prevState = null;
            StartCoroutine(SendNextTrainerCreature());
        }
    }

    IEnumerator SendNextTrainerCreature()
    {
        var nextCreature = trainerParty.GetHealthyCreature();
        battleState = BattleState.Busy;
        enemyUnit.Setup(nextCreature);
        yield return dialogBox.TypeDialog($"{trainer.trainerName} send out {nextCreature._base.creatureName}");

        battleState = BattleState.RunningTurn;
    }
    IEnumerator ThrowHexoball()
    {
        battleState = BattleState.Busy;
        if (isTrainerBattle)
        {
            yield return dialogBox.TypeDialog($"You can't steal the trainer's creature!");
            battleState = BattleState.RunningTurn;
            yield break;
        }
        yield return dialogBox.TypeDialog($"{player.playerName} used HEXOBALL!");

        var hexoBallObject = Instantiate(hexoballSprite, playerUnit.transform.position - new Vector3(2, 0), Quaternion.identity);
        var hexoball = hexoBallObject.GetComponent<SpriteRenderer>();

        //Animations
        yield return hexoball.transform.DOJump(enemyUnit.transform.position + new Vector3(0, 2f), 2f, 1, 1f).WaitForCompletion();
        yield return enemyUnit.PlayCaptureAnimation();
        yield return hexoball.transform.DOMoveY(enemyUnit.transform.position.y - 1.3f, 0.5f).WaitForCompletion();
        int shakeCount = TryToCatchCreature(enemyUnit.creature);
        for (int i = 0; i < Mathf.Min(shakeCount, 3); i++)
        {
            yield return new WaitForSeconds(0.5f);
            hexoball.transform.DOPunchRotation(new Vector3(0, 0, 10f), 0.8f).WaitForCompletion();
        }
        if (shakeCount == 4)
        {
            // Creature is caught
            yield return dialogBox.TypeDialog($"{enemyUnit.creature._base.creatureName} was caught");
            yield return hexoball.DOFade(0, 1.5f).WaitForCompletion();

            playerParty.AddCreature(enemyUnit.creature);
            yield return dialogBox.TypeDialog($"{enemyUnit.creature._base.creatureName} has been added to your party");

            Destroy(hexoball);
            BattleOver(true);
        }
        else
        {
            // Creature broke out
            yield return new WaitForSeconds(1f);
            hexoball.DOFade(0, 0.2f);
            yield return enemyUnit.PlayBreakOutAnimation();
            if (shakeCount < 2)
            {
                yield return dialogBox.TypeDialog($"{enemyUnit.creature._base.creatureName} broke free");
            }
            else
            {
                yield return dialogBox.TypeDialog($"Almost caught it");
            }
            Destroy(hexoball);
            battleState = BattleState.RunningTurn;

        }
    }
    int TryToCatchCreature(Creature creature)
    {
        float a = (3 * creature.maxHealth * creature.HP) *
         creature._base.catchRate * ConditionDB.GetStatusBonus(creature.status) / (3 * creature.maxHealth);
        if (a >= 255)
        {
            return 4;
        }
        float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a));
        int shakeCount = 0;
        while (shakeCount < 4)
        {
            if (UnityEngine.Random.Range(0, 65535) >= b)
            {
                break;
            }
            shakeCount++;
        }
        return shakeCount;
    }
    IEnumerator TryToEscape()
    {
        battleState = BattleState.Busy;

        if (isTrainerBattle)
        {
            yield return dialogBox.TypeDialog($"You can't run from trainer battles!");
            battleState = BattleState.RunningTurn;
            yield break;
        }
        escapeAttempts++;
        int playerSpeed = playerUnit.creature.Speed;
        int enemySpeed = enemyUnit.creature.Speed;
        if (enemySpeed < playerSpeed)
        {
            yield return dialogBox.TypeDialog($"Ran away safely!");
            BattleOver(true);
        }
        else
        {
            float f = (playerSpeed * 128) / enemySpeed + 30 * escapeAttempts;
            f = f % 256;

            if (UnityEngine.Random.Range(0, 256) < f)
            {
                yield return dialogBox.TypeDialog($"Ran away safely!");
                BattleOver(true);
            }
            else
            {
                yield return dialogBox.TypeDialog($"Can't escape!");
                battleState = BattleState.RunningTurn;
            }
        }
    }
}
