using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct ClusterAttributes
{
    public int height;
    public int width;
    public int x_coor;
    public int y_coor;

    public ClusterAttributes(int h, int w, int x, int y)
    {
        height = h;
        width = w;
        x_coor = x;
        y_coor = y;
    }
}

public class Minimap : MonoBehaviour
{
    //private List<List<RoomInfo>> visitedRoomsClusters = new List<List<RoomInfo>>(); //room list, cluster oriented
    private List<GameObject> clusterHolderList = new List<GameObject>();    //game objects to parent room transforms
    private List<ClusterAttributes> clusterAttributesList = new List<ClusterAttributes>();
    private int clusterNum = 1;
    private Vector3 currentTransform = new Vector3(0, 0, 0);
    private float shiftUnit = Convert.ToSingle(0.1);
    private MapManager mapManager;
    private GameObject roomObjectPrefab;
    private int leftTurns = 0;
    private int upTurns = 0;    //used to determine if cluster should change position

    // Start is called before the first frame update
    void Start()
    {
        GameObject go = GameObject.Find("MapManager");
        mapManager = go.GetComponent<MapManager>();
        mapManager.SetMinimap(this);

        GameObject firstCluster = new GameObject("Cluster" + clusterNum);
        clusterHolderList.Add(firstCluster);
        clusterAttributesList.Add(new ClusterAttributes(0, 0, 0, 0));

        AddRoom(firstCluster, 0, 0, 0);
    }

    private void AddRoom(GameObject holder, int cluster_id, int x, int y)
    {
            GameObject go = Instantiate(roomObjectPrefab, currentTransform, Quaternion.identity);
            go.transform.SetParent(holder.transform);
    }

    public void WentInDirection(string dir)
    {
        switch (dir)
        {
            case "left":
                leftTurns++;
                currentTransform -= new Vector3(shiftUnit, 0, 0);
                break;
            case "right":
                leftTurns--;
                currentTransform += new Vector3(shiftUnit, 0, 0);
                break;
            case "up":
                upTurns++;
                currentTransform += new Vector3(0, shiftUnit, 0);
                break;
            case "down":
                upTurns--;
                currentTransform -= new Vector3(0, shiftUnit, 0);
                break;
        }

        AddRoom(clusterHolderList[0]);

        if (leftTurns > 0)
            ShiftCluster(clusterHolderList[0], new Vector3(0, shiftUnit, 0));
        else if (upTurns > 0)
            ShiftCluster(clusterHolderList[0], new Vector3(shiftUnit, 0, 0));
    }

    public void ShiftCluster(GameObject holder, Vector3 translate)
    {
        holder.transform.Translate(translate);
    }
}
