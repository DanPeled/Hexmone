using UnityEngine;
[CreateAssetMenu(menuName = "Items/Create new hexoball")]
public class HexoballItem : ItemBase
{
    public float catchRateModifier = 1;
    public override bool Use(Creature creature)
    {
        return true;
    }
    public override bool CanUseOutsideBattle => false;
}