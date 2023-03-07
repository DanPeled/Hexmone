public class Toggle
{
    private bool lastInput = false, pressed = false, changed = false, state = false;
    public Toggle() { }
    public Toggle(bool initialState) { this.state = initialState; }

    public void update(bool input)
    {
        if (lastInput != input)
        { // if the input has changed,
            changed = true; // (it has changed)
            pressed = input;
            if (pressed) state = !state; // The toggle changes only when it's pressed and not when it's released
            lastInput = input;
        }
        
        else
        {
            changed = false; // otherwise it didn't
        }
    }

    public void set(bool input)
    {
        changed = lastInput != input; // simplified
        state = input;
        lastInput = input;
    }

    public bool toggle()
    {
        state = !state;
        changed = true;
        return state;
    }

    public bool isPressed()
    {
        return pressed;
    }

    public bool isChanged()
    {
        return changed;
    }

    public bool getState() { return state; }

    public bool isClicked()
    {
        return changed && pressed;
    }

    public bool isReleased() { return changed && !pressed; }
}