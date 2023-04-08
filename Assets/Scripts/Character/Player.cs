using System.Collections.Generic;
using System.Collections;
using System.Linq.Expressions;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using System.Linq;

public class Player : MonoBehaviour, ISavable
{
    [Header("Vars")]
    public string playerName;
    public float runSpeed = 20.0f;
    public enum FacingDir
    {
        forward,
        backward,
        left,
        right
    };
    public bool isMoving;
    FacingDir facingDir;
    public static Player instance;
    float horizontal;
    float vertical;
    float moveLimiter = 0.7f;
    public bool colldingDoor, collidingInteractable;
    public int camIndex;
    string notification;

    Door doorObject;
    GameObject interactObject;
    Coroutine lastRoutine = null;
    public bool playerActive = true;
    public static Rect viewPort;

    [Header("Refrences")]
    public GameController gameController;
    public GameObject notificationBar,
           battleSystem;
    public Sprite sprite;
    public RoomSystem roomSys;
    private CharacterAnimator anim;
    Rigidbody2D body;

    [Header("Cameras")]
    public List<Camera> cameras;
    public Camera mainCam;

    void Awake()
    {
        mainCam = Camera.main;
        anim = GetComponent<CharacterAnimator>();
        battleSystem = FindObjectOfType<BattleSystem>().gameObject;
        gameController = FindObjectOfType<GameController>();
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
        notificationBar.transform.parent.gameObject.GetComponent<Canvas>().worldCamera = cameras[camIndex];
        instance = this;
        if (colldingDoor && InputSystem.instance.action.isClicked())
        {
            Door(doorObject.gameObject);
        }
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
            horizontal = InputSystem.instance.right.isPressed() ? 1 : (InputSystem.instance.left.isPressed() ? -1 : 0);  // -1 is left
            vertical = InputSystem.instance.up.isPressed() ? 1 : (InputSystem.instance.down.isPressed() ? -1 : 0); // -1 is down
        }
        else
        {
            this.GetComponent<BoxCollider2D>().enabled = false;
            horizontal = 0;
            vertical = 0;
        }
        mainCam.transform.position = new Vector3(transform.position.x, transform.position.y, -0.4f);
        #region animation states
        if (vertical > 0)
        {
            isMoving = true;

            anim.moveY = 1; // forward
            anim.moveX = 0;
            this.facingDir = FacingDir.forward;
        }
        else if (vertical < 0)
        {
            isMoving = true;
            anim.moveX = 0;
            anim.moveY = -1;// back
            this.facingDir = FacingDir.backward;
        }
        else if (horizontal > 0)
        {
            isMoving = true;
            anim.moveY = 0;
            anim.moveX = -1;// right
            this.facingDir = FacingDir.right;
        }
        else if (horizontal < 0)
        {
            isMoving = true;

            anim.moveX = 1;// left
            anim.moveY = 0;
            this.facingDir = FacingDir.left;
        }
        else
        {
            isMoving = false;
        }
        #endregion
        anim.isMoving = isMoving;
        if (InputSystem.instance.action.isClicked())
        {
            ShowDialog();
            StartCoroutine(Interact(interactObject));
        }
    }
    IEnumerator Interact(GameObject obj)
    {
        if (collidingInteractable)
        {
            yield return obj.GetComponent<Interactable>()?.Interact(transform);
        }
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
            case "TV":
                if (InputSystem.instance.action.isClicked() || active)
                {
                    TV();
                }
                break;
        }
    }
    public void Door(GameObject other)
    {
        roomSys.ChangeRoom(
            other.GetComponent<Door>().targetRoom,
            this.gameObject
        );
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
        if (other.gameObject.GetComponent<Interactable>() != null)
            lastRoutine = StartCoroutine(removeNotification());
    }

    #endregion
    public IEnumerator notify(string notification)
    {
        this.notification = notification;
        this.notificationBar.GetComponentInChildren<TextMeshProUGUI>().text = notification;
        while (notificationBar.transform.localPosition.y < -350)
        {
            this.notificationBar.transform.localPosition += new Vector3(0, 50, 0);
            yield return new WaitForSeconds(0.05f);
        }
    }
    public IEnumerator notify()
    {
        while (notificationBar.transform.localPosition.y < -130)
        {
            var rectTransform = notificationBar.GetComponent<RectTransform>();
            rectTransform.localPosition += new Vector3(0, 50, 0);
            yield return new WaitForSeconds(0.05f);
        }
    }
    public void ShowDialog()
    {
        var rectTransform = notificationBar.GetComponent<RectTransform>();
        rectTransform.localPosition = new Vector3(notificationBar.transform.localPosition.x, -130, 0);
    }

    public IEnumerator removeNotification()
    {
        var rectTransform = notificationBar.GetComponent<RectTransform>();

        while (rectTransform.localPosition.y > -720)
        {
            rectTransform.localPosition -= new Vector3(0, 50, 0);
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void EnterBattle()
    {
        battleSystem.SetActive(true);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Door")
        {
            this.colldingDoor = true;
            this.doorObject = other.GetComponent<Door>();
        }
        if (other.gameObject.GetComponent<Interactable>() != null)
        {
            this.collidingInteractable = true;
            this.interactObject = other.gameObject;
        }
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<Interactable>() != null)
        {
            this.collidingInteractable = true;
            this.interactObject = other.gameObject;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Door")
        {
            this.colldingDoor = false;
        }
        if (other.gameObject.GetComponent<Interactable>() != null)
        {
            this.collidingInteractable = false;
            lastRoutine = StartCoroutine(removeNotification());
        }
    }
    public void OnTriggerStay2D(Collider2D other)
    {
        OnMoveOver(other.gameObject);
        switch (other.gameObject.tag)
        {
            case "Door":
                if (InputSystem.instance.action.isClicked())
                {
                    Door(other.gameObject);
                }
                break;
            case "TV":
                if (InputSystem.instance.action.isClicked() || active)
                {
                    TV();
                }
                break;
        }
    }
    public void OnMoveOver(GameObject other)
    {
        StartCoroutine(
            CheckForEncounters(
                other.tag == "LongGrass" && (horizontal != 0 || vertical != 0)
            )
        );
        StartCoroutine(CheckIfInTrainersView(other.gameObject.GetComponentInParent<TrainerController>(), other.tag.Equals("FOV")));
    }
    public IEnumerator CheckForEncounters(bool touchingLongGrass)
    {
        int rnd = UnityEngine.Random.Range(1, 1000);
        if (touchingLongGrass)
        {
            if (rnd <= 10 && playerActive)
            {
                playerActive = false;
                isMoving = false;
                roomSys.levelLoader.Load(
                    new Room(transform.position, "Transition", this.gameObject)
                );
                yield return new WaitForSeconds(1);
                battleSystem.SetActive(true);
                SwitchCamera(1);
                gameController.StartBattle();
            }
        }
    }
    public IEnumerator CheckIfInTrainersView(TrainerController trainer, bool inView)
    {
        if (inView && trainer != null && playerActive)
        {
            yield return trainer.TriggerTrainerBattle(this);
            trainer.fov.SetActive(false);
        }
    }
    public void SwitchCamera(int camIndex)
    {
        this.camIndex = camIndex;
        cameras.ForEach(c => c.gameObject.SetActive(false));
        cameras[camIndex].gameObject.SetActive(true);
    }
    public object CaptureState()
    {
        var saveData = new PlayerSaveData()
        {
            pos = new float[] { transform.position.x, transform.position.y },
            creatures = GetComponent<CreaturesParty>().Creatures.Select(p => p.GetSaveData()).ToList()
        };
        return saveData;
    }
    public void RestoreState(object state)
    {
        var saveData = (PlayerSaveData)state;

        //Restore pos
        var pos = saveData.pos;
        transform.position = new Vector3(pos[0], pos[1]);

        // Restore party
        GetComponent<CreaturesParty>().Creatures = saveData.creatures.Select(s => new Creature(s)).ToList();
    }
    public void SetViewPort(Rect port)
    {
        viewPort = new Rect(port);
        foreach (var cam in cameras)
        {
            cam.rect = port;
        }
    }

}

[System.Serializable]
public class PlayerSaveData
{
    public float[] pos;
    public List<CreatureSaveData> creatures;
}

