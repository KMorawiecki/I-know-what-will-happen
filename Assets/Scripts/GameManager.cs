using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    private int level = 1;
    private List<RoomInfo> newDungeonLevel = new List<RoomInfo>();
    private List<int> availableTeleports;

    // Start is called before the first frame update
    void Start()
    {
        availableTeleports = Enumerable.Range(1, (level + 3) * 4).ToList();
        CreateDungeon(level);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CreateDungeon(int level)
    {
        for(int i = 0; i < 3 + level; i++)
            for(int j = 0; j < 3 + level; j++)
            {
                RoomInfo newRoom = new RoomInfo();
                newRoom.variant = (j + (i*(3+level)));
                newRoom.left = -1;
                newRoom.leftlock = "null";
                newRoom.right = -1;
                newRoom.rightlock = "null";
                newRoom.up = -1;
                newRoom.uplock = "null";
                newRoom.down = -1;
                newRoom.downlock = "null";

                newDungeonLevel.Add(newRoom);
            }

        for (int i = 0; i < 3 + level; i++)
            for (int j = 0; j < 3 + level; j++)
            {
                if (i == 0 && newDungeonLevel.ElementAt(j).up == -1)
                {
                    //remove from available teleport pool
                    var itemToRemove = availableTeleports.SingleOrDefault(r => r == j);
                    availableTeleports.Remove(itemToRemove);
                    //find destination at random
                    AddTeleport(newDungeonLevel.ElementAt(j), "up");
                    Debug.Log(itemToRemove);
                }
                if (j == level + 2 && newDungeonLevel.ElementAt(i*(level + 3) + j).right == -1)
                {
                    var itemToRemove = availableTeleports.SingleOrDefault(r => r == ((i * (level + 3) + j) + 13)/4);
                    availableTeleports.Remove(itemToRemove);
                    AddTeleport(newDungeonLevel.ElementAt(i * (level + 3) + j), "right");
                    Debug.Log(itemToRemove);
                }
                if(i == level + 2 && newDungeonLevel.ElementAt(i * (level + 3) + j).down == -1)
                {
                    var itemToRemove = availableTeleports.SingleOrDefault(r => r == (23 - (i * (level + 3) + j)));
                    availableTeleports.Remove(itemToRemove);
                    AddTeleport(newDungeonLevel.ElementAt(i * (level + 3) + j), "down");
                    Debug.Log(itemToRemove);
                }
                if (j == 0 && newDungeonLevel.ElementAt(i * (level + 3)).left == -1)
                {
                    var itemToRemove = availableTeleports.SingleOrDefault(r => r == (60 - i*(level + 3))/4);
                    availableTeleports.Remove(itemToRemove);
                    AddTeleport(newDungeonLevel.ElementAt(i * (level + 3)), "left");
                    Debug.Log(itemToRemove);
                }
            }

        RoomInfo[] dungeon = newDungeonLevel.ToArray();
        string dungeonJson = JsonHelper.ToJson(dungeon, true);
        Debug.Log(dungeonJson);
    }

    private void AddTeleport(RoomInfo room, string dir)
    {
        System.Random random = new System.Random();
        int index = random.Next(availableTeleports.Count);
        int teleportIndex = availableTeleports.ElementAt(index);
        availableTeleports.RemoveAt(index);
        int destination = newDungeonLevel.ElementAt(teleportIndex).variant;

        switch (dir)
        {
            case "up":
                room.up = newDungeonLevel.ElementAt(teleportIndex).variant;
                break;
            case "down":
                room.down = newDungeonLevel.ElementAt(teleportIndex).variant;
                break;
            case "left":
                room.left = newDungeonLevel.ElementAt(teleportIndex).variant;
                break;
            case "right":
                room.right = newDungeonLevel.ElementAt(teleportIndex).variant;
                break;
        }

        if (teleportIndex < level + 3)
            newDungeonLevel.ElementAt(teleportIndex).up = room.variant;
        else if (teleportIndex < 2 * (level + 3))
            newDungeonLevel.ElementAt(4*(teleportIndex - 3) - 1).right = room.variant;
        else if (teleportIndex < 3 * (level + 3))
            newDungeonLevel.ElementAt(23 - teleportIndex).down = room.variant;
        else
            newDungeonLevel.ElementAt(4*(15 - teleportIndex)).left = room.variant;
    }
}
