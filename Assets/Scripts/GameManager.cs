using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using System;

public class GameManager : MonoBehaviour
{
    private int level = 1;
    private Wizard wizPrefab;
    private Wizard wizInst;
    private MovementManager movementManager;
    private MapManager mapManager;

    private void Start()
    {
        InitDungeon newInit = new InitDungeon(level);
        wizPrefab = Resources.Load<Wizard>("Objects/LightBandit");
        wizInst = Instantiate(wizPrefab, new Vector3(0, 0, -1), Quaternion.identity);

        GameObject go = GameObject.Find("MovementManager");
        if (go != null)
        {
            movementManager = go.GetComponent<MovementManager>();
            movementManager.SetPlayer(wizInst);
        }

        go = GameObject.Find("MapManager");
        if (go != null)
            mapManager = go.GetComponent<MapManager>();
    }

    public void DisableRoom()
    {
        GameObject cur_room = mapManager.GetRoomInstance();
        cur_room.SetActive(false);
        wizInst.gameObject.SetActive(false);
    }

    public void EnableRoom()
    {
        GameObject cur_room = mapManager.GetRoomInstance();
        cur_room.SetActive(true);
        wizInst.gameObject.SetActive(true);
    }

    public void CloseCurrentTab()
    {
        GameObject cur_tab = GameObject.Find("CurrentTab");
        Destroy(cur_tab);
        EnableRoom();
    }
}
