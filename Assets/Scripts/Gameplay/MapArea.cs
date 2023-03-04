using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class MapArea : MonoBehaviour
{
    public List<Creature> wildCreatures;

    public Creature GetRandomWildCreature()
    {
        var wildCreature = wildCreatures[UnityEngine.Random.Range(0, wildCreatures.Count)];
        wildCreature.Init();
        return wildCreature;
    }
}
