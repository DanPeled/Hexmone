using UnityEngine;

public class ItemBase : ScriptableObject
{
    public string name;
    public string description;
    public Sprite icon;
    public virtual string Name => name;
    public virtual bool Use(Creature creature){
        return false;
    }
    public virtual bool isReusable => false;
    public virtual bool CanUseInBattle  => true;
    public virtual bool CanUseOutsideBattle => true;

}