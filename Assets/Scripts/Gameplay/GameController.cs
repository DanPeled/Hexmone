using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameState state;
    public BattleSystem battleSystem;
    public Player player;
    public static GameController instance;
    private void Awake()
    {
        this.player = GameObject.FindObjectOfType<Player>();
        ConditionDB.Init();
    }
    void Start()
    {
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
    }
    TrainerController trainer;
    public void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        battleSystem.isTrainerBattle = false;
        var playerParty = player.GetComponent<CreaturesParty>();
        var wildCreature = GameObject
            .FindObjectOfType<MapArea>()
            .GetComponent<MapArea>()
            .GetRandomWildCreature();
        var wildCreatureCopy = new Creature(wildCreature._base, wildCreature.level);
        battleSystem.StartBattle(playerParty, wildCreatureCopy);
    }
    public void StartTrainerBattle(TrainerController trainer)
    {
        state = GameState.Battle;
        this.trainer = trainer;
        battleSystem.gameObject.SetActive(true);
        var playerParty = player.GetComponent<CreaturesParty>();
        var trainerParty = trainer.GetComponent<CreaturesParty>();
        battleSystem.StartTrainerBattle(playerParty, trainerParty);
        player.SwitchCamera(player.cameras[1]);
    }
    void Update()
    {
        if (state == GameState.Dialog)
        {
            DialogManager.instance.HandleUpdate();
        }
        instance = this;
    }
    public void EndBattle(bool won){
        if (trainer != null && won){
            trainer.BattleLost();
            trainer = null;
        }
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
    }
}

public enum GameState
{
    Battle,
    FreeRoam,
    Dialog
}
