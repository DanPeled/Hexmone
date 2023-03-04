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
}
