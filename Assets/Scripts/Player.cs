using System.Collections;
using System.Linq.Expressions;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    Rigidbody2D body;
    public Camera mainCam;
    public Camera[] cameras;
    float horizontal;
    float vertical;
    float moveLimiter = 0.7f;
    public RoomSystem roomSys;
    private Animator anim;
    public float runSpeed = 20.0f;
    public GameObject notificationBar,
        battleSystem;
    string notification;
    Coroutine lastRoutine = null;
    public bool playerActive = true;
    public GameController gameController;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        roomSys = new RoomSystem(roomSys.startingRoom);
        body = GetComponent<Rigidbody2D>();
        roomSys.ChangeRoom(roomSys.startingRoom, this.gameObject);
        battleSystem.SetActive(false);
        this.roomSys.levelLoader = GameObject
            .FindGameObjectWithTag("LevelLoader")
            .GetComponent<LevelLoader>();
    }

    void Update()
    {
        if (active)
        {
            TV();
        }
        this.roomSys.levelLoader = GameObject
            .FindGameObjectWithTag("LevelLoader")
            .GetComponent<LevelLoader>();
        if (playerActive)
        {
            this.GetComponent<BoxCollider2D>().enabled = true;
            // Gives a value between -1 and 1
            horizontal = Input.GetAxisRaw("Horizontal"); // -1 is left
            vertical = Input.GetAxisRaw("Vertical"); // -1 is down
        }
        else
        {
            this.GetComponent<BoxCollider2D>().enabled = false;
            horizontal = 0;
            vertical = 0;
        }
        mainCam.transform.position = new Vector3(transform.position.x, transform.position.y, -0.4f);
        #region animation states
        /*
            0 => idle
            1 => backwards
            2 => forward
            3 => right
            4 => left
        */
        if (vertical == 1)
        {
            anim.enabled = true;
            anim.SetInteger("state", 2); // forward
        }
        else if (vertical == -1)
        {
            anim.enabled = true;
            anim.SetInteger("state", 1); // back
        }
        else if (horizontal == 1)
        {
            anim.enabled = true;
            anim.SetInteger("state", 3); // right
        }
        else if (horizontal == -1)
        {
            anim.enabled = true;
            anim.SetInteger("state", 4); // left
        }
        else
        {
            anim.enabled = false;
        }
        #endregion
    }

    void FixedUpdate()
    {
        if (horizontal != 0 && vertical != 0) // Check for diagonal movement
        {
            // limit movement speed diagonally, so you move at {moveLimiter}% speed
            horizontal *= moveLimiter;
            vertical *= moveLimiter;
        }
        body.velocity = new Vector2(horizontal * runSpeed, vertical * runSpeed);
    }
    bool active = false;

    #region Collision
    void OnCollisionStay2D(Collision2D other)
    {
        switch (other.gameObject.tag)
        {
            case "Door":
                if (Input.GetButtonDown("Action"))
                {
                    roomSys.ChangeRoom(
                        other.gameObject.GetComponent<Door>().targetRoom,
                        this.gameObject
                    );
                }
                break;
            case "TV":
                if (Input.GetButton("Action") || active)
                {
                    TV();
                }
                break;
        }
    }
    public void TV()
    {
        active = true;
        if (lastRoutine != null)
        {
            StopCoroutine(lastRoutine);
        }
        string timeOnly = DateTime.Now.ToString("h:mm:ss tt");
        lastRoutine = StartCoroutine(notify($"{timeOnly}"));
    }
    void OnCollisionExit2D(Collision2D other)
    {
        if (lastRoutine != null)
            StopCoroutine(lastRoutine);
        active = false;
        lastRoutine = StartCoroutine(removeNotification());
    }

    #endregion
    public IEnumerator notify(string notification)
    {
        this.notification = notification;
        this.notificationBar.GetComponentInChildren<TextMeshProUGUI>().text = notification;
        while (notificationBar.transform.localPosition.y < -400)
        {
            this.notificationBar.transform.localPosition += new Vector3(0, 50, 0);
            yield return new WaitForSeconds(0.05f);
        }
    }

    public IEnumerator removeNotification()
    {
        while (notificationBar.transform.localPosition.y > -720)
        {
            this.notificationBar.transform.localPosition -= new Vector3(0, 50, 0);
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void EnterBattle()
    {
        battleSystem.SetActive(true);
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        StartCoroutine(
            CheckForEncounters(
                other.gameObject.tag == "LongGrass" && (horizontal != 0 || vertical != 0)
            )
        );
        switch (other.gameObject.tag)
        {
            case "TV":
                if (Input.GetButton("Action") || active)
                {
                    TV();
                }
                break;
        }
    }

    public IEnumerator CheckForEncounters(bool touchingLongGrass)
    {
        int rnd = UnityEngine.Random.Range(1, 1000);
        if (touchingLongGrass)
        {
            if (rnd <= 10)
            {
                roomSys.levelLoader.Load(
                    new Room(transform.position, "Transition", this.gameObject)
                );

                yield return new WaitForSeconds(1);
                playerActive = false;
                battleSystem.SetActive(true);
                gameController.StartBattle();
            }
        }
    }
}
