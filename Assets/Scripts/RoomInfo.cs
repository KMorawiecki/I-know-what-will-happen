using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class RoomInfo
{
    public int variant;
    public int left;
    public string leftlock;
    public int right;
    public string rightlock;
    public int up;
    public string uplock;
    public int down;
    public string downlock;
}

[Serializable]
public class RootObject
{
    public List<RoomInfo> roomList;
}
