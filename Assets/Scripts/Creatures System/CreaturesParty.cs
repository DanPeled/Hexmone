using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class CreaturesParty : MonoBehaviour
{
    public List<Creature> creatures;
    public event Action onUpdated;

    public void Start()
    {
        foreach (var creature in creatures)
        {
            creature.Init();
        }
    }

    public Creature GetHealthyCreature()
    {
        return creatures.Where(x => x.HP > 0).FirstOrDefault();
    }
    public void AddCreature(Creature newCreature){
        if (creatures.Count < 6){
            creatures.Add(newCreature);
            onUpdated?.Invoke();
        } else {
            // TODO: Add the to the PC
        }
    }
    public static CreaturesParty GetPlayerParty(){
        return FindObjectOfType<Player>().GetComponent<CreaturesParty>();
    }
}