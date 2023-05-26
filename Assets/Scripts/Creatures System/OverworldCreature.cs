using System.Collections.Generic;
using UnityEngine;
using System.Collections;

// public enum OverworldCreatureAnimation
// {
//     Default = 0,
//     Forward1 = 1,
//     Forward2 = 2,
//     Backward1 = 3,
//     Backward2 = 4,
//     Left1 = 5,
//     Left2 = 6,
//     Right1 = 7,
//     Right2 = 8
// }

public class OverworldCreature : MonoBehaviour
{
    // private List<Sprite>[] animations;
    // private SpriteRenderer spriteRenderer;
    // private OverworldCreatureAnimation currentAnimation;

    // void Awake()
    // {
    //     spriteRenderer = GetComponent<SpriteRenderer>();
    //     animations = new List<Sprite>[9];
    // }

    // public void Setup(List<Sprite>[] animationSprites)
    // {
    //     animations = animationSprites;
    //     currentAnimation = OverworldCreatureAnimation.Default;
    //     spriteRenderer.sprite = animations[(int)currentAnimation][0];
    // }

    // public void PlayAnimation(OverworldCreatureAnimation animation, bool loop = true)
    // {
    //     if (currentAnimation == animation) return;

    //     currentAnimation = animation;
    //     StopAllCoroutines();
    //     StartCoroutine(Animate(loop));
    // }

    // IEnumerator Animate(bool loop)
    // {
    //     int frameIndex = 0;

    //     while (true)
    //     {
    //         spriteRenderer.sprite = animations[(int)currentAnimation][frameIndex];

    //         yield return new WaitForSeconds(0.1f); // Change this value to adjust animation speed

    //         frameIndex++;

    //         if (frameIndex >= animations[(int)currentAnimation].Count)
    //         {
    //             if (loop)
    //             {
    //                 frameIndex = 0;
    //             }
    //             else
    //             {
    //                 currentAnimation = OverworldCreatureAnimation.Default;
    //                 spriteRenderer.sprite = animations[(int)currentAnimation][0];
    //                 break;
    //             }
    //         }
    //     }
    // }
}
