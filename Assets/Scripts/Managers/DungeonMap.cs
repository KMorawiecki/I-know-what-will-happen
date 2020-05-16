using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMap
{
    private RoomInfo[] roomlist;

    public RoomInfo GetRoom(int roomNumber)
    {
        Debug.Log(1);
        return roomlist[roomNumber];
    }
}
