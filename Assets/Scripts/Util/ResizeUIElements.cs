using UnityEngine;
using UnityEngine.UI;

public class ResizeUIElements : MonoBehaviour
{
    public Canvas[] canvases;
    public float referenceScreenWidth = 800f;
    public float referenceScreenHeight = 600f;

    void Start()
    {
        ResizeElements();
    }

    void Update()
    {
        if (Screen.width != referenceScreenWidth || Screen.height != referenceScreenHeight)
        {
            ResizeElements();
        }
    }

    void ResizeElements()
    {
        float screenRatio = Screen.width / Screen.height;
        float referenceRatio = referenceScreenWidth / referenceScreenHeight;
        float scaleFactor;

        if (screenRatio >= referenceRatio)
        {
            scaleFactor = Screen.height / referenceScreenHeight;
        }
        else
        {
            scaleFactor = Screen.width / referenceScreenWidth;
        }

        foreach (Canvas canvas in canvases)
        {
            foreach (RectTransform rectTransform in canvas.GetComponentsInChildren<RectTransform>())
            {
                rectTransform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
            }
        }

        referenceScreenWidth = Screen.width;
        referenceScreenHeight = Screen.height;
    }
}
