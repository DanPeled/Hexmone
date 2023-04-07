using UnityEngine;

[CreateAssetMenu(menuName ="Items/Create new TM or HM")]
public class TMItem : ItemBase{
    public MoveBase move;
    public bool isHM;
    public override string Name => base.Name + $": {move.moveName}";

    public override bool Use(Creature creature){
        // Learning move is handled from inventory ui, if it was learned return true
        return creature.HasMove(move);
    }
    public bool CanBeTaught(Creature creature){
        return creature._base.learnableByItems.Contains(this.move);
    }
    public override bool isReusable => isHM;
    public override bool CanUseInBattle => false;
}