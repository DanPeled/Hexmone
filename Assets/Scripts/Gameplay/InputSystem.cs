using UnityEngine;

public class InputSystem : MonoBehaviour
{
    public Toggle up = new Toggle(),
        down = new Toggle(),
        left = new Toggle(),
        right = new Toggle();
    public static InputSystem instance;
    void Update()
    {
        instance = this;
        up.update(Input.GetAxisRaw("Vertical") > 0);
        down.update(Input.GetAxisRaw("Vertical") < 0);
        left.update(Input.GetAxisRaw("Horizontal") < 0);
        right.update(Input.GetAxisRaw("Horizontal") > 0);
    }
}