using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    public List<Sprite> walkDownSprites, walkUpSprites, walkLeftSprites, walkRightSprites;
    // Prameters
    public float moveX, moveY;
    public bool isMoving, wasPrevioslyMoving;

    // States
    SpriteAnimator walkDownAnim,
     walkUpAnim,
     walkLeftAnim,
     walkRightAnim, currentAnim;

    // Refrences
    SpriteRenderer spriteRenderer;
    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        walkUpAnim = new SpriteAnimator(walkUpSprites, spriteRenderer);
        walkLeftAnim = new SpriteAnimator(walkLeftSprites, spriteRenderer);
        walkRightAnim = new SpriteAnimator(walkRightSprites, spriteRenderer);
        walkDownAnim = new SpriteAnimator(walkDownSprites, spriteRenderer);


        currentAnim = walkDownAnim;
    }

    void Update()
    {
        var prevAnim = currentAnim;
        if (moveX == 1)
        {
            currentAnim = walkRightAnim;
        }
        else if (moveX == -1)
        {
            currentAnim = walkLeftAnim;
        }
        else if (moveY == 1)
        {
            currentAnim = walkUpAnim;
        }
        else if (moveY == -1)
        {
            currentAnim = walkDownAnim;
        }
        if (currentAnim != prevAnim)
        {
            currentAnim.Start();
        }
        currentAnim.HandleUpdate();
        if (!isMoving)
        {
            spriteRenderer.sprite = currentAnim.frames[0];
        }
        wasPrevioslyMoving = isMoving;
    }

}