using System.Collections;
using UnityEngine;

public class Character : MonoBehaviour
{
    public Rigidbody2D body;
    void Awake()
    {
        this.body = GetComponent<Rigidbody2D>();
    }
    public IEnumerator Move(Vector2 moveVec)
    {
        var targetPos = transform.position;
        targetPos.x += moveVec.x;
        targetPos.y += moveVec.y;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, 5f * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;

    }
}