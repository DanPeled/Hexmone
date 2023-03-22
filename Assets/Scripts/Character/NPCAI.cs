using UnityEngine;
using Pathfinding;
public class NPCAI : MonoBehaviour
{
    public Transform target;
    public CharacterAnimator anim;
    public float speed = 200f;
    public float nextWayPointDistance = 7f;
    Path path;
    int currentWayPoint = 0;
    bool reachedEndOfPath = false;
    Seeker seeker;
    Rigidbody2D rb;
    public Vector2[] paths;
    public int currentPath;
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        InvokeRepeating("UpdatePath", 0f, .5f);
        speed = speed * 100000;
        for(int i = 0; i < paths.Length; i++){
            paths[i] += (Vector2)transform.position;
        }
    }
    void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(rb.position, paths[currentPath], OnPathComplete);
        }
    }
    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWayPoint = 0;
            if (currentPath + 1 < paths.Length){
                currentPath ++;
            } else currentPath = 0;
        }
    }
    void FixedUpdate()
    {
        if (path == null)
        {
            return;
        }
        if (currentWayPoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }
        Vector2 direction = ((Vector2)path.vectorPath[currentWayPoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;
        anim.moveX = force.x;
        anim.moveY = force.y;
        rb.AddForce(force);
        if (force != Vector2.zero)
        {
            anim.isMoving = true;
        }
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWayPoint]);

        if (distance < nextWayPointDistance)
        {
            currentWayPoint++;
        }
    }
}