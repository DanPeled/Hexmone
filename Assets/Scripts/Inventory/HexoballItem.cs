using UnityEngine;
[CreateAssetMenu(menuName ="Items/Create new hexoball")]
public class HexoballItem : ItemBase {
    public override bool Use(Creature creature)
    {

        return true;
    }
}