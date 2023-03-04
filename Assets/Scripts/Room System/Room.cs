using UnityEngine;

[System.Serializable]
public class Room
{
    public string scene;
    public Vector3 roomPosition;
    public GameObject room;

    public Room(Vector3 roomPosition, string scene, GameObject room)
    {
        this.roomPosition = roomPosition;
        this.scene = scene;
        this.room = room;
    }
}
