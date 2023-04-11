using UnityEngine;

[CreateAssetMenu(menuName ="Items/Create new evolution item")]
public class EvolutionItem : ItemBase
{
    public override bool Use(Creature creature)
    {
        // being handled from inv ui script
        return true;
    }
}