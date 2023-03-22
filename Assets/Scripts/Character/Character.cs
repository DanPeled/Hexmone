using System.Collections;
using UnityEngine;

public class Character : MonoBehaviour
{
    public Rigidbody2D body;
    void Awake()
    {
        this.body = GetComponent<Rigidbody2D>();
    }
    public void Move(float horizontal, float vertical, float moveLimiter, float runSpeed)
    {
        if (horizontal != 0 && vertical != 0) // Check for diagonal movement
        {
            // limit movement speed diagonally, so you move at {moveLimiter}% speed
            horizontal *= moveLimiter;
            vertical *= moveLimiter;
        }
        body.velocity = new Vector2(horizontal * runSpeed, vertical * runSpeed);
    }
}