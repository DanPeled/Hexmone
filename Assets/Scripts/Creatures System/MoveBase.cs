using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Creatures/Create New Move")]
public class MoveBase : ScriptableObject
{
    public string moveName;
    public string description;
    public CreatureType type;
    public int power,
        accuracy,
        pp;
    public MoveCategory category;
    public MoveEffects effects;
    public MoveTarget target;
}

[System.Serializable]
public class MoveEffects
{
    public List<StatBoost> boosts;
}

[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}

public enum MoveTarget
{
    Foe,
    Self
}

public enum MoveCategory
{
    Physical,
    Special,
    Status
}
