using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoadAuto : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.Object.DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update() { }
}
