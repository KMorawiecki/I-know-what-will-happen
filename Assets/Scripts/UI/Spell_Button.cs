using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_Button : MonoBehaviour
{
    //private RaycastHit2D rh2d;

    //private void Start()
    //{
    //    rh2d = new RaycastHit2D();
    //}

    public void InvokeSpell()
    {
        InvokeExistentialPurge();
        //transform.GetComponentInParent<FightAtributeHolder>().BlockUI();
        BattleManager.Instance.BlockSwordAndStaff();
        transform.GetComponentInParent<FightAtributeHolder>().Deactivate();
    }

    public void InvokeExistentialPurge()
    {
        GameObject go = new GameObject("ExistentialPurge");
        ExistentialPurge ep = go.AddComponent<ExistentialPurge>();

        foreach(Enemy enemy in MapManager.Instance.GetEnemies())
            enemy.SetTargeting(ep);

        BattleManager.Instance.SetSpell(go);
    }
}
