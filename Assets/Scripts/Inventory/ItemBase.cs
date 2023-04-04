using UnityEngine;

public class ItemBase : ScriptableObject
{
    public string name;
    public string description;
    public Sprite icon;
    public virtual bool Use(Creature creature){
        return false;
    }
}