using UnityEngine;
using TMPro;
public class ControllerTest : MonoBehaviour
{
    public TextMeshProUGUI text;

    void Update()
    {
        if (Input.anyKey)
        {
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(key))
                {
                    text.text = key.ToString();
                    break;
                }
            }
        }
    }
}