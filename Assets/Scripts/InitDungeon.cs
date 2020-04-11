using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using System;

public class InitDungeon
{
    private int level;
    private List<RoomInfo> newDungeonLevel = new List<RoomInfo>();
    private List<int> availableTeleports;
    private System.Random random = new System.Random();

    // Start is called before the first frame update
    public InitDungeon(int givenLevel)
    {
        level = givenLevel;
        availableTeleports = Enumerable.Range(0, (level + 3) * 4).ToList();
        CreateDungeon(level);
    }

    private void CreateDungeon(int level)
    {
        //create basics
        for (int i = 0; i < 3 + level; i++)
            for (int j = 0; j < 3 + level; j++)
            {
                RoomInfo newRoom = new RoomInfo();
                newRoom.variant = (j + (i * (3 + level)));
                newRoom.left = -1;
                newRoom.leftlock = 0;
                newRoom.right = -1;
                newRoom.rightlock = 0;
                newRoom.up = -1;
                newRoom.uplock = 0;
                newRoom.down = -1;
                newRoom.downlock = 0;
                newRoom.visited = false;

                newDungeonLevel.Add(newRoom);
            }

        bool rightBlock = false;
        bool leftBlock = false;
        bool upBlock = false;
        bool downBlock = false;

        //add doors and common connections
        for (int i = 0; i < 3 + level; i++)
            for (int j = 0; j < 3 + level; j++)
            {
                if (i == 0)
                    upBlock = true;
                if (j == level + 2)
                    rightBlock = true;
                if (i == level + 2)
                    downBlock = true;
                if (j == 0)
                    leftBlock = true;

                if (!rightBlock)
                    newDungeonLevel.ElementAt(i * (level + 3) + j).right = i * (level + 3) + j + 1;
                if (!downBlock)
                    newDungeonLevel.ElementAt(i * (level + 3) + j).down = i * (level + 3) + j + level + 3;
                if (!leftBlock)
                    newDungeonLevel.ElementAt(i * (level + 3) + j).left = i * (level + 3) + j - 1;
                if (!upBlock)
                    newDungeonLevel.ElementAt(i * (level + 3) + j).up = i * (level + 3) + j - level - 3;

                AddDoors(newDungeonLevel.ElementAt(i * (level + 3) + j));

                upBlock = false;
                downBlock = false;
                leftBlock = false;
                rightBlock = false;
            }

        //add teleports
        while (availableTeleports.Count != 0)
        {
            //remove from available teleport pool
            var index = availableTeleports.ElementAt(0);
            availableTeleports.Remove(index);

            if (index < level + 3)
                AddTeleport(newDungeonLevel.ElementAt(index), "up", newDungeonLevel.ElementAt(index).uplock);
            else if (index < 2 * (level + 3))
                AddTeleport(newDungeonLevel.ElementAt(4 * (index - 3) - 1), "right", newDungeonLevel.ElementAt(4 * (index - 3) - 1).rightlock);
            else if (index < 3 * (level + 3))
                AddTeleport(newDungeonLevel.ElementAt(23 - index), "down", newDungeonLevel.ElementAt(23 - index).downlock);
            else
                AddTeleport(newDungeonLevel.ElementAt(4 * (15 - index)), "left", newDungeonLevel.ElementAt(4 * (15 - index)).leftlock);
        }


        RoomInfo[] dungeon = newDungeonLevel.ToArray();
        string dungeonJson = JsonHelper.ToJson(dungeon, true);
        File.WriteAllText(Application.dataPath + "/Data/DungeonMap.json", dungeonJson);
    }

