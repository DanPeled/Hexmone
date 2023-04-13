using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MoveActorAction : CutSceneAction
{
    public CutsceneActor actor;
    public List<Vector2> movePatterns;

    public override IEnumerator Play()
    {
        var charac = actor.GetCharacter();
        foreach (var pattern in movePatterns)
        {
            yield return charac.Move(pattern);
        }
    }
}
[System.Serializable]
public class CutsceneActor
{
    public bool isPlayer;
    public Character character;

    public Character GetCharacter() => isPlayer ? null : character;
}