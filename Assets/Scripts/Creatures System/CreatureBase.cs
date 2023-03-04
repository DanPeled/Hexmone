using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq.Expressions;
using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "Creature", menuName = "Creatures/ Create New Creature")]
public class CreatureBase : ScriptableObject
{
    public string creatureName,
        description;
    public int maxHealth;
    public const int moveAmount = 3;
    public CreatureType type1;
    public CreatureType type2;
    public int damage;
    public Sprite frontSprite;
    public Sprite backSprite;
    public int defense;
    public int spAttack;
    public int spDefense;
    public int speed;
    public List<LearnableMove> learnableMoves = new List<LearnableMove>();
}

[System.Serializable]
public class LearnableMove
{
    public MoveBase moveBase;
    public int level;
}

public enum CreatureType
{
    Normal,
    Fire,
    Water,
    Air,
    Rock,
    Bug,
    Grass,
    Dragon,
    Diffrent
}

public enum Stat
{
    Attack,
    Defense,
    SpAttack,
    SpDefense,
    Speed
}

public class TypeChart
{
    static float[][] chart =
    {
        //
        /*NOR*/new float[]
        {
            1f,
            1f,
            1f
        },
        /*FIR*/new float[] { 1f, 0.5f, 0.5f },
        /*WAT*/new float[] { 1f, 2f, 0.5f }
    };

    public static float GetAffectiveness(CreatureType attackType, CreatureType defenseType)
    {
        if (attackType == CreatureType.Normal || defenseType == CreatureType.Normal)
        {
            return 1;
        }
        int row = (int)attackType - 1;
        int col = (int)defenseType - 1;
        return chart[row][col];
    }
}
