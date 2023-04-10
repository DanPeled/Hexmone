using UnityEngine;

public class Move
{
    public MoveBase base_ { get; set; }
    public int PP { get; set; }

    public Move(MoveBase pBase)
    {
        this.base_ = pBase;
        this.PP = pBase.pp;
    }
    public Move(MoveSaveData saveData)
    {
        base_ = MovesDB.GetObjectByName(saveData.name);
        PP = saveData.pp;
    }
    public MoveSaveData GetSaveData()
    {
        var saveData = new MoveSaveData()
        {
            name = base_.name,
            pp = PP
        };
        return saveData;
    }
    public void IncreasePP(int amount)
    {
        this.PP = Mathf.Clamp(this.PP + amount, 0, base_.pp);
    }
}

[System.Serializable]
public class MoveSaveData
{
    public string name;
    public int pp;
}