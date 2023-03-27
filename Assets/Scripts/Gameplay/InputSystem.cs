using UnityEngine;

public class InputSystem : MonoBehaviour
{
    public Toggle up = new Toggle(),
        down = new Toggle(),
        left = new Toggle(),
        right = new Toggle(),
        action = new Toggle(),
        back = new Toggle();
    public static InputSystem instance;
    void Update()
    {
        instance = this;
        up.update(Input.GetAxisRaw("Vertical") > 0 || MobileControls.i.up);
        down.update(Input.GetAxisRaw("Vertical") < 0 || MobileControls.i.down);
        left.update(Input.GetAxisRaw("Horizontal") < 0 || MobileControls.i.left);
        right.update(Input.GetAxisRaw("Horizontal") > 0 || MobileControls.i.right);
        action.update(Input.GetButton("Action") || MobileControls.i.action);
        back.update(Input.GetButton("Back") || MobileControls.i.back);
    }
}