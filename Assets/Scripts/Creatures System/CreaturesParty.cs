using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CreaturesParty : MonoBehaviour
{
    public List<Creature> creatures;

    void Start()
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
}
