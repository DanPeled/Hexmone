using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using AYellowpaper.SerializedCollections;
public class CreaturesBox : MonoBehaviour
{
    public SerializedDictionary<int, Creature[]> boxes;
    public Creature Get(int index, int box)
    {
        return GetBox(box)[index];
    }
    public Creature[] GetBox(int box)
    {
        return boxes[box];
    }
    public void GetIndexInAllBoxes(Creature find, out int boxIndex, out int creatureIndex)
    {
        boxIndex = -1;
        creatureIndex = -1;
        for (int box = 0; box < boxes.Values.Count; box++)
        {
            for (int c = 0; c < boxes[box].Length; c++)
            {
                if (Get(c, box) == find)
                {
                    boxIndex = box; creatureIndex = c;
                    return;
                }
            }
        }
    }
    public void GetIndexInBox(Creature find, int boxIndex, out int creatureIndex)
    {
        creatureIndex = -1;
        for (int c = 0; c < boxes[boxIndex].Length; c++)
        {
            if (Get(c, boxIndex) == find)
            {
                creatureIndex = c;
                return;
            }
        }
    }
}