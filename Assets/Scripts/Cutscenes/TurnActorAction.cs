using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnActorAction : CutSceneAction
{
    public CutsceneActor actor;
    public Player.FacingDir dir;

    public override IEnumerator Play()
    {
        actor.GetCharacter();
        yield break;
    }
}
