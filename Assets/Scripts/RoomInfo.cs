using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class RoomInfo
{
    public int variant;
    public int left;
    public int leftlock;
    public int right;
    public int rightlock;
    public int up;
    public int uplock;
    public int down;
    public int downlock;

    public int visited; //cluster number (-1 if not visited)
    public bool upTel;
    public bool rightTel;
    public bool leftTel;
    public bool downTel;

    //OK, THIS IS A WORKAROUD TO CLEAN, SIMPLE DICTIONARY, BEAR WITH ME HERE
    //enemy x - x position, digit count equals number of enemies (ie. second digit form end is x position of second monster default: 0
    //enemy y - same default: 0
    //enemy name - after "_" comes another enemy name default: ""
    //enemy hp - in format "123_123_123" default: "

    //goodbye my love
    //public Dictionary<Tuple<int, int>, string> enemyPositions; //x and y on room tilemap of enemy of given name

    public int enemy_num;
    public int enemy_x;
    public int enemy_y;
    public string enemy_name;
    public string enemy_hp;
}

[Serializable]
public class RootObject
{
    public List<RoomInfo> roomList;
}
