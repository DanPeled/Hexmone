using UnityEngine;
[CreateAssetMenu(menuName ="Items/Create new Recovery Item")]
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
}