using System.Collections.Generic;
using UnityEngine;

public enum OverworldCreatureAnimation : long
{
    Default = 0,
    Forward1 = 0,
    Forward2 = 1,
}

public class OverworldCreature : MonoBehaviour
{
    List<Sprite> sprites;
    SpriteRenderer spr;

    void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
    }

    public void Setup(List<Sprite> sprites)
    {
        this.sprites = sprites;
        spr.sprite = sprites[((int)OverworldCreatureAnimation.Default)];
    }
}
