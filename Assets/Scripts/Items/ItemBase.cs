using UnityEngine;
[System.Serializable]
public class ItemBase : ScriptableObject
{
    public string name;
    public string description;
    public Sprite icon;
    public float price;
    public bool isSellable;
    public virtual string Name => name;
    public virtual bool Use(Creature creature){
        return false;
    }
    public virtual bool isReusable => false;
    public virtual bool CanUseInBattle  => true;
    public virtual bool CanUseOutsideBattle => true;

}