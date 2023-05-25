using System.Collections;
using UnityEngine;
using System.Linq;
using System;
using System.Collections.Generic;

public class CreaturesParty : MonoBehaviour
{
    public event Action onUpdated;
    public List<Creature> creatures;
    CreaturesBox creaturesBox;
    public List<Creature> Creatures
    {
        get { return creatures; }
        set
        {
            creatures = value;
            onUpdated?.Invoke();
        }
    }

    public void Start()
    {
        creaturesBox = CreaturesBox.GetPlayerBox();
        foreach (var creature in Creatures)
        {
            creature.Init();
        }
    }

    /// <summary>
    /// Gets the first healthy (with hp > 0) creature
    /// </summary>
    /// <returns>The first healthy creature found in the party</returns>
    public Creature GetHealthyCreature()
    {
        return Creatures.Where(x => x.HP > 0).FirstOrDefault();
    }

    /// <summary>
    /// Adds a creature to the creature party / PC
    /// </summary>
    /// <param name="newCreature">The creature being added</param>
    public void AddCreature(Creature newCreature)
    {
        if (Creatures.Count < 6)
        {
            Creatures.Add(newCreature);
            onUpdated?.Invoke();
        }
        else
        {
            // TODO: Add the to the PC
            creaturesBox.Add(newCreature);
        }
    }

    /// <summary>
    /// Gets the player's party
    /// </summary>
    public static CreaturesParty GetPlayerParty()
    {
        if (FindObjectOfType<Player>() != null)
            return FindObjectOfType<Player>().GetComponent<CreaturesParty>();
        else
            return null;
    }

    /// <summary>
    /// Checks for evolutions possible in the party
    /// </summary>
    /// <returns>Wheter some creature in the party has a possible evolution</returns>
    public bool CheckForEvolutions()
    {
        return creatures.Any(p => p.CheckForEvolution() != null);
    }

    public IEnumerator RunEvolutions()
    {
        foreach (var creature in creatures)
        {
            var evolution = creature.CheckForEvolution();
            if (evolution != null)
            {
                yield return EvolutionManager.instance.Evolve(creature, evolution);
            }
        }
        onUpdated?.Invoke();
    }

    public void PartyUpdated()
    {
        onUpdated?.Invoke();
    }

    public string GetPartyDiscordStatus()
    {
        string[] names = creatures.Select(c => c._base.name).ToArray();
        int[] lvls = creatures.Select(c => c.level).ToArray();
        string res = "";
        for (int i = 0; i < creatures.Count; i++)
        {
            string text =
                $"{(names[i] != names[0] ? ", " : "")}{names[i]} {(creatures[i].GetName() == names[i] ? "" : ("\"" + creatures[i].GetName() + "\""))} lvl {lvls[i]}";
            res += $"{text}";
        }
        return res;
    }
}
