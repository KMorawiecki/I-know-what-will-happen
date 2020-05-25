using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct ClusterAttributes
{
    public float x_zero;  //relative coords of left and up border
    public float y_zero;
    public float x_coor;
    public float y_coor;

    public ClusterAttributes(float xm, float ym, float x, float y)
    {
        x_zero = xm;
        y_zero = ym;
        x_coor = x;
        y_coor = y;
    }
}

public class Minimap : MonoBehaviour
{
    private List<GameObject> clusterHolderList = new List<GameObject>();    //game objects to parent room transforms
    private List<ClusterAttributes> clusterAttributesList = new List<ClusterAttributes>();
    private Dictionary<int, GameObject> roomDict = new Dictionary<int, GameObject>();
    private int clusterNum = 1;
    private int current_clust = 0;
    private int mergedClustersNum = 0; //for equation for new cluster position
    private float shiftUnit = Convert.ToSingle(0.5);
    private GameObject roomObjectPrefab;
    private GameObject currentRoomObjectPrefab;
    private GameObject currentRoomObjectInstance;

    // Start is called before the first frame update
    void Start()
    {
        roomObjectPrefab = Resources.Load<GameObject>("Objects/Minimap_room");
        currentRoomObjectPrefab = Resources.Load<GameObject>("Objects/Minimap_current");

        MapManager.Instance.SetMinimap(this);

        GameObject firstCluster = new GameObject("Cluster" + clusterNum);
        firstCluster.transform.SetParent(transform);
        firstCluster.transform.localPosition = new Vector3(0, 0, 0);

        clusterHolderList.Add(firstCluster);
        clusterAttributesList.Add(new ClusterAttributes(0, 0, 0, 0));

        currentRoomObjectInstance = Instantiate(currentRoomObjectPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        currentRoomObjectInstance.transform.SetParent(firstCluster.transform);
        currentRoomObjectInstance.transform.localPosition = new Vector3(0, 0, 0);

        roomDict.Add(MapManager.Instance.GetRoom().variant, AddRoom(0, 0, MapManager.Instance.GetRoom()));
    }

    //private void Update()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        currentRoomObjectInstance.transform.SetParent(transform);
    //    }
    //}

    private GameObject AddRoom(float x, float y, RoomInfo current)
    {
        GameObject holder = clusterHolderList[current_clust];
        ClusterAttributes att = clusterAttributesList[current_clust];
        GameObject go = Instantiate(roomObjectPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        go.transform.SetParent(holder.transform);
        go.transform.localPosition = new Vector3(x, y, 0);

        if (x < att.x_zero)
        {
            holder.transform.Translate(new Vector3(att.x_zero - x, 0, 0));
            att.x_zero = x;
        }
        if (y > att.y_zero)
        {
            holder.transform.Translate(new Vector3(0, att.y_zero - y, 0));
            att.y_zero = y;
        }
        
        clusterAttributesList[current_clust] = att;

        current.visited = current_clust;

        return go;
    }

    //current - room we are going into
    public void MoveCurrent(string dir, RoomInfo current)
    {
        ClusterAttributes att = clusterAttributesList[current_clust];
        bool teleport = false;

        switch (dir)
        {
            case "left":
                att.x_coor-=shiftUnit;
                teleport = MapManager.Instance.GetRoom().leftTel; //room we're leaving
                break;
            case "right":
                att.x_coor+=shiftUnit;
                teleport = MapManager.Instance.GetRoom().rightTel;
                break;
            case "up":
                att.y_coor+=shiftUnit;
                teleport = MapManager.Instance.GetRoom().upTel;
                break;
            case "down":
                att.y_coor-=shiftUnit;
                teleport = MapManager.Instance.GetRoom().downTel;
                break;
        }

        clusterAttributesList[current_clust] = att;

        //going to the next unvisited room
        if (current.visited == -1 && !teleport)
        {
            roomDict.Add(current.variant, AddRoom(att.x_coor, att.y_coor, current));
            currentRoomObjectInstance.transform.localPosition = new Vector3(att.x_coor, att.y_coor, 0);
        }
        //going to an unvisited room via teleporter
        else if (current.visited == -1 && teleport)
        {
            clusterNum++;
            current_clust = clusterNum - 1;
            GameObject nextCluster = new GameObject("Cluster" + clusterNum);
            nextCluster.transform.SetParent(transform);
            nextCluster.transform.localPosition = new Vector3(0, -(5*(clusterNum - 1 - mergedClustersNum)*shiftUnit), 0);

            currentRoomObjectInstance.transform.SetParent(nextCluster.transform);

            clusterHolderList.Add(nextCluster);
            clusterAttributesList.Add(new ClusterAttributes(0, 0, 0, 0));

            roomDict.Add(current.variant, AddRoom(0, 0, current));

            currentRoomObjectInstance.transform.localPosition = new Vector3(0, 0, 0);
        }
        //going to a visited room of same cluster normally
        else if (current.visited == current_clust && !teleport)
        {
            currentRoomObjectInstance.transform.localPosition = new Vector3(att.x_coor, att.y_coor, 0);
        }
        //going to a visited room of different cluster normally
        else if (current.visited != current_clust && !teleport)
        {
            //placing holders on one height
            if (current_clust > current.visited)
                clusterHolderList[current_clust].transform.localPosition = new Vector3(clusterHolderList[current_clust].transform.localPosition.x, clusterHolderList[current.visited].transform.localPosition.y, 0);
            else
                clusterHolderList[current.visited].transform.localPosition = new Vector3(clusterHolderList[current.visited].transform.localPosition.x, clusterHolderList[current_clust].transform.localPosition.y, 0);

            currentRoomObjectInstance.transform.localPosition = new Vector3(att.x_coor, att.y_coor, 0);
            Vector3 first_cluster_pos = currentRoomObjectInstance.transform.position;
            Vector3 sec_cluster_pos = roomDict[current.variant].transform.position;

            //adjecting holders, so rooms are in correct order
            if (first_cluster_pos.x < sec_cluster_pos.x)
                clusterHolderList[current_clust].transform.Translate(new Vector3(sec_cluster_pos.x - first_cluster_pos.x, 0, 0));
            else
                clusterHolderList[current.visited].transform.Translate(new Vector3(first_cluster_pos.x - sec_cluster_pos.x, 0, 0));

            if (first_cluster_pos.y > sec_cluster_pos.y)
                clusterHolderList[current_clust].transform.Translate(new Vector3(0, sec_cluster_pos.y - first_cluster_pos.y, 0));
            else
                clusterHolderList[current.visited].transform.Translate(new Vector3(0, first_cluster_pos.y - sec_cluster_pos.y, 0));

            int upperHolder;
            int downerHolder;

            //pin rooms from one cluster to another
            if(current_clust > current.visited)
            {
                while(clusterHolderList[current_clust].transform.childCount > 0)
                    clusterHolderList[current_clust].transform.GetChild(0).SetParent(clusterHolderList[current.visited].transform);

                MapManager.Instance.ChangeCluster(current_clust, current.visited);

                upperHolder = current.visited;
                downerHolder = current_clust;

                current_clust = current.visited;
            }
            else
            {
                while (clusterHolderList[current.visited].transform.childCount > 0)
                    clusterHolderList[current.visited].transform.GetChild(0).SetParent(clusterHolderList[current_clust].transform);

                MapManager.Instance.ChangeCluster(current.visited, current_clust);

                upperHolder = current_clust; 
                downerHolder = current.visited;
            }

            //correct x_coor and y_coor atributes
            ClusterAttributes clust = clusterAttributesList[current_clust];
            clust.x_coor = currentRoomObjectInstance.transform.localPosition.x;
            clust.y_coor = currentRoomObjectInstance.transform.localPosition.y;
            clusterAttributesList[current_clust] = clust;

            //move remaining holders
            if (clusterNum - 1 > downerHolder)
                for (int i = downerHolder + 1; i < clusterNum; i++)
                    clusterHolderList[i].transform.Translate(new Vector3(0, 5, 0));

            mergedClustersNum++;
        }
        else if (teleport)
        {
            //going to a visited room of different cluster via teleport
            if (current.visited != current_clust)
            {
                current_clust = current.visited;
                currentRoomObjectInstance.transform.SetParent(clusterHolderList[current_clust].transform);
            }
            //going to a visited room of same cluster via teleport
            ClusterAttributes new_att = clusterAttributesList[current_clust];
            Vector3 pos_for_cur = roomDict[current.variant].transform.localPosition;

            new_att.x_coor = pos_for_cur.x;
            new_att.y_coor = pos_for_cur.y;

            clusterAttributesList[current_clust] = new_att;
            currentRoomObjectInstance.transform.localPosition = pos_for_cur;
        }
    }

    public void Hide()
    {
        foreach (var cluster in clusterHolderList)
            cluster.SetActive(false);
    }

    public void Show()
    {
        foreach (var cluster in clusterHolderList)
            cluster.SetActive(true);
    }
}
