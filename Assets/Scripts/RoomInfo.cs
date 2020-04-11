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
    public bool visited;
}

[Serializable]
public class RootObject
{
    public List<RoomInfo> roomList;
}
