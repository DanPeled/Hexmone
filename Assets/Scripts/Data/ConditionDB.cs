using System;
using System.Collections.Generic;
using UnityEngine;

public class ConditionDB
{
    public static void Init()
    {
        foreach (var kvp in conditions)
        {
            var conditionID = kvp.Key;
            var condition = kvp.Value;

            condition.iD = conditionID;
        }
    }

    public static Dictionary<ConditionID, Condition> conditions { get; set; } =
        new Dictionary<ConditionID, Condition>()
        {
            {
                ConditionID.psn,
                new Condition()
                {
                    name = "Poison",
                    startMessage = "has been poisoned",
                    onAfterTurn = (Creature creature) =>
                    {
                        creature.UpdateHP(creature.maxHealth / 8);
                        creature.statusChanges.Enqueue(
                            $"{creature._base.creatureName} hurt itself due to poison"
                        );
                    }
                }
            },
            {
                ConditionID.brn,
                new Condition()
                {
                    name = "Burn",
                    startMessage = "has been burned",
                    onAfterTurn = (Creature creature) =>
                    {
                        creature.UpdateHP(creature.maxHealth / 16);
                        creature.statusChanges.Enqueue(
                            $"{creature._base.creatureName} hurt itself due to burn"
                        );
                    }
                }
            },
            {
                ConditionID.par,
                new Condition()
                {
                    name = "Paralyzed",
                    startMessage = "has been paralyzed",
                    onBeforeMove = (Creature creature) =>
                    {
                        if (UnityEngine.Random.Range(1, 5) == 1)
                        {
                            creature.statusChanges.Enqueue(
                                $"{creature._base.creatureName}'s paralyzed and can't move"
                            );
                            return false;
                        }
                        return true;
                    }
                }
            },
            {
                ConditionID.frz,
                new Condition()
                {
                    name = "Freeze",
                    startMessage = "has been forzen",
                    onBeforeMove = (Creature creature) =>
                    {
                        if (UnityEngine.Random.Range(1, 5) == 1)
                        {
                            creature.CureStatus();
                            creature.statusChanges.Enqueue(
                                $"{creature._base.creatureName}'s not frozen anymore"
                            );
                            return true;
                        }
                        return false;
                    }
                }
            },
            {
                ConditionID.slp,
                new Condition()
                {
                    name = "Sleep",
                    startMessage = "has fallen asleep",
                    onStart = (Creature creature) =>
                    {
                        // sleep for 1-3 turnes
                        creature.statusTime = UnityEngine.Random.Range(1, 4);
                        Debug.Log($"Will be asleep for {creature.statusTime} moves");
                    },
                    onBeforeMove = (Creature creature) =>
                    {
                        if (creature.statusTime <= 0)
                        {
                            creature.CureStatus();
                            creature.statusChanges.Enqueue(
                                $"{creature._base.creatureName} woke up!"
                            );
                            return true;
                        }
                        creature.statusTime--;
                        creature.statusChanges.Enqueue(
                            $"{creature._base.creatureName} is sleeping"
                        );
                        return false;
                    }
                }
            },
            // Volatile Status Conditions
            {
                ConditionID.confusion,
                new Condition()
                {
                    name = "Confusion",
                    startMessage = "has been confused",
                    onStart = (Creature creature) =>
                    {
                        // sleep for 1 - 4 turnes
                        creature.volatileStatusTime = UnityEngine.Random.Range(1, 5);
                        Debug.Log($"Will be confused for {creature.volatileStatusTime} moves");
                    },
                    onBeforeMove = (Creature creature) =>
                    {
                        if (creature.volatileStatusTime <= 0)
                        {
                            creature.CureVolatileStatus();
                            creature.statusChanges.Enqueue(
                                $"{creature._base.creatureName} kicked out of confusion!"
                            );
                            return true;
                        }
                        creature.volatileStatusTime--;
                        // 50% To perform move
                        if (UnityEngine.Random.Range(1, 3) == 1)
                        {
                            return true;
                        }
                        // Hurt By Confusion
                        creature.statusChanges.Enqueue(
                            $"{creature._base.creatureName} is confused"
                        );
                        creature.UpdateHP(creature.maxHealth / 8);
                        creature.statusChanges.Enqueue($"It hurt itself due to confusion");
                        return false;
                    }
                }
            }
        };
        public static float GetStatusBonus(Condition condition){
            if (condition == null){
                return 1f;
            } else if (condition.iD == ConditionID.slp || condition.iD == ConditionID.frz){
                return 2f;
            } else if (condition.iD == ConditionID.par || condition.iD == ConditionID.psn || condition.iD == ConditionID.brn){
                return 1.5f;
            }
            return 1f;
        }
}

public enum ConditionID
{
    none,
    psn,
    brn,
    slp,
    par,
    frz,
    confusion
}
