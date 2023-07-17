using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameController : MonoBehaviour
{
    public bool battleActive = false; // Flag to track if a battle is already active
    public GameState state,
        prevState,
        stateBeforeEvolution;
    public BattleSystem battleSystem;
    public Player player;
    public static GameController instance;
    public PartyScreen partyScreen;
    MenuController menu;
    public InventoryUI inventoryUI;
    public TextMeshProUGUI screenshotText;

    private Coroutine screenshotCoroutine;

    private void Awake()
    {
        instance = this;
        menu = GetComponent<MenuController>();
        this.player = GameObject.FindObjectOfType<Player>();
        ConditionDB.Init();
        MovesDB.Init();
        CreatureDB.Init();
        ItemDB.Init();
        QuestDB.Init();
    }

    public void PauseGame(bool state)
    {
        this.state = state ? GameState.Paused : GameState.FreeRoam;
    }

    void Start()
    {
        partyScreen.Init();
        battleSystem.gameObject.SetActive(false);
        DialogManager.instance.OnShowDialog += () =>
        {
            prevState = state;
            state = GameState.Dialog;
        };

        DialogManager.instance.OnCloseDialog += () =>
        {
            if (state == GameState.Dialog)
            {
                state = prevState;
            }
        };
        menu.onBack += () =>
        {
            state = GameState.FreeRoam;
        };
        menu.onMenuSelected += onMenuSelected;
        EvolutionManager.instance.onStartEvolution += () =>
        {
            stateBeforeEvolution = state;
            state = GameState.Evolution;
        };
        EvolutionManager.instance.onCompleteEvolution += () =>
        {
            partyScreen.SetPartyData();
            state = stateBeforeEvolution;
        };
        ShopController.i.onStart += () => state = GameState.Shop;
        ShopController.i.onFinish += () => state = GameState.FreeRoam;
    }

    TrainerController trainer;

    public void StartBattle()
    {
        if (battleActive) // Check if a battle is already active
            return;
        player.playerActive = false;
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        battleSystem.isTrainerBattle = false;
        var playerParty = player.GetComponent<CreaturesParty>();
        var wildCreature = GameObject
            .FindObjectOfType<MapArea>()
            .GetComponent<MapArea>()
            .GetRandomWildCreature();
        var wildCreatureCopy = new Creature(wildCreature._base, wildCreature.level);
        battleSystem.isTrainerBattle = false;
        battleSystem.StartWildBattle(playerParty, wildCreatureCopy);
        battleActive = true; // Set the battle flag to true
    }

    public void StartTrainerBattle(TrainerController trainer)
    {
        if (battleActive) // Check if a battle is already active
            return;
        state = GameState.Battle;
        this.trainer = trainer;
        battleSystem.gameObject.SetActive(true);
        battleSystem.isTrainerBattle = true;
        var playerParty = player.GetComponent<CreaturesParty>();
        var trainerParty = trainer.GetComponent<CreaturesParty>();
        battleSystem.StartTrainerBattle(playerParty, trainerParty);
        player.SwitchCamera(1);
        battleActive = true; // Set the battle flag to true
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            if (screenshotCoroutine != null)
            {
                StopCoroutine(screenshotCoroutine);
            }
            screenshotCoroutine = StartCoroutine(TakeScreenshot());
        }
        if (state == GameState.Console)
        {
            return;
        }
        if (state == GameState.Dialog)
        {
            DialogManager.instance.HandleUpdate();
        }
        if (state != GameState.FreeRoam)
        {
            player.playerActive = false;
        }
        else
        {
            player.playerActive = true;
            if (InputSystem.start.isClicked())
            {
                menu.OpenMenu();
                state = GameState.Menu;
                Time.timeScale = 0;
            }
        }
        if (state == GameState.Menu)
        {
            menu.HandleUpdate();
        }
        else if (state == GameState.PartyScreen)
        {
            Action onSelected = () => {
                //TODO:  Go to Summary Screen
            };
            Action onBack = () =>
            {
                partyScreen.gameObject.SetActive(false);
                state = GameState.FreeRoam;
            };
            partyScreen.HandleUpdate(onSelected, onBack);
        }
        else if (state == GameState.Bag)
        {
            Action onBack = () =>
            {
                inventoryUI.gameObject.SetActive(false);
                inventoryUI.UpdateItemList();

                state = GameState.FreeRoam;
            };
            inventoryUI.HandleUpdate(onBack);
        }
        else if (state == GameState.Shop)
        {
            ShopController.i.HandleUpdate();
        }

        if (ShopController.i.shopUI.gameObject.activeInHierarchy)
        {
            state = GameState.Shop;
        }
        instance = this;
    }

    public void EndBattle(bool won)
    {
        if (trainer != null && won)
        {
            trainer.BattleLost();
            trainer = null;
        }
        partyScreen.SetPartyData();
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        AudioManager.i.StopMusic();
        var playerParty = player.GetComponent<CreaturesParty>();
        bool hasEvolutions = playerParty.CheckForEvolutions();
        if (hasEvolutions)
            StartCoroutine(playerParty.RunEvolutions());
        // else AudioManager.i.PlayMusic(); // play main music
        battleActive = false; // Reset the battle flag
    }

    void onMenuSelected(int selected)
    {
        switch (selected)
        {
            case 0:
                // creature
                partyScreen.gameObject.SetActive(true);
                partyScreen.SetPartyData();
                state = GameState.PartyScreen;
                break;
            case 1:
                // Bag
                inventoryUI.gameObject.SetActive(true);
                inventoryUI.UpdateItemList();
                state = GameState.Bag;
                break;
            case 2:
                // Save
                SavingSystem.i.Save("saveSlot1");
                state = GameState.FreeRoam;
                break;
            case 3:
                // Load
                player.loadPos = true;
                SavingSystem.i.Load("saveSlot1");
                state = GameState.FreeRoam;
                break;
        }
    }

    public void StartCutsceneState()
    {
        state = GameState.CutScene;
    }

    public void StartFreeRoamState()
    {
        state = GameState.FreeRoam;
    }

    public void MoveCamera(Vector2 moveOffset)
    {
        //LevelLoader.i.Load();
        player.cameras[player.camIndex]
            .transform
            .position += new Vector3(moveOffset.x, moveOffset.y);
    }

    public IEnumerator TakeScreenshot()
    {
        ED.SC.Extra.ScreenCommands.CaptureScreenshot();
        yield return screenshotText.DOFade(255, 1f).WaitForCompletion();
        yield return screenshotText.DOFade(0, 1f).WaitForCompletion();
    }
}

public enum GameState
{
    Battle,
    FreeRoam,
    Dialog,
    Menu,
    Paused,
    CutScene,
    PartyScreen,
    Bag,
    Evolution,
    Shop,
    Console,
    SummaryScreen
}
