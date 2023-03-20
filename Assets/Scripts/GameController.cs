using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameState state;
    public BattleSystem battleSystem;
    public Player player;

    private void Awake()
    {
        this.player = GameObject.FindObjectOfType<Player>();
        ConditionDB.Init();
    }
    void Start()
    {
        DialogManager.instance.OnShowDialog += () => {
            state = GameState.Dialog;
        };

       DialogManager.instance.OnCloseDialog += () => {
           if (state == GameState.Dialog){
            state = GameState.FreeRoam;
           }
        };
    }
public void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        var playerParty = player.GetComponent<CreaturesParty>();
        var wildCreature = GameObject
            .FindObjectOfType<MapArea>()
            .GetComponent<MapArea>()
            .GetRandomWildCreature();
        battleSystem.StartBattle(playerParty, wildCreature);
    }
    void Update()
    {
        if (state == GameState.Dialog){
            DialogManager.instance.HandleUpdate();
        }
    }
}

public enum GameState
{
    Battle,
    FreeRoam,
    Dialog
}
