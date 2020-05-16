using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_Button : MonoBehaviour
{
    private MapManager map_manager;
    private BattleManager battleManager;
    private RaycastHit2D rh2d;

    private void Start()
    {
        map_manager = GameObject.Find("MapManager").GetComponent<MapManager>();
        battleManager = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        rh2d = new RaycastHit2D();
    }

    public void InvokeSpell()
    {
        InvokeExistentialPurge();
        //transform.GetComponentInParent<FightAtributeHolder>().BlockUI();
        battleManager.BlockSwordAndStaff();
        transform.GetComponentInParent<FightAtributeHolder>().Deactivate();
    }

    public void InvokeExistentialPurge()
    {
        GameObject go = new GameObject("ExistentialPurge");
        ExistentialPurge ep = go.AddComponent<ExistentialPurge>();

        foreach(Enemy enemy in map_manager.GetEnemies())
        {
            enemy.SetTargeting(ep);
        }

        battleManager.SetSpell(go);
    }
}
