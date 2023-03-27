using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class MobileControls : MonoBehaviour
{
    Canvas canvas;
    public static MobileControls i;
    public bool action, back, up, down, left, right, isMobile;
    public MobileButton actionBtn, backBtn, upBtn, downBtn, leftBtn, rightBtn;
    public GameObject dpad;
    public List<GameObject> buttons;
    public bool active;
    void Awake()
    {
        buttons.Add(actionBtn.gameObject);
        buttons.Add(backBtn.gameObject);
        buttons.Add(dpad);
        canvas = GetComponent<Canvas>();
    }
    void Start()
    {
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
        if (SystemInfo.deviceType == DeviceType.Handheld && active)
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
        foreach (var button in buttons)
        {
            button.SetActive(state);
        }
    }
    void Update()
    {
        i = this;
        this.canvas.worldCamera = Player.instance.cameras[Player.instance.camIndex];
    }
    public void pAction()
    {
        StartCoroutine(Action());
    }
    public void pBack()
    {
        StartCoroutine(Back());
    }
    public IEnumerator Action()
    {
        this.action = true;
        yield return new WaitForSeconds(0.1f);
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