using UnityEngine;

public class InputSystem : MonoBehaviour
{
    public static Toggle up = new Toggle(),
        down = new Toggle(),
        left = new Toggle(),
        right = new Toggle(),
        action = new Toggle(),
        back = new Toggle(),
        select = new Toggle(),
        start = new Toggle();
    public bool isMobile;
    public static InputSystem i;
    public bool customProfile = false;

    void Start()
    {
        if (!customProfile)
        {
            ControllerProfile.ResetProfile();
        }
    }

    void Update()
    {
        i = this;
        up.update(Input.GetKey(ControllerProfile.upKeyCode));
        down.update(Input.GetKey(ControllerProfile.downKeyCode));
        left.update(Input.GetKey(ControllerProfile.leftKeyCode));
        right.update(Input.GetKey(ControllerProfile.rightKeyCode));
        action.update(Input.GetKey(ControllerProfile.actionKeyCode));
        back.update(Input.GetKey(ControllerProfile.backKeyCode));
        start.update(Input.GetKey(ControllerProfile.startKeyCode));
    }
}

public class ControllerProfile
{
    public static KeyCode actionKeyCode,
        startKeyCode,
        selectKeyCode,
        rightKeyCode,
        leftKeyCode,
        upKeyCode,
        downKeyCode,
        backKeyCode;

    public static void ReplaceKey(KeyCode oldKeyCode, KeyCode newKeyCode)
    {
        if (actionKeyCode == oldKeyCode)
            actionKeyCode = newKeyCode;

        if (startKeyCode == oldKeyCode)
            startKeyCode = newKeyCode;

        if (selectKeyCode == oldKeyCode)
            selectKeyCode = newKeyCode;

        if (rightKeyCode == oldKeyCode)
            rightKeyCode = newKeyCode;

        if (leftKeyCode == oldKeyCode)
            leftKeyCode = newKeyCode;

        if (upKeyCode == oldKeyCode)
            upKeyCode = newKeyCode;

        if (downKeyCode == oldKeyCode)
            downKeyCode = newKeyCode;

        if (backKeyCode == oldKeyCode)
            backKeyCode = newKeyCode;
    }

    public static void ResetProfile()
    {
        //Movement key codes
        upKeyCode = KeyCode.W;
        downKeyCode = KeyCode.S;
        leftKeyCode = KeyCode.A;
        rightKeyCode = KeyCode.D;

        //Actions key codes
        actionKeyCode = KeyCode.J;
        backKeyCode = KeyCode.K;
        startKeyCode = KeyCode.Return;
    }
}
