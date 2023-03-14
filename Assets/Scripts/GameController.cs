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
}

public enum GameState
{
    Battle,
    FreeRoam
}
