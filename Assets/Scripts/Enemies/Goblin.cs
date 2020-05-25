using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : Enemy
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        healthPoints = 10;
        hitPoints = 5;
    }

    public override IEnumerator Turn()
    {
        //check if goblin is close enaugh to initialise attack
        if (Math.Abs(gameObject.transform.position.x - playerInst.transform.position.x) + Math.Abs(gameObject.transform.position.y - player.transform.position.y) <= 1)
        {
            yield return StartCoroutine(Attack());
            yield return StartCoroutine(MoveAway());
        }
        else
        {
            yield return StartCoroutine(MoveTowards());
            if (Math.Abs(gameObject.transform.position.x - playerInst.transform.position.x) + Math.Abs(gameObject.transform.position.y - player.transform.position.y) <= 1)
                Attack();
        }
    }

    protected override IEnumerator Attack()
    {
        animator.SetTrigger("Attack");
        player.Damage(hitPoints);

        //yield return StartCoroutine(WaitForAnimation())
        //yield return new WaitForSeconds(GetCurrentAnimatorStateInfo(0).length + anim.GetCurrentAnimatorStateInfo(0).normalizedTime););

        yield return null;
    }

    protected override IEnumerator MoveTowards()
    {
        Debug.Log("gonna get you bitch");
        yield return null;
        //player.
    }

    protected override IEnumerator MoveAway()
    {
        Debug.Log("lol, not today");
        yield return null;
        //player.
    }
}
