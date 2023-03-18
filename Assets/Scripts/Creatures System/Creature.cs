using System;
using System.Runtime.ExceptionServices;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Creature
{
    public CreatureBase _base;
    public int level;
    public int HP, statusTime;
    public bool HPChanged { get; set; }
    public List<Move> moves = new List<Move>();
    public Move currentMove;
    public Dictionary<Stat, int> stats;
    public Dictionary<Stat, int> statBoosts;
    public Condition status;
    public Queue<string> statusChanges = new Queue<string>();
    public Action OnStatusChanged;
    public Condition volatileStatus;
    public int volatileStatusTime;

    public void Init()
    {
        // Generate Moves
        foreach (var move in _base.learnableMoves)
        {
            if (move.level <= level)
            {
                moves.Add(new Move(move.moveBase));
            }
            if (moves.Count >= 4)
            {
                break;
            }
        }
        CalculateStats();
        this.HP = this.maxHealth;

        ResetStatBoost();
        status = null;
        volatileStatus = null;
    }

    public void CalculateStats()
    {
        stats = new Dictionary<Stat, int>();
        stats.Add(Stat.Attack, Mathf.FloorToInt((this._base.damage * level) / 100f) + 5);
        stats.Add(Stat.Defense, Mathf.FloorToInt((this._base.defense * level) / 100f) + 5);
        stats.Add(Stat.SpAttack, Mathf.FloorToInt((this._base.spAttack * level) / 100f) + 5);
        stats.Add(Stat.SpDefense, Mathf.FloorToInt((this._base.spDefense * level) / 100f) + 5);
        stats.Add(Stat.Speed, Mathf.FloorToInt((this._base.speed * level) / 100f) + 5);

        this.maxHealth = Mathf.FloorToInt((this._base.maxHealth * level) / 100f + 10);
    }
    void ResetStatBoost()
    {
        this.statBoosts = new Dictionary<Stat, int>()
        {
            { Stat.Attack, 0 },
            { Stat.Defense, 0 },
            { Stat.SpAttack, 0 },
            { Stat.SpDefense, 0 },
            { Stat.Speed, 0 },
            {Stat.Accuracy, 0},
            {Stat.Evasion, 0},
        };
    }
    public int GetStat(Stat stat)
    {
        int statVal = stats[stat];

        //Apply stat boost
        int boost = statBoosts[stat];
        var boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        if (boost >= 0)
        {
            statVal = Mathf.FloorToInt(statVal * boostValues[boost]);
        }
        else
        {
            statVal *= Mathf.FloorToInt(statVal / boostValues[-boost]);
        }

        return statVal;
    }

    public void ApplyBoosts(List<StatBoost> statBoosts)
    {
        foreach (var statBoost in statBoosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            this.statBoosts[stat] = Mathf.Clamp(this.statBoosts[stat] + boost, -6, 6);
            string change = boost > 0 ? "rose" : "fell";

            statusChanges.Enqueue($"{_base.creatureName}'s {stat} {change}!");

            Debug.Log($"{stat} Has been boosted to {this.statBoosts[stat]}");
        }
    }
    public void SetStatus(ConditionID conditionID)
    {
        if (status != null) return;

        status = ConditionDB.conditions[conditionID];
        status?.onStart?.Invoke(this);
        statusChanges.Enqueue($"{_base.creatureName} {status.startMessage}");

        OnStatusChanged?.Invoke();
    }
    public void CureStatus()
    {
        status = null;
        OnStatusChanged?.Invoke();
    }
    public void SetVolatileStatus(ConditionID conditionID)
    {
        if (volatileStatus != null) return;

        volatileStatus = ConditionDB.conditions[conditionID];
        volatileStatus?.onStart?.Invoke(this);
        statusChanges.Enqueue($"{_base.creatureName} {volatileStatus.startMessage}");
    }
    public void CureVolatileStatus()
    {
        volatileStatus = null;
    }
    public int Attack
    {
        get { return GetStat(Stat.Attack); }
    }
    public int Defense
    {
        get { return GetStat(Stat.Defense); }
    }
    public int SpAttack
    {
        get { return GetStat(Stat.SpAttack); }
    }
    public int SpDefense
    {
        get { return GetStat(Stat.SpDefense); }
    }
    public int Speed
    {
        get { return GetStat(Stat.Speed); }
    }
    public int maxHealth { get; private set; }

    public DamageDetails TakeDamage(Move move, Creature attacker)
    {
        float critical = 1f;
        if (UnityEngine.Random.value * 100f <= 6.25f)
        {
            critical = 2f;
        }
        float type =
            TypeChart.GetAffectiveness(move.base_.type, this._base.type1)
            * TypeChart.GetAffectiveness(move.base_.type, this._base.type2);
        var damageDetails = new DamageDetails()
        {
            TypeEffectiveness = type,
            Critical = critical,
            Fainted = false
        };
        float attack =
            (move.base_.category == MoveCategory.Special) ? attacker.SpAttack : attacker.Attack;
        float defense = (move.base_.category == MoveCategory.Special) ? SpDefense : Defense;
        float modifiers = UnityEngine.Random.Range(0.85f, 1f) * type * critical;
        float a = (2 * attacker.level + 10) / 250f;
        float d = a * move.base_.power * ((float)attack / defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        UpdateHP(damage);
        return damageDetails;
    }

    public Move GetRandomMove()
    {
        return moves[UnityEngine.Random.Range(0, moves.Count)];
    }
    public void OnAfterTurn()
    {
        status?.onAfterTurn?.Invoke(this);
        volatileStatus?.onAfterTurn?.Invoke(this);
    }
    public bool OnBeforeMove()
    {
        bool canPreformMove = true;
        if (status?.onBeforeMove != null)
        {
            if (!status.onBeforeMove(this))
            {
                canPreformMove = false;
            }
        }
        if (volatileStatus?.onBeforeMove != null)
        {
            if (!volatileStatus.onBeforeMove(this))
            {
                canPreformMove = false;
            }
        }
        return canPreformMove;
    }
    public void UpdateHP(int damage)
    {
        HP = Mathf.Clamp(HP - damage, 0, maxHealth);
        HPChanged = true;
    }
    public void OnBattleOver()
    {
        volatileStatus = null;
        ResetStatBoost();
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }
}
