using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq.Expressions;
using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "Creature", menuName = "Creatures/ Create New Creature")]
public class CreatureBase : ScriptableObject
{
    public string name,
        description;

    [Header("Sprites")]
    public Sprite frontSprite;
    public Sprite backSprite;
    public Sprite repaintedFrontSprite,
        repaintedBackSprite;

    [Header("Types")]
    public CreatureType type1;
    public CreatureType type2;

    [Header("Stats")]
    public int maxHealth;
    public const int moveAmount = 3;
    public int damage;
    public int defense;
    public int spAttack;
    public int spDefense;
    public int speed;
    public int catchRate = 225;
    public int expYield;
    public GrowthRate growthRate;

    [Header("Moves")]
    public List<LearnableMove> learnableMoves = new List<LearnableMove>();
    public List<MoveBase> learnableByItems = new List<MoveBase>();

    [Header("Evolutions")]
    public List<Evolution> evolutions;

    [HideInInspector]
    public int maxNumberOfMoves = 4;
    public string Name
    {
        get { return name; }
    }

    public int GetExpForLevel(int level)
    {
        if (growthRate == GrowthRate.Fast)
        {
            return 4 * (level * level * level) / 5;
        }
        else if (growthRate == GrowthRate.MeduimFast)
        {
            return level * level * level;
        }
        else if (growthRate == GrowthRate.MeduimSlow)
        {
            return Mathf.FloorToInt(
                (6f / 5f) * Mathf.Pow(level, 3f) - 15f * Mathf.Pow(level, 2f) + 100f * level - 140f
            );
        }
        else if (growthRate == GrowthRate.Slow)
        {
            return Mathf.FloorToInt((5 * Mathf.Pow(level, 3)) / 4f);
        }
        else if (growthRate == GrowthRate.Erratic)
        {
            if (level <= 50)
            {
                return Mathf.FloorToInt((level * level * level) * (100 - level) / 50);
            }
            else if (level <= 68)
            {
                return Mathf.FloorToInt((level * level * level) * (150 - level) / 100);
            }
            else if (level <= 98)
            {
                return Mathf.FloorToInt((level * level * level) * ((1911 - 10 * level) / 3) / 500);
            }
            else
            {
                return Mathf.FloorToInt((level * level * level) * (160 - level) / 100);
            }
        }
        else if (growthRate == GrowthRate.Fluctuating)
        {
            if (level <= 15)
            {
                return Mathf.FloorToInt(
                    (level * level * level) * ((Mathf.Floor((level + 1) / 3f) + 24) / 50f)
                );
            }
            else if (level <= 36)
            {
                return Mathf.FloorToInt((level * level * level) * ((level + 14) / 50f));
            }
            else
            {
                return Mathf.FloorToInt(
                    (level * level * level) * ((Mathf.Floor(level / 2f) + 32) / 50f)
                );
            }
        }

        return -1;
    }
}

[System.Serializable]
public class Evolution
{
    public CreatureBase evolvesInto;
    public int requiredLevel;
    public EvolutionItem requiredItem;
}

[System.Serializable]
public class LearnableMove
{
    public MoveBase moveBase;
    public int level;
}

