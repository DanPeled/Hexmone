using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialog
{
    public List<string> lines;

    public Dialog(List<string> lines)
    {
        this.lines = lines;
    }

    public Dialog(string line)
    {
        this.lines = new List<string>() { line };
    }
}
