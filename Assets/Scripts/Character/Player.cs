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
    public bool loadPos;
    public static Room gotoRoom;

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
    public FacingDir facingDir;
    public static Player instance;
    float horizontal;
    float vertical;
    float moveLimiter = 0.7f;
    public bool colldingDoor,
        collidingInteractable;
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
        console;
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
        gameController = FindObjectOfType<GameController>();
    }

    void Start()
    {
        roomSys = new RoomSystem(roomSys.startingRoom);
        body = GetComponent<Rigidbody2D>();
        // this.roomSys.levelLoader = GameObject
        //     .FindGameObjectWithTag("LevelLoader")
        //     .GetComponent<LevelLoader>();
        //StartCoroutine(roomSys.ChangeRoom(roomSys.startingRoom, this.gameObject));
        GameController.instance.battleSystem.gameObject.SetActive(false);
        if (gotoRoom != null)
            transform.position = gotoRoom.roomPosition;
        SavingSystem.i.Load("saveSlot1");
    }

    void Update()
    {
        notificationBar.transform.parent.gameObject.GetComponent<Canvas>().worldCamera = cameras[
            camIndex
        ];
        instance = this;
        if (colldingDoor && InputSystem.action.isClicked())
        {
            Door(doorObject.gameObject);
        }
        if (active)
        {
            TV();
        }
        // this.roomSys.levelLoader = GameObject
        //     .FindGameObjectWithTag("LevelLoader")
        //     .GetComponent<LevelLoader>();
        if (playerActive)
        {
            this.GetComponent<BoxCollider2D>().enabled = true;
            // Gives a value between -1 and 1

            if (playerActive)
            {
                this.GetComponent<BoxCollider2D>().enabled = true;

                // Restrict diagonal movement
                if (InputSystem.right.isPressed())
                {
                    horizontal = 1;
                    vertical = 0;
                }
                else if (InputSystem.left.isPressed())
                {
                    horizontal = -1;
                    vertical = 0;
                }
                else if (InputSystem.up.isPressed())
                {
                    horizontal = 0;
                    vertical = 1;
                }
                else if (InputSystem.down.isPressed())
                {
                    horizontal = 0;
                    vertical = -1;
                }
                else
                {
                    horizontal = 0;
                    vertical = 0;
                }
            }
        }
        else
        {
            // this.GetComponent<BoxCollider2D>().enabled = false;
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
            anim.moveY = -1; // back
            this.facingDir = FacingDir.backward;
        }
        else if (horizontal > 0)
        {
            isMoving = true;
            anim.moveY = 0;
            anim.moveX = -1; // right
            this.facingDir = FacingDir.right;
        }
        else if (horizontal < 0)
        {
            isMoving = true;

            anim.moveX = 1; // left
            anim.moveY = 0;
            this.facingDir = FacingDir.left;
        }
        else
        {
            isMoving = false;
        }
        #endregion
        anim.isMoving = isMoving;
        if (InputSystem.action.isClicked() && interactObject != null)
        {
            ShowDialog();
            StartCoroutine(Interact(interactObject));
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            TriggerConsole();
        }
        TriggerConsole(console.activeInHierarchy);
    }

    void TriggerConsole()
    {
        playerActive = !playerActive;
        GameController.instance.state = playerActive ? GameState.FreeRoam : GameState.Console;
    }

    void TriggerConsole(bool active)
    {
        playerActive = !active;
        // GameController.instance.state = playerActive ? GameState.FreeRoam : GameState.Console;
    }

    IEnumerator Interact(GameObject obj)
    {
        if (collidingInteractable)
        {
            yield return obj.GetComponent<Interactable>()?.Interact(transform);
            collidingInteractable = false;
            interactObject = null;
            yield break;
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
        StartCoroutine(Move(horizontal, vertical));
    }

    public IEnumerator Move(float x, float y)
    {
        body.velocity = new Vector2(x * runSpeed, y * runSpeed);
        yield return null;
    }

    bool active = false;

    #region Collision
    void OnCollisionStay2D(Collision2D other)
    {
        switch (other.gameObject.tag)
        {
            case "TV":
                if (InputSystem.action.isClicked() || active)
                {
                    TV();
                }
                break;
        }
        if (other.gameObject.GetComponent<Interactable>() != null)
        {
            this.collidingInteractable = true;
            this.interactObject = other.gameObject;
        }
    }

    public void Door(GameObject other)
    {
        loadPos = false;
        SavingSystem.i.Save("saveSlot1");
        LevelLoader.i.Load();
        gotoRoom = other.GetComponent<Door>().targetRoom;
    }

    public void TV()
    {
        active = true;
        if (lastRoutine != null)
        {
            StopCoroutine(lastRoutine);
        }
        string timeOnly = DateTime.Now.ToString("h:mm:ss tt");
        // lastRoutine = StartCoroutine(notify($"{timeOnly}"));
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (lastRoutine != null)
            StopCoroutine(lastRoutine);
        if (other.gameObject.GetComponent<Interactable>() != null)
        {
            this.collidingInteractable = false;
            this.interactObject = null;
        }
        active = false;
        // if (other.gameObject.GetComponent<Interactable>() != null)
        // lastRoutine = StartCoroutine(removeNotification());
    }

    #endregion
    //public IEnumerator notify(string notification)
    // {
    //     this.notification = notification;
    //     this.notificationBar.GetComponentInChildren<TextMeshProUGUI>().text = notification;
    //     while (notificationBar.transform.localPosition.y < -350)
    //     {
    //         this.notificationBar.transform.localPosition += new Vector3(0, 50, 0);
    //         yield return new WaitForSeconds(0.05f);
    //     }
    // }
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
        notificationBar.SetActive(true);
    }

    public void removeNotification()
    {
        notificationBar.SetActive(false);
    }

    public void EnterBattle()
    {
        GameController.instance.battleSystem.gameObject.SetActive(true);
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
        if (other.gameObject.GetComponent<IPlayerTriggerable>() != null)
        {
            other.gameObject.GetComponent<IPlayerTriggerable>().OnPlayerTriggered(this);
        }
        if (other.gameObject.tag.Equals("TallGrassBlock"))
        {
            transform.position = new Vector2(71.84f, -51.6f);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        // if (other.gameObject.GetComponent<SurfableWater>() != null)
        // {
        //     anim.isSurfing = true;
        //     other.gameObject.GetComponent<CompositeCollider2D>().isTrigger = anim.isSurfing;
        // }
        if (other.gameObject.GetComponent<Interactable>() != null)
        {
            this.collidingInteractable = true;
            this.interactObject = other.gameObject;
        }
    }

    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.GetComponent<Interactable>() != null)
        {
            this.collidingInteractable = false;
            this.interactObject = null;
            // lastRoutine = (removeNotification());
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
            this.interactObject = null;
            removeNotification();
        }
        if (other.gameObject.GetComponent<IPlayerTriggerable>() != null)
        {
            other.GetComponent<BoxCollider2D>().enabled = true;
        }
        // if (other.gameObject.GetComponent<SurfableWater>() != null){
        //     anim.isSurfing = false;
        // }
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        OnMoveOver(other.gameObject);
        switch (other.gameObject.tag)
        {
            case "Door":
                if (InputSystem.action.isClicked())
                {
                    Door(other.gameObject);
                }
                break;
            case "TV":
                if (InputSystem.action.isClicked() || active)
                {
                    TV();
                }
                break;
        }
        if (other.gameObject.tag == "TallGrassBlock")
        {
            transform.position = new Vector2(71.84f, -51.6f);
        }
        if (other.gameObject.GetComponent<Interactable>() != null)
        {
            this.collidingInteractable = true;
            this.interactObject = other.gameObject;
        }
    }

    public void OnMoveOver(GameObject other)
    {
        StartCoroutine(
            CheckForEncounters(other.tag == "LongGrass" && (horizontal != 0 || vertical != 0))
        );
        StartCoroutine(
            CheckIfInTrainersView(
                other.gameObject.GetComponentInParent<TrainerController>(),
                other.tag.Equals("FOV")
            )
        );
    }

    public IEnumerator CheckForEncounters(bool touchingLongGrass)
    {
        if (!playerActive)
            yield break;
        int rnd = UnityEngine.Random.Range(1, 1000);
        if (touchingLongGrass)
        {
            if (rnd <= 10 && playerActive)
            {
                playerActive = false;
                isMoving = false;
                // LevelLoader.i.Load();
                yield return new WaitForSeconds(1);
                GameController.instance.battleSystem.gameObject.SetActive(true);
                SwitchCamera(1);
                gameController.StartBattle();
            }
        }
    }

    public IEnumerator CheckIfInTrainersView(TrainerController trainer, bool inView)
    {
        if (inView && trainer != null && playerActive)
        {
            trainer.fov.SetActive(false);
            yield return trainer.TriggerTrainerBattle(this);
            yield break;
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
            creatures = GetComponent<CreaturesParty>().Creatures
                .Select(p => p.GetSaveData())
                .ToList()
        };
        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = (PlayerSaveData)state;

        //Restore pos
        if (loadPos)
        {
            var pos = saveData.pos;
            transform.position = new Vector3(pos[0], pos[1]);
        }
        // Restore party
        GetComponent<CreaturesParty>().Creatures = saveData.creatures
            .Select(s => new Creature(s))
            .ToList();
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