public enum GrowthRate
{
    Erratic,
    Fast,
    MeduimFast,
    MeduimSlow,
    Slow,
    Fluctuating
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
    Fairy,
    Food,
    Sound
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
        /*                NOR FIR WAT ELE GRA ICE FIG POI GRO FLY PSY BUG ROC GHO DRG DAR STE FAI FOD  */
        /* Normal    */{
            1f,
            1f,
            1f,
            1f,
            1f,
            1f,
            2f,
            1f,
            1f,
            1f,
            1f,
            1f,
            0.5f,
            0f,
            1f,
            1f,
            0.5f,
            1f,
            2f
        },
        /* Fire      */{ 1f, 0.5f, 2f, 1f, 0.5f, 0.5f, 1f, 1f, 2f, 1f, 1f, 2f, 0.5f, 1f, 1f, 1f, 0.5f, 1f, 0.5f },
        /* Water     */{ 1f, 2f, 0.5f, 1f, 2f, 0.5f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 1f, 0.5f, 1f, 2f },
        /* Electric  */{ 1f, 1f, 2f, 0.5f, 0.5f, 1f, 1f, 1f, 0f, 2f, 1f, 1f, 1f, 1f, 1f, 1f, 0.5f, 1f, 0f },
        /* Grass     */{ 1f, 2f, 0.5f, 1f, 0.5f, 2f, 1f, 2f, 0.5f, 2f, 1f, 0.5f, 2f, 1f, 1f, 1f, 0.5f, 1f, 1f },
        /* Ice       */{ 1f, 2f, 1f, 1f, 2f, 0.5f, 1f, 1f, 2f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 0.5f, 1f, 0.5f },
        /* Fighting  */{ 2f, 1f, 1f, 1f, 1f, 2f, 1f, 0.5f, 1f, 0.5f, 0.5f, 0.5f, 2f, 0f, 1f, 2f, 2f, 0.5f, 1f },
        /* Poison    */{ 1f, 1f, 1f, 1f, 2f, 1f, 1f, 0.5f, 0.5f, 1f, 1f, 1f, 0.5f, 0.5f, 1f, 1f, 0f, 2f, 2f },
        /* Ground    */{ 1f, 1f, 1f, 2f, 0f, 2f, 1f, 0.5f, 2f, 1f, 1f, 0.5f, 2f, 1f, 1f, 1f, 1f, 1f, 1f },
        /* Flying    */{ 1f, 1f, 1f, 2f, 0.5f, 1f, 2f, 1f, 0f, 1f, 1f, 2f, 0.5f, 1f, 1f, 1f, 1f, 1f, 2f },
        /* Psychic   */{ 1f, 1f, 1f, 1f, 1f, 1f, 0.5f, 2f, 1f, 1f, 0.5f, 1f, 1f, 1f, 2f, 2f, 1f, 1f, 1f },
        /* Bug       */{ 1f, 0.5f, 1f, 1f, 2f, 1f, 0.5f, 1f, 0.5f, 2f, 1f, 2f, 1f, 0.5f, 2f, 1f, 0.5f, 0.5f, 1f },
        /* Rock      */{ 0.5f, 2f, 1f, 2f, 2f, 1f, 0.5f, 1f, 2f, 0.5f, 1f, 1f, 1f, 1f, 1f, 1f, 0.5f, 1f, 1f },
        /* Ghost     */{ 0f, 1f, 1f, 1f, 1f, 1f, 0f, 0.5f, 1f, 1f, 2f, 1f, 1f, 2f, 1f, 1f, 1f, 2f, 0.5f },
        /* Dragon    */{ 1f, 0.5f, 0.5f, 0.5f, 0.5f, 2f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 2f, 1f, 1f },
        /* Dark      */{ 1f, 1f, 1f, 1f, 1f, 1f, 0.5f, 1f, 1f, 1f, 0f, 2f, 1f, 2f, 1f, 0.5f, 1f, 0.5f, 1f },
        /* Steel     */{
            0.5f,
            2f,
            1f,
            0.5f,
            2f,
            1f,
            1f,
            1f,
            0.5f,
            2f,
            1f,
            0.5f,
            1f,
            0.5f,
            1f,
            0.5f,
            0.5f,
            2f,
            2f
        },
        /* Fairy     */{ 1f, 0.5f, 1f, 1f, 1f, 1f, 2f, 0.5f, 1f, 1f, 1f, 1f, 1f, 1f, 0f, 2f, 0.5f, 1f, 1f },
        /* Food      */{ 0.5f, 2f, 0.5f, 1f, 1f, 2f, 1f, 1f, 1f, 0.5f, 1f, 0.5f, 1f, 1f, 0.5f, 1f, 0.5f, 1f, 0f }
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
