using UnityEngine;
[CreateAssetMenu(menuName = "Items/Create new Recovery Item")]
class RecoveryItem : ItemBase
{
    [Header("HP")]
    public int hpAmount;
    public bool restoreMaxHp;

    [Header("PP")]
    public int ppAmount;
    public bool restoreMaxPP;

    [Header("Status Conditions")]
    public ConditionID status;
    public bool recoverAllStatus;

    [Header("Revive")]
    public bool revive;
    public bool maxRevive;

    public override bool Use(Creature creature)
    {
        // Revive
        if (revive || maxRevive)
        {
            if (creature.HP > 0)
            {
                return false;
            }
            if (revive)
            {
                creature.IncreaseHP(creature.maxHealth / 2);
            }
            else if (maxRevive)
            {
                creature.IncreaseHP(creature.maxHealth);
            }
            creature.CureStatus();

            return true;
        }
        // no other items (than revive) can be used on a fainted creature
        if (creature.HP <= 0)
        {
            return false;
        }
        // Restore HP
        if (restoreMaxHp || hpAmount > 0)
        {
            if (creature.HP == creature.maxHealth)
            {
                return false;
            }
            if (restoreMaxHp)
            {
                creature.IncreaseHP(creature.maxHealth);
            }
            else
            {
                creature.IncreaseHP(hpAmount);
            }
        }

        // Recover status
        if (recoverAllStatus || status != ConditionID.none)
        {
            if (creature.status == null && creature.volatileStatus == null)
            {
                return false;
            }
            if (recoverAllStatus)
            {
                creature.CureStatus();
                creature.CureVolatileStatus();
            }
            else
            {
                if (creature.status.iD == status)
                {
                    creature.CureStatus();
                }
                else if (creature.volatileStatus.iD == status)
                {
                    creature.CureVolatileStatus();
                }
                else
                {
                    return false;
                }
            }
        }

        //Restore PP
        if (restoreMaxPP)
        {
            creature.moves.ForEach(m => m.IncreasePP(m.base_.pp));
        }
        else if (ppAmount > 0)
        {
            creature.moves.ForEach(m => m.IncreasePP(ppAmount));
        }

        return true;
    }
}