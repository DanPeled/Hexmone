using UnityEngine;

[System.Serializable]
public class RoomSystem
{
    public Room startingRoom;
    public Room currentRoom;
    public LevelLoader levelLoader;

    public RoomSystem(Room startingRoom)
    {
        this.startingRoom = startingRoom;
        this.currentRoom = startingRoom;
    }

    public void ChangeRoom(Room targetRoom, GameObject player)
    {
        this.currentRoom = targetRoom;
        this.levelLoader.Load(targetRoom);
    }
}
