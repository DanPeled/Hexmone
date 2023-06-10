using UnityEngine;

[RequireComponent(typeof(NPCController))]
public class NPCMovement : MonoBehaviour
{
    bool gizmosDrawn = false;
    public int speed;
    private Transform target;
    private int wavepointIndex = 0;
    NPCController npcController;
    public Transform[] waypoints;
    public bool collidingPlayer;

    void Start()
    {
        npcController = GetComponent<NPCController>();
        target = waypoints[0];
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<Player>() != null)
            collidingPlayer = true;
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<Player>() != null)
            collidingPlayer = false;
    }

    void Update()
    {
        Vector3 dir = target.position - transform.position;
        if (!collidingPlayer)
        {
            transform.Translate(dir.normalized * Time.deltaTime * speed, Space.World);
            this.GetComponentInChildren<SpriteRenderer>().flipX =
                target.position.x < transform.position.x;
            npcController.spriteAnimator.HandleUpdate();
        }
        else
            npcController.spriteAnimator.SetFrame(0);
        if (Vector3.Distance(transform.position, target.position) <= 0.2f)
        {
            GetNextWaypoint();
        }
    }

    void GetNextWaypoint()
    {
        if (wavepointIndex >= waypoints.Length - 1)
        {
            EndPath();
            return;
        }
        wavepointIndex++;
        target = waypoints[wavepointIndex];
    }

    void EndPath()
    {
        wavepointIndex = 0;
        target = waypoints[wavepointIndex];
    }

    void OnDrawGizmosSelected()
    {
        if (gizmosDrawn)
        {
            return;
        }
        Gizmos.color = Color.yellow;
        for (int waypoint = 0; waypoint < waypoints.Length - 1; waypoint++)
        {
            Gizmos.DrawLine(
                waypoints[waypoint].transform.position,
                waypoints[waypoint + 1].transform.position
            );
        }
        gizmosDrawn = true;
    }
}
