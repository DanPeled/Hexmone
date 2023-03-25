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
    Electric,
    Grass,
    Ice,
    Fighting,
    Poison,
    Ground,
    Flying,
    Psychic,
    Bug,
    Rock,
    Ghost,
    Dragon,
    Dark,
    Steel,
    Fairy
}

public enum Stat
{
    Attack,
    Defense,
    SpAttack,
    SpDefense,
    Speed,

    // These 2 aren't actual stats, they are used to boost the move accuracy
    Accuracy,
    Evasion
}

public class TypeChart
{
    static float[,] chart =
    {
    /*              NOR FIR WAT ELE GRA ICE FIG POI GRO FLY PSY BUG ROC GHO DRG DAR STE FAI  */
    /* Normal    */ { 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 1f, 1f, 1f, 0.5f, 0f, 1f, 1f, 0.5f, 1f },
    /* Fire      */ { 1f, 0.5f, 2f, 1f, 0.5f, 0.5f, 1f, 1f, 2f, 1f, 1f, 2f, 0.5f, 1f, 1f, 1f, 0.5f, 1f },
    /* Water     */ { 1f, 2f, 0.5f, 1f, 2f, 0.5f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 1f, 0.5f, 1f },
    /* Electric */ { 1f, 1f, 2f, 0.5f, 0.5f, 1f, 1f, 1f, 0f, 2f, 1f, 1f, 1f, 1f, 1f, 1f, 0.5f, 1f },
    /* Grass     */ { 1f, 2f, 0.5f, 1f, 0.5f, 2f, 1f, 2f, 0.5f, 2f, 1f, 0.5f, 2f, 1f, 1f, 1f, 0.5f, 1f },
    /* Ice       */ { 1f, 2f, 1f, 1f, 2f, 0.5f, 1f, 1f, 2f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 0.5f, 1f },
    /* Fighting */ { 2f, 1f, 1f, 1f, 1f, 2f, 1f, 0.5f, 1f, 0.5f, 0.5f, 0.5f, 2f, 0f, 1f, 2f, 2f, 0.5f },
    /* Poison    */ { 1f, 1f, 1f, 1f, 2f, 1f, 1f, 0.5f, 0.5f, 1f, 1f, 1f, 0.5f, 0.5f, 1f, 1f, 0f, 2f },
    /* Ground    */ { 1f, 1f, 1f, 2f, 0f, 2f, 1f, 0.5f, 2f, 1f, 1f, 0.5f, 2f, 1f, 1f, 1f, 1f, 1f },
    /* Flying    */ { 1f, 1f, 1f, 2f, 0.5f, 1f, 2f, 1f, 0f, 1f, 1f, 2f, 0.5f, 1f, 1f, 1f, 1f, 1f },
    /* Psychic   */ { 1f, 1f, 1f, 1f, 1f, 1f, 0.5f, 2f, 1f, 1f, 0.5f, 1f, 1f, 1f, 2f, 2f, 1f, 1f },
    /* Bug       */ { 1f, 0.5f, 1f, 1f, 2f, 1f, 0.5f, 1f, 0.5f, 2f, 1f, 2f, 1f, 0.5f, 2f, 1f, 0.5f, 0.5f },
    /* Rock      */ { 0.5f, 2f, 1f, 1f, 2f, 1f, 0.5f, 1f, 2f, 0.5f, 1f, 1f, 1f, 1f, 1f, 1f, 0.5f, 1f },
    /* Ghost     */ { 0f, 1f, 1f, 1f, 1f, 1f, 0f, 0.5f, 1f, 1f, 2f, 1f, 1f, 2f, 1f, 1f, 1f, 2f },
    /* Dragon    */ { 1f, 0.5f, 0.5f, 0.5f, 0.5f, 2f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 2f, 1f },
    /* Dark      */ { 1f, 1f, 1f, 1f, 1f, 1f, 0.5f, 1f, 1f, 1f, 0f, 2f, 1f, 2f, 1f, 0.5f, 1f, 0.5f },
    /* Steel     */ { 0.5f, 2f, 1f, 0.5f, 2f, 1f, 1f, 1f, 0.5f, 2f, 1f, 0.5f, 1f, 0.5f, 1f, 0.5f, 0.5f, 2f },
    /* Fairy     */ { 1f, 0.5f, 1f, 1f, 1f, 1f, 2f, 0.5f, 1f, 1f, 1f, 1f, 1f, 1f, 0f, 2f, 0.5f, 1f }
    };

    public static float GetAffectiveness(CreatureType attackType, CreatureType defenseType)
    {
        if (attackType == CreatureType.Normal || defenseType == CreatureType.Normal)
        {
            return 1;
        }
        int row = (int)attackType - 1;
        int col = (int)defenseType - 1;
        return chart[row, col];
    }
}
