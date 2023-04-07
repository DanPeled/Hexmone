using UnityEngine;

public class InputSystem : MonoBehaviour
{
    public Toggle up = new Toggle(),
        down = new Toggle(),
        left = new Toggle(),
        right = new Toggle(),
        action = new Toggle(),
        back = new Toggle(),
        select = new Toggle(),
        start = new Toggle();
    public bool isMobile;
    public static InputSystem instance;
    void Update()
    {
        instance = this;
        // Normal Controls
        if (MobileControls.i == null && !isMobile)
        {
            up.update(Input.GetAxisRaw("Vertical") > 0);
            down.update(Input.GetAxisRaw("Vertical") < 0);
            left.update(Input.GetAxisRaw("Horizontal") < 0);
            right.update(Input.GetAxisRaw("Horizontal") > 0);
            action.update(Input.GetButton("Action") || Input.GetKey(KeyCode.J));
            back.update(Input.GetButton("Back") || Input.GetKey(KeyCode.K));
            start.update(Input.GetButton("Start"));
        }

        // Mobile Controls
        else if (isMobile || MobileControls.i != null)
        {
            up.update(Input.GetAxisRaw("Vertical") > 0 || MobileControls.i.up);
            down.update(Input.GetAxisRaw("Vertical") < 0 || MobileControls.i.down);
            left.update(Input.GetAxisRaw("Horizontal") < 0 || MobileControls.i.left);
            right.update(Input.GetAxisRaw("Horizontal") > 0 || MobileControls.i.right);
            action.update(Input.GetButton("Action") || Input.GetKey(KeyCode.J) || MobileControls.i.action);
            back.update(Input.GetButton("Back") || Input.GetKey(KeyCode.K) || MobileControls.i.back);
            start.update(Input.GetButton("Start") || MobileControls.i.start);
        }
    }
}