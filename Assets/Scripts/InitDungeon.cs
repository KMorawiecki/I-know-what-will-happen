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
                newRoom.visited = -1;
                newRoom.leftTel = false;
                newRoom.rightTel = false;
                newRoom.downTel = false;
                newRoom.upTel = false;

                newRoom.enemy_num = 0;
                newRoom.enemy_x = 0;
                newRoom.enemy_y = 0;
                newRoom.enemy_name = "";
                newRoom.enemy_hp = "";

                newDungeonLevel.Add(newRoom);
            }

        //add doors and common connections
        //decide if enemy room
        for (int i = 0; i < 3 + level; i++)
            for (int j = 0; j < 3 + level; j++)
            {
                if (i == 0)
                    newDungeonLevel.ElementAt(i * (level + 3) + j).upTel = true;
                if (j == level + 2)
                    newDungeonLevel.ElementAt(i * (level + 3) + j).rightTel = true;
                if (i == level + 2)
                    newDungeonLevel.ElementAt(i * (level + 3) + j).downTel = true;
                if (j == 0)
                    newDungeonLevel.ElementAt(i * (level + 3) + j).leftTel = true;

                if (!newDungeonLevel.ElementAt(i * (level + 3) + j).rightTel)
                    newDungeonLevel.ElementAt(i * (level + 3) + j).right = i * (level + 3) + j + 1;
                if (!newDungeonLevel.ElementAt(i * (level + 3) + j).downTel)
                    newDungeonLevel.ElementAt(i * (level + 3) + j).down = i * (level + 3) + j + level + 3;
                if (!newDungeonLevel.ElementAt(i * (level + 3) + j).leftTel)
                    newDungeonLevel.ElementAt(i * (level + 3) + j).left = i * (level + 3) + j - 1;
                if (!newDungeonLevel.ElementAt(i * (level + 3) + j).upTel)
                    newDungeonLevel.ElementAt(i * (level + 3) + j).up = i * (level + 3) + j - level - 3;

                AddDoors(newDungeonLevel.ElementAt(i * (level + 3) + j));

                //enemy chance calculated

                if (random.Next(1) == 0)
                    AddMonsters(newDungeonLevel.ElementAt(i * (level + 3) + j));
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

    private void AddMonsters(RoomInfo roomInfo)
    {
        int mon_num = random.Next(4) + 1;
        //Dictionary<Tuple<int, int>, string> enemyList = new Dictionary<Tuple<int, int>, string>();
        List<Tuple<int, int>> positions = new List<Tuple<int, int>>();

        roomInfo.enemy_num = mon_num;

        for (int i = 0; i < mon_num; i++)
        {
            int x = random.Next(1, 5);
            int y = random.Next(1, 3);

            if (!positions.Contains(new Tuple<int, int>(x, y)))
            {
                positions.Add(new Tuple<int, int>(x, y));

                roomInfo.enemy_x *= 10;
                roomInfo.enemy_x += x;

                roomInfo.enemy_y *= 10;
                roomInfo.enemy_y += y;

                roomInfo.enemy_name += "_Goblin";
                roomInfo.enemy_hp += "_10";
            }
            else
                i--;
        }
    }

    //room - room from which teleport starts
    //dir - direction on teleport door
    //shutOrOpen - bool for door checking
    private void AddTeleport(RoomInfo room, string dir, int shutOrOpen)
    {
        int index = -1;
        bool possibleTel = false;
        int teleportIndex = -1;
        int destination;
        int stuckCount = 0;

        while (!possibleTel)
        {
            index = random.Next(availableTeleports.Count);
            teleportIndex = availableTeleports.ElementAt(index);
            possibleTel = CheckIfTeleportPossible(room, teleportIndex, -shutOrOpen);
            stuckCount++;
            if (stuckCount > 100)
            {
                teleportIndex = EmergencySwap(room, shutOrOpen);
                possibleTel = true;
            }
        }

        availableTeleports.Remove(teleportIndex);

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

    //check if doors are correctly paired
    //check if not leading to itself
    //check if two doors aren't leading to the same room
    private bool CheckIfTeleportPossible(RoomInfo startingRoom, int teleportIndex, int expectedDoor)
    {
        int destination;
        int newRoomID;

        if (teleportIndex < level + 3)
        {
            newRoomID = newDungeonLevel.ElementAt(teleportIndex).variant;
            if (startingRoom.variant == newRoomID)
                return false;
            if (newRoomID == startingRoom.right || newRoomID == startingRoom.left || newRoomID == startingRoom.down)
                return false;
            return newDungeonLevel.ElementAt(teleportIndex).uplock == expectedDoor;
        }
        else if (teleportIndex < 2 * (level + 3))
        {
            destination = 4 * (teleportIndex - 3) - 1;
            newRoomID = newDungeonLevel.ElementAt(destination).variant;
            if (startingRoom.variant == newDungeonLevel.ElementAt(destination).variant)
                return false;
            if (newRoomID == startingRoom.up || newRoomID == startingRoom.left || newRoomID == startingRoom.down)
                return false;
            return newDungeonLevel.ElementAt(destination).rightlock == expectedDoor;
        }
        else if (teleportIndex < 3 * (level + 3))
        {
            destination = 23 - teleportIndex;
            newRoomID = newDungeonLevel.ElementAt(destination).variant;
            if (startingRoom.variant == newDungeonLevel.ElementAt(destination).variant)
                return false;
            if (newRoomID == startingRoom.up || newRoomID == startingRoom.left || newRoomID == startingRoom.right)
                return false;
            return newDungeonLevel.ElementAt(destination).downlock == expectedDoor;
        }
        else
        {
            destination = 4 * (15 - teleportIndex);
            newRoomID = newDungeonLevel.ElementAt(destination).variant;
            if (startingRoom.variant == newDungeonLevel.ElementAt(destination).variant)
                return false;
            if (newRoomID == startingRoom.up || newRoomID == startingRoom.right || newRoomID == startingRoom.down)
                return false;
            return newDungeonLevel.ElementAt(destination).leftlock == expectedDoor;
        }
    }
    
    //if things went terribly on teleporter addition
    private int EmergencySwap(RoomInfo room, int shutOrOpen)
    {
        List<int> emergencyTeleports = Enumerable.Range(0, (level + 3) * 4).ToList();

        int index = -1;
        bool possibleTel = false;
        int teleportIndex = -1;
        int destination;
        int swappedTel = -1;

        while (!possibleTel)
        {
            index = random.Next(emergencyTeleports.Count);
            teleportIndex = emergencyTeleports.ElementAt(index);
            possibleTel = CheckIfTeleportPossible(room, teleportIndex, -shutOrOpen);
        }

        if (teleportIndex < level + 3)
        {
            destination = teleportIndex;
            swappedTel = newDungeonLevel.ElementAt(destination).up;
        }
        else if (teleportIndex < 2 * (level + 3))
        {
            destination = 4 * (teleportIndex - 3) - 1;
            swappedTel = newDungeonLevel.ElementAt(destination).right;
        }
        else if (teleportIndex < 3 * (level + 3))
        {
            destination = 23 - teleportIndex;
            swappedTel = newDungeonLevel.ElementAt(destination).down;
        }
        else
        {
            destination = 4 * (15 - teleportIndex);
            swappedTel = newDungeonLevel.ElementAt(destination).left;
        }

        RoomInfo swappedRoom = newDungeonLevel.Find(x => x.variant == swappedTel);

        if(swappedRoom.up == destination)
            availableTeleports.Add(swappedTel);
        if (swappedRoom.right == destination)
            availableTeleports.Add((swappedTel + 13)/4);
        if (swappedRoom.down == destination)
            availableTeleports.Add(23 - swappedTel);
        if (swappedRoom.left == destination)
            availableTeleports.Add((60 - swappedTel)/4);

        availableTeleports.Add(teleportIndex);
        Debug.Log("emergency swapped: " + teleportIndex);
        return teleportIndex;
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

    //to easily find variant number
    private int ConvertTelToVariant(int teleport)
    {
        if (teleport < level + 3)
            return teleport;
        else if (teleport < 2 * (level + 3))
            return 4 * (teleport - 3) - 1;
        else if (teleport < 3 * (level + 3))
            return 23 - teleport;
        else
            return 4 * (15 - teleport);
    }

    ////to easily find teleport number
    //private int ConvertVariantToTel()
    //{
    //    if (variant < level + 3)
    //        return teleport;
    //    else if (teleport < 2 * (level + 3))
    //        return 4 * (teleport - 3) - 1;
    //    else if (teleport < 3 * (level + 3))
    //        return 23 - teleport;
    //    else
    //        return 4 * (15 - teleport);
    //}
}
