using System.Collections.Generic;
using UnityEngine;
public class CreatureDB
{
    static Dictionary<string, CreatureBase> creatures;
    public static void Init()
    {
        creatures = new Dictionary<string, CreatureBase>();
        var creatureArr = Resources.LoadAll<CreatureBase>("");
        foreach (var creature in creatureArr)
        {
            if (creatures.ContainsKey(creature.creatureName))
            {
                Debug.LogError("There are duplicate creatures");
                continue;
            }
            creatures[creature.creatureName] = creature;
        }
    }
    public static CreatureBase GetCreatureByName(string name)
    {
        if (!creatures.ContainsKey(name))
        {
            Debug.LogError($"{name} Creature was not found");
            return null;
        }
        return creatures[name];
    }
}