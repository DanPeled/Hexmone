using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Linq;
// using Unity.RemoteConfig;
public class MapArea : MonoBehaviour
{
    public struct userAttributes { }
    public struct appAttributes { }
    public List<CreatureEncounterRecord> wildCreatures;
    [HideInInspector]
    public int totalChance = 0;
    public void Awake()
    {
        // ConfigManager.FetchCompleted += CheckForEvent;
        // ConfigManager.FetchConfigs<userAttributes, appAttributes>(new userAttributes(), new appAttributes());
    }
    // void CheckForEvent(ConfigResponse response)
    // {

    // }
    void OnValidate()
    {
        totalChance = 0;
        foreach (var record in wildCreatures)
        {
            record.chanceLower = totalChance;
            record.chanceUpper = totalChance + record.chancePercentage;
            totalChance = totalChance + record.chancePercentage;
        }
    }
    public Creature GetRandomWildCreature()
    {
        bool repainted = Random.Range(1, 4070) == 1;
        int r = Random.Range(1, 101);
        var creatureRecord = wildCreatures.FirstOrDefault(c => r >= c.chanceLower && r <= c.chanceUpper);
        var levelRange = creatureRecord.levelRange;
        var lvl = levelRange.y == 0 ? levelRange.x : Random.Range(levelRange.x, levelRange.y + 1);

        var wildCreature = new Creature(creatureRecord.creature, lvl, repainted);
        wildCreature.Init();
        return wildCreature;
    }
}
[System.Serializable]
public class CreatureEncounterRecord
{
    public CreatureBase creature;
    public Vector2Int levelRange;
    public int chancePercentage;

    [field: SerializeField]
    [field: HideInInspector]
    public int chanceLower { get; set; }
    [field: SerializeField]
    [field: HideInInspector]
    public int chanceUpper { get; set; }
}
