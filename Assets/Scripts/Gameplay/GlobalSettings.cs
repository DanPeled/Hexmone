using UnityEngine;

public class GlobalSettings : MonoBehaviour
{
    public Color highlightedColor;
    public static GlobalSettings i;

    void Awake()
    {
        i = this;
    }
}