using UnityEngine;

[System.Serializable]
public class Room
{
    public string scene;
    public Vector3 roomPosition;

    public Room(Vector3 roomPosition, string scene)
    {
        this.roomPosition = roomPosition;
        this.scene = scene;
    }
}
