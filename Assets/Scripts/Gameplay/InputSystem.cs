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

        if (isMobile)
        {
            // Handle mobile input here
        }
        else
        {
            // Handle keyboard input here
            up.update(Input.GetKey(ControllerProfile.upKeyCode));
            down.update(Input.GetKey(ControllerProfile.downKeyCode));
            left.update(Input.GetKey(ControllerProfile.leftKeyCode));
            right.update(Input.GetKey(ControllerProfile.rightKeyCode));
            action.update(Input.GetKey(ControllerProfile.actionKeyCode));
            back.update(Input.GetKey(ControllerProfile.backKeyCode));
            start.update(Input.GetKey(ControllerProfile.startKeyCode));
        }

        // Check if any controllers are connected
        string[] joystickNames = Input.GetJoystickNames();
        if (joystickNames.Length > 0 && !string.IsNullOrEmpty(joystickNames[0]))
        {
            // Handle gamepad input only if a controller is connected
            Vector2 gamepadMovementInput = ControllerProfile.GetGamepadMovementInput();
            up.update(gamepadMovementInput.y > 0);
            down.update(gamepadMovementInput.y < 0);
            left.update(gamepadMovementInput.x < 0);
            right.update(gamepadMovementInput.x > 0);
        }
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

    public static void ResetGamepadProfile()
    {
        // Movement key codes for gamepad
        upKeyCode = KeyCode.None;
        downKeyCode = KeyCode.None;
        leftKeyCode = KeyCode.None;
        rightKeyCode = KeyCode.None;

        // Actions key codes for gamepad
        actionKeyCode = KeyCode.JoystickButton0; // A button
        backKeyCode = KeyCode.JoystickButton1; // B button
        startKeyCode = KeyCode.JoystickButton9; // Start button
    }

    // Use this method to check the movement input on a gamepad
    public static Vector2 GetGamepadMovementInput()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        return new Vector2(horizontalInput, verticalInput);
    }
}
