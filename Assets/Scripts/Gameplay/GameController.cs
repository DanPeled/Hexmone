using System;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameState state;
    public BattleSystem battleSystem;
    public Player player;
    public static GameController instance;
    public PartyScreen partyScreen;
    MenuController menu;
    public InventoryUI inventoryUI;
    private void Awake()
    {
        menu = GetComponent<MenuController>();
        this.player = GameObject.FindObjectOfType<Player>();
        ConditionDB.Init();
        MovesDB.Init();
        CreatureDB.Init();
    }
    void Start()
    {
        partyScreen.Init();
        battleSystem.gameObject.SetActive(false);
        DialogManager.instance.OnShowDialog += () =>
        {
            state = GameState.Dialog;
        };

        DialogManager.instance.OnCloseDialog += () =>
        {
            if (state == GameState.Dialog)
            {
                state = GameState.FreeRoam;
            }
        };
        menu.onBack += () =>
        {
            state = GameState.FreeRoam;
        };
        menu.onMenuSelected += onMenuSelected;

    }
    TrainerController trainer;
    public void StartBattle()
    {
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
        battleSystem.StartBattle(playerParty, wildCreatureCopy);
    }
    public void StartTrainerBattle(TrainerController trainer)
    {
        state = GameState.Battle;
        this.trainer = trainer;
        battleSystem.gameObject.SetActive(true);
        battleSystem.isTrainerBattle = true;
        var playerParty = player.GetComponent<CreaturesParty>();
        var trainerParty = trainer.GetComponent<CreaturesParty>();
        battleSystem.StartTrainerBattle(playerParty, trainerParty);
        player.SwitchCamera(1);
    }
    void Update()
    {
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
            if (InputSystem.instance.start.isClicked())
            {
                menu.OpenMenu();
                state = GameState.Menu;
            }
        }
        if (state == GameState.Menu)
        {
            menu.HandleUpdate();

        } else if (state == GameState.PartyScreen){
            Action onSelected = () => {
                //TODO:  Go to Summary Screen
            }; Action onBack = () => {
                partyScreen.gameObject.SetActive(false);
                state = GameState.FreeRoam;
            };
            partyScreen.HandleUpdate(onSelected, onBack);
        } else if (state == GameState.Bag){
            Action onBack = () =>
            {
                inventoryUI.gameObject.SetActive(false);
                state = GameState.FreeRoam;
            };
            inventoryUI.HandleUpdate(onBack);
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
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
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
                state = GameState.Bag;
                break;
            case 2:
                // Save
                SavingSystem.i.Save("saveSlot1");
                state = GameState.FreeRoam;
                break;
            case 3:
                // Load
                SavingSystem.i.Load("saveSlot1");
                state = GameState.FreeRoam;
                break;
        }
    }
}

public enum GameState
{
    Battle,
    FreeRoam,
    Dialog, Menu, Paused, CutScene, PartyScreen, Bag
}
