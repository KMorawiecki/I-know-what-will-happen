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
    private UIManager uiManager;

    private Goblin goblinPrefab;

    private bool battle_move;
    private bool battle_fight;

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

        GameObject UI_man_go = new GameObject("UIManager");
        uiManager = UI_man_go.AddComponent<UIManager>();

        goblinPrefab = Resources.Load<Goblin>("Objects/Goblin");
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


    //////////////////////////////////BATTLE METHODS
    public void ChangeToBattle()
    {
        battle_move = false;
        battle_fight = false;

        uiManager.ChangeToBattle();
        wizInst.BattleStance();

        GameObject go = new GameObject();
        BattleManager battleManager = go.AddComponent<BattleManager>();
    }

    public Wizard GetPlayer()
    {
        return wizInst;
    }

    public void SetMove()
    {
        battle_move = true;
    }

    public void SetFight()
    {
        battle_fight = true;
    }

    public bool GetMove()
    {
        return battle_move;
    }

    public bool GetFight()
    {
        return battle_fight;
    }

    public GameObject GetRoom()
    {
        return mapManager.GetRoomInstance();
    }

    public Enemy GetMonster(string name)
    {
        return goblinPrefab;
    }
}
