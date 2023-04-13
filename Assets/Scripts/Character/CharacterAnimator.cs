using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    public List<Sprite> walkDownSprites, walkUpSprites, walkLeftSprites, walkRightSprites, surfingSprites;
    // Prameters
    public float moveX, moveY;
    public bool isMoving, wasPrevioslyMoving, isSurfing;

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
        if (!isSurfing){
        if (moveX > 0)
        {
            currentAnim = walkRightAnim;
            moveY = 0;
        }
        else if (moveX < 0)
        {
            currentAnim = walkLeftAnim;
            moveY = 0;
        }
        else if (moveY > 0)
        {
            currentAnim = walkUpAnim;
            moveX = 0;
        }
        else if (moveY < 0)
        {
            currentAnim = walkDownAnim;
            moveX = 0;
        }
        
        if (currentAnim != prevAnim)
        {
            currentAnim.Start();
        }
        currentAnim.HandleUpdate();
        if (!isMoving)
        {
            spriteRenderer.sprite = currentAnim.frames[0];
        }}
        else {
            if (moveX > 0){
                spriteRenderer.sprite = surfingSprites[2];
            } else if (moveX < 0){
                spriteRenderer.sprite = surfingSprites[3];
            } else if (moveY > 0){
                spriteRenderer.sprite = surfingSprites[1];
            } else if (moveY < 0){
                spriteRenderer.sprite = surfingSprites[0];
            }
        }
        wasPrevioslyMoving = isMoving;
    }

}