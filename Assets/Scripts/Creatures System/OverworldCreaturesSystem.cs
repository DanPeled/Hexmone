using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class OverworldCreaturesSystem : MonoBehaviour
{
    public SerializedDictionary<CreatureBase, List<Sprite>> creatures;
    public OverworldCreature overworldCreature;
    public CreatureBase defaultCreature;

    void Start()
    {
        if (overworldCreature != null)
            SetCreature(defaultCreature);
    }

    public static OverworldCreaturesSystem GetOverworldCreaturesSystem()
    {
        return FindObjectOfType<GameController>().GetComponent<OverworldCreaturesSystem>();
    }

    public void SetCreature(CreatureBase c)
    {
        overworldCreature.Setup(creatures.GetValueOrDefault(c));
    }
}
