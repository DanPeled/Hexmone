using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class MobileControls : MonoBehaviour
{
    Canvas canvas;
    public static MobileControls i;
    public bool action, back, start, up, down, left, right;
    public static bool isMobilePersisted = false; // Static variable to persist the isMobile state across scene changes
    public bool isMobile;
    public MobileButton actionBtn, backBtn, upBtn, downBtn, leftBtn, rightBtn, startBtn;
    public GameObject dpad;
    public List<GameObject> buttons;
    public bool active;
    public Rect mobileViewPort = new Rect(0.15f, 0.3f, 0.7f, 0.7f);
    public Rect defaultViewPort = new Rect(0, 0, 1, 1);
    public Rect currentViewPort;

    void SetUp()
    {
        buttons.Add(actionBtn.gameObject);
        buttons.Add(backBtn.gameObject);
        buttons.Add(dpad);
        buttons.Add(startBtn.gameObject);
        canvas = GetComponent<Canvas>();
    }

    void Start()
    {
        SetUp();
        actionBtn?.SetAction(() => pAction());
        backBtn?.SetAction(() => pBack());
        upBtn?.SetAction(() => Up());
        downBtn?.SetAction(() => Down());
        leftBtn?.SetAction(() => Left());
        rightBtn?.SetAction(() => Right());
        upBtn?.SetUp(() => up = false);
        downBtn?.SetUp(() => down = false);
        leftBtn?.SetUp(() => left = false);
        rightBtn?.SetUp(() => right = false);
        startBtn?.SetUp(() => pStart());
        isMobile = isMobilePersisted; // Restore the persisted state of the script
        Toggle(isMobile); // Toggle the buttons according to the persisted state
    }

    void Update()
    {
        isMobile = isMobilePersisted; // Restore the persisted state of the script
        i = this;
        //this.canvas.worldCamera = Player.instance.cameras[Player.instance.camIndex];
        this.currentViewPort = (isMobile) ? mobileViewPort : defaultViewPort;
        Player.instance.SetViewPort(currentViewPort);
        if ((SystemInfo.deviceType == DeviceType.Handheld && active) || isMobile)
        {
            // The device is running Android
            Toggle(true);
        }
        else
        {
            Toggle(false);
        }
    }

    public void Toggle(bool state)
    {
        isMobile = state;
        isMobilePersisted = state; // Store the state persistently
        foreach (var button in buttons)
        {
            button?.SetActive(state);
        }
    }
    public void pAction()
    {
        StartCoroutine(Action());
    }
    public void pBack()
    {
        StartCoroutine(Back());
    }
    public void pStart(){
        StartCoroutine(Start_());
    }
    public IEnumerator Action()
    {
        this.action = true;
        yield return new WaitForSeconds(0.1f);
        this.action = false;
    }
    public IEnumerator Start_(){
        this.start = true;
        yield return new WaitForSeconds(0.1f);
        this.start = false;
    }
    public IEnumerator Back()
    {
        this.back = true;
        yield return new WaitForSeconds(0.1f);
        this.back = false;
    }
    public void Up()
    {
        this.up = true;
    }
    public void Down()
    {
        this.down = true;
    }
    public void Left()
    {
        this.left = true;
    }
    public void Right()
    {
        this.right = true;
    }
}