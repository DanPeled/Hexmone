using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using AYellowpaper.SerializedCollections;
public class CreaturesBox : MonoBehaviour
{
    public SerializedDictionary<int, Creature[]> boxes;
    int boxSize = 10;
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
    public bool IsFull(int box)
    {
        return boxes[box].Length == boxSize;
    }
    public void Add(Creature creature)
    {
        int boxIndex = 0;
        foreach (var box in boxes.Keys)
        {
            if (!IsFull(box))
            {
                boxIndex = box;
                break;
            }
        }
        GetBox(boxIndex).Append(creature);
        DialogManager.instance.ShowDialogText($"{creature.GetName()} has been added to your Hexo-Box");
    }

    public static CreaturesBox GetPlayerBox()
    {
        return FindObjectsOfType<CreaturesBox>().FirstOrDefault(cb => cb.gameObject.tag.Equals("Player"));
    }
}