    private void AddTeleport(RoomInfo room, string dir, int shutOrOpen)
    {
        int index = -1;
        bool possibleTel = false;
        int teleportIndex = -1;
        int destination;

        while (!possibleTel)
        {
            index = random.Next(availableTeleports.Count);
            teleportIndex = availableTeleports.ElementAt(index);
            possibleTel = CheckIfTeleportPossible(teleportIndex, -shutOrOpen);
        }

        availableTeleports.RemoveAt(index);

        if (teleportIndex < level + 3)
        {
            destination = teleportIndex;
            newDungeonLevel.ElementAt(destination).up = room.variant;
        }
        else if (teleportIndex < 2 * (level + 3))
        {
            destination = 4 * (teleportIndex - 3) - 1;
            newDungeonLevel.ElementAt(destination).right = room.variant;
        }
        else if (teleportIndex < 3 * (level + 3))
        {
            destination = 23 - teleportIndex;
            newDungeonLevel.ElementAt(destination).down = room.variant;
        }
        else
        {
            destination = 4 * (15 - teleportIndex);
            newDungeonLevel.ElementAt(destination).left = room.variant;
        }

        switch (dir)
        {
            case "up":
                room.up = newDungeonLevel.ElementAt(destination).variant;
                break;
            case "down":
                room.down = newDungeonLevel.ElementAt(destination).variant;
                break;
            case "left":
                room.left = newDungeonLevel.ElementAt(destination).variant;
                break;
            case "right":
                room.right = newDungeonLevel.ElementAt(destination).variant;
                break;
        }
    }

    private bool CheckIfTeleportPossible(int teleportIndex, int expectedDoor)
    {
        int destination;
        //Debug.Log(teleportIndex + " : " + expectedDoor);

        if (teleportIndex < level + 3)
            return newDungeonLevel.ElementAt(teleportIndex).uplock == expectedDoor;
        else if (teleportIndex < 2 * (level + 3))
        {
            destination = 4 * (teleportIndex - 3) - 1;
            return newDungeonLevel.ElementAt(destination).rightlock == expectedDoor;
        }
        else if (teleportIndex < 3 * (level + 3))
        {
            destination = 23 - teleportIndex;
            return newDungeonLevel.ElementAt(destination).downlock == expectedDoor;
        }
        else
        {
            destination = 4 * (15 - teleportIndex);
            return newDungeonLevel.ElementAt(destination).leftlock == expectedDoor;
        }
    }

    private void AddDoors(RoomInfo room)
    {
        List<int> doorToChoose = new List<int> { -1, -1, 1, 1 };

        if (room.leftlock != 0 || room.uplock != 0)
        {
            int erasingParam = room.leftlock + room.uplock;

            if (erasingParam == 0)
            {
                doorToChoose.RemoveAt(0);
                doorToChoose.RemoveAt(doorToChoose.Count - 1);
            }
            else
            {
                while (erasingParam > 0)
                {
                    doorToChoose.RemoveAt(doorToChoose.Count - 1);
                    erasingParam--;
                }
                while (erasingParam < 0)
                {
                    doorToChoose.RemoveAt(0);
                    erasingParam++;
                }
            }
        }

        if (room.leftlock == 0)
        {
            int index = random.Next(doorToChoose.Count);
            int door = doorToChoose.ElementAt(index);
            room.leftlock = door;
            doorToChoose.RemoveAt(index);

            if (room.left != -1)
                newDungeonLevel.Single(r => r.variant == room.left).rightlock = -door;
        }

        if (room.uplock == 0)
        {
            int index = random.Next(doorToChoose.Count);
            int door = doorToChoose.ElementAt(index);
            room.uplock = door;
            doorToChoose.RemoveAt(index);

            if (room.up != -1)
                newDungeonLevel.Single(r => r.variant == room.up).downlock = -door;
        }

        if (room.rightlock == 0)
        {
            int index = random.Next(doorToChoose.Count);
            int door = doorToChoose.ElementAt(index);
            room.rightlock = door;
            doorToChoose.RemoveAt(index);

            if (room.right != -1)
                newDungeonLevel.Single(r => r.variant == room.right).leftlock = -door;
        }

        if (room.downlock == 0)
        {
            int index = random.Next(doorToChoose.Count);
            int door = doorToChoose.ElementAt(index);
            room.downlock = door;
            doorToChoose.RemoveAt(index);

            if (room.down != -1)
                newDungeonLevel.Single(r => r.variant == room.down).uplock = -door;
        }
    }
}
