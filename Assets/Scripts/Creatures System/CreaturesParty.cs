using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class CreaturesParty : MonoBehaviour
{
    public event Action onUpdated;
    public List<Creature> creatures;
    public List<Creature> Creatures {
        get {
            return creatures;
        } set {
            creatures = value;
            onUpdated?.Invoke();
        }
    }

    public void Start()
    {
        foreach (var creature in Creatures)
        {
            creature.Init();
        }
    }

    public Creature GetHealthyCreature()
    {
        return Creatures.Where(x => x.HP > 0).FirstOrDefault();
    }
    public void AddCreature(Creature newCreature){
        if (Creatures.Count < 6){
            Creatures.Add(newCreature);
            onUpdated?.Invoke();
        } else {
            // TODO: Add the to the PC
        }
    }
    public static CreaturesParty GetPlayerParty(){
        return FindObjectOfType<Player>().GetComponent<CreaturesParty>();
    }
}