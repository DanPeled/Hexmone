using System.Collections;
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

    public IEnumerator ChangeRoom(Room targetRoom, GameObject player)
    {
        player.GetComponent<Player>().playerActive = false;
        this.currentRoom = targetRoom;
        this.levelLoader.Load();
        yield return new WaitForSeconds(0.4f);
        player.transform.position = targetRoom.roomPosition;
        player.GetComponent<Player>().playerActive = true;
    }
}
