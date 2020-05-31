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
        monsterSpeed = 10f;
    }

    public override IEnumerator Turn()
    {
        //check if goblin is close enaugh to initialise attack
        if (Math.Abs(gameObject.transform.position.x - playerInst.transform.position.x) + Math.Abs(gameObject.transform.position.y - player.transform.position.y) <= BattleManager.Instance.ConvBoardToReal(1))
        {
            yield return StartCoroutine(Attack());
            yield return StartCoroutine(MoveAway());
        }
        else
        {
            yield return StartCoroutine(MoveTowards());
            if (Math.Abs(gameObject.transform.position.x - playerInst.transform.position.x) + Math.Abs(gameObject.transform.position.y - player.transform.position.y) <= BattleManager.Instance.ConvBoardToReal(1))
                Attack();
        }
    }

    protected override IEnumerator Attack()
    {
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.5f);
        player.Damage(hitPoints);

        yield return StartCoroutine(WaitForAnimation(animator, "Attack"));
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length + animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
    }

    //private IEnumerator MovePlayer(Vector2 click_pos)
    //{
    //    if (click_pos.x <= 7 && click_pos.x >= -7 && click_pos.y <= 4 && click_pos.y >= -4)
    //    {
    //        playerMoving = true;

    //        switch (click_pos.x >= player.transform.position.x)
    //        {
    //            case true:
    //                player.SetRight();
    //                break;
    //            case false:
    //                player.SetLeft();
    //                break;
    //        }
    //        player.SetMoving(true);

    //        Vector3 destination = room.transform.GetComponent<Grid>().CellToWorld(new Vector3Int(Convert.ToInt32(click_pos.x), Convert.ToInt32(click_pos.y), 0));

    //        yield return StartCoroutine(MovementManager.Instance.SmoothMovement(player.gameObject, destination, player_speed));

    //        player_x = (int)click_pos.x;
    //        player_y = (int)click_pos.y;

    //        player.SetMoving(false);

    //        playerMoving = false;
    //        playerTurn = false;

    //        UnsetupMovementUI();

    //        //TODO: will enable when turns are implemented
    //        //GameManager.Instance.SetMove(false);
    //        //moveUI = false;
    //        //move_butt.interactable = false;
    //        //moveBlock = true;
    //    }

    //    yield return null;
    //}

    protected override IEnumerator MoveTowards(List<string> disabledDir = null)
    {
        if (disabledDir == null)
            disabledDir = new List<string>();

        if(disabledDir.Count == 3)
            yield break;

        if (Math.Abs(gameObject.transform.position.x - playerInst.transform.position.x) >= Math.Abs(gameObject.transform.position.y - playerInst.transform.position.y))
        {
            if (gameObject.transform.position.x - playerInst.transform.position.x > 0)
                yield return StartCoroutine(CheckAndGo(disabledDir, "left", new Vector3(-2.4f, 0, 0)));

            if (gameObject.transform.position.y - playerInst.transform.position.y > 0)
                yield return StartCoroutine(CheckAndGo(disabledDir, "down", new Vector3(0, -2.5f, 0)));
            else
                yield return StartCoroutine(CheckAndGo(disabledDir, "up", new Vector3(0, 2.5f, 0)));
        }
        else
        {
            if (gameObject.transform.position.y - playerInst.transform.position.y > 0)
                yield return StartCoroutine(CheckAndGo(disabledDir, "down", new Vector3(0, -2.5f, 0)));

            if (gameObject.transform.position.x - playerInst.transform.position.x > 0)
                yield return StartCoroutine(CheckAndGo(disabledDir, "left", new Vector3(-2.4f, 0, 0)));
            else
                yield return StartCoroutine(CheckAndGo(disabledDir, "right", new Vector3(2.4f, 0, 0)));
        }

        yield return null;
    }

    private IEnumerator CheckAndGo(List<string> disabledDir, string newDir, Vector3 newVec)
    {
        if (!disabledDir.Contains(newDir))
        {
            if (CheckForCollision(newDir))
            {
                yield return StartCoroutine(MoveMonster(transform.position + newVec));
                StopAllCoroutines();
            }
            else
            {
                disabledDir.Add(newDir);
                yield return StartCoroutine(MoveTowards(disabledDir));
                yield break;
            }
        }
    }

    protected override IEnumerator MoveAway()
    {
        Debug.Log("lol, not today");
        yield return null;
        //player.
    }
}
