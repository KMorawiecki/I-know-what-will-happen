using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int level = 1;
    private Wizard wizPrefab;
    private Wizard wizInst;
    private PlayerPossession possession;

    private Goblin goblinPrefab;

    private bool battle_move;
    private bool battle_fight;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        possession = new PlayerPossession();

        InitDungeon newInit = new InitDungeon(level);
        wizPrefab = Resources.Load<Wizard>("Objects/LightBandit");
        wizInst = Instantiate(wizPrefab, new Vector3(0, 0, -1), Quaternion.identity);

        MovementManager.Instance.SetPlayer(wizInst);

        goblinPrefab = Resources.Load<Goblin>("Objects/Goblin");
    }

    public void DisableRoom()
    {
        GameObject cur_room = MapManager.Instance.GetRoomInstance();
        cur_room.SetActive(false);
        wizInst.gameObject.SetActive(false);
    }

    public void EnableRoom()
    {
        GameObject cur_room = MapManager.Instance.GetRoomInstance();
        cur_room.SetActive(true);
        wizInst.gameObject.SetActive(true);
    }

    public void CloseCurrentTab()
    {
        GameObject cur_tab = GameObject.Find("CurrentTab");
        Destroy(cur_tab);
        EnableRoom();
    }

    public Enemy GetMonster(string name)
    {
        return goblinPrefab;
    }

    //////////////////////////////////BATTLE METHODS
    public void ChangeToBattle()
    {
        battle_move = false;
        battle_fight = false;

        UIManager.Instance.ChangeToBattle();
        wizInst.BattleStance();
    }

    public void ChangeToNormal()
    {
        battle_move = false;
        battle_fight = false;

        UIManager.Instance.ChangeToNormal();
        wizInst.NormalStance();
    }

    public Wizard GetPlayer()
    {
        return wizInst;
    }

    public void SetMove(bool setter)
    {
        battle_move = setter;
    }

    public void SetFight(bool setter)
    {
        battle_fight = setter;
    }

    public bool GetMove()
    {
        return battle_move;
    }

    public bool GetFight()
    {
        return battle_fight;
    }
}
