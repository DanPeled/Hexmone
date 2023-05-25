using System;
using System.Runtime.ExceptionServices;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Creature
{
    public string nickname = "";
    public CreatureBase _base;
    public int level,
        exp;
    public int HP,
        statusTime;
    public List<Move> moves;
    public Move currentMove;
    public Dictionary<Stat, int> stats;
    public Dictionary<Stat, int> statBoosts;
    public Condition status;
    public Queue<string> statusChanges;
    public event Action OnStatusChanged;
    public event Action OnHPChanged;
    public Condition volatileStatus;
    public int volatileStatusTime;
    public bool repainted;

    public Creature(CreatureBase base_, int pLvl)
    {
        this._base = base_;
        level = pLvl;
        Init();
    }

    public Creature(CreatureBase base_, int pLvl, bool repainted)
    {
        this._base = base_;
        level = pLvl;
        this.repainted = repainted;
        Init();
    }

    public void Init()
    {
        moves = new List<Move>();
        // Generate Moves
        foreach (var move in _base.learnableMoves)
        {
            if (move.level <= level && move != null)
            {
                moves.Add(new Move(move.moveBase));
            }
            if (moves.Count >= _base.maxNumberOfMoves)
            {
                break;
            }
        }
        exp = _base.GetExpForLevel(level);
        CalculateStats();
        this.HP = this.maxHealth;
        statusChanges = new Queue<string>();
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
        int oldHP = maxHealth;
        this.maxHealth = Mathf.FloorToInt((this._base.maxHealth * level) / 100f + 10);
        if (oldHP != 0)
            HP += maxHealth - oldHP;
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
            { Stat.Accuracy, 0 },
            { Stat.Evasion, 0 },
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

            statusChanges.Enqueue($"{GetName()}'s {stat} {change}!");

            Debug.Log($"{stat} Has been boosted to {this.statBoosts[stat]}");
        }
    }

    public string GetName()
    {
        return nickname != "" ? nickname : _base.Name;
    }

    public void SetStatus(ConditionID conditionID)
    {
        if (status != null)
            return;

        status = ConditionDB.conditions[conditionID];
        status?.onStart?.Invoke(this);
        statusChanges.Enqueue($"{GetName()} {status.startMessage}");

        OnStatusChanged?.Invoke();
    }

    public void CureStatus()
    {
        status = null;
        OnStatusChanged?.Invoke();
    }

    public void SetVolatileStatus(ConditionID conditionID)
    {
        if (volatileStatus != null)
            return;

        volatileStatus = ConditionDB.conditions[conditionID];
        volatileStatus?.onStart?.Invoke(this);
        statusChanges.Enqueue($"{GetName()} {volatileStatus.startMessage}");
    }

    public void CureVolatileStatus()
    {
        volatileStatus = null;
    }

    public bool CheckForLevelUp()
    {
        if (exp > _base.GetExpForLevel(level + 1))
        {
            level++;
            CalculateStats();
            return true;
        }
        return false;
    }

    public LearnableMove GetLearnableMoveAtCurrLevel()
    {
        return _base.learnableMoves.Where(x => x.level == this.level).FirstOrDefault();
    }

    public void LearnMove(MoveBase moveToLearn)
    {
        if (moves.Count > this._base.maxNumberOfMoves)
            return;
        moves.Add(new Move(moveToLearn));
    }

    public bool HasMove(MoveBase move)
    {
        return this.moves.Count(m => m.base_ == move) > 0;
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

        DecreaseHP(damage);
        return damageDetails;
    }

    public Move GetRandomMove()
    {
        var movesWithPP = moves.Where(x => x.PP > 0).ToList();
        return movesWithPP[UnityEngine.Random.Range(0, movesWithPP.Count)];
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

    public void IncreaseHP(int amount)
    {
        HP = Mathf.Clamp(HP + amount, 0, maxHealth);
        OnHPChanged?.Invoke();
    }

    public void DecreaseHP(int damage)
    {
        HP = Mathf.Clamp(HP - damage, 0, maxHealth);
        OnHPChanged?.Invoke();
    }

    public void OnBattleOver()
    {
        volatileStatus = null;
        ResetStatBoost();
    }

    public Creature(CreatureSaveData saveData)
    {
        _base = CreatureDB.GetObjectByName(saveData.name);
        this.HP = saveData.hp;
        this.level = saveData.level;
        this.exp = saveData.exp;
        this.nickname = saveData.nickname;
        if (saveData.statusId != null)
        {
            status = ConditionDB.conditions[saveData.statusId.Value];
        }
        else
        {
            status = null;
        }

        this.moves = saveData.moves.Select(s => new Move(s)).ToList();

        CalculateStats();
        statusChanges = new Queue<string>();
        ResetStatBoost();
        volatileStatus = null;
    }

    public CreatureSaveData GetSaveData()
    {
        var saveData = new CreatureSaveData()
        {
            nickname = this.nickname,
            name = this._base.Name,
            hp = this.HP,
            level = this.level,
            exp = this.exp,
            statusId = status?.iD,
            moves = moves.Select(p => p.GetSaveData()).ToList()
        };
        return saveData;
    }

    public Evolution CheckForEvolution()
    {
        return _base.evolutions.FirstOrDefault(e => e.requiredLevel <= level);
    }

    public Evolution CheckForEvolution(ItemBase item)
    {
        return _base.evolutions.FirstOrDefault(e => e.requiredItem == item);
    }

    public void Evolve(Evolution evolution)
    {
        _base = evolution.evolvesInto;
        CalculateStats();
    }

    public void Heal()
    {
        HP = maxHealth;
        CureStatus();
        OnHPChanged?.Invoke();
    }

    public Sprite GetFrontSprite()
    {
        return repainted ? _base.repaintedFrontSprite : _base.frontSprite;
    }

    public Sprite GetBackSprite()
    {
        return repainted ? _base.repaintedBackSprite : _base.backSprite;
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }
}

[System.Serializable]
public class CreatureSaveData
{
    public string name,
        nickname;
    public int hp,
        level,
        exp;
    public ConditionID? statusId;
    public List<MoveSaveData> moves;
}
