using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public string animationState;

    protected int healthPoints;
    protected int hitPoints;
    protected float monsterSpeed; 

    protected Animator animator;
    protected GameObject playerInst;
    protected Wizard player;

    private bool hit_trigger = false;
    private bool enable_targeting = false;
    private bool isMoving = false;
    private Spell spell_to_hit;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        playerInst = GameObject.Find("LightBandit(Clone)");
        player = playerInst.GetComponent<Wizard>();
    }

    protected virtual void Update()
    {
        if(hit_trigger)
        {
            Hurt();
            hit_trigger = false;
        }

        if (isMoving)
            animator.SetInteger("AnimState", 2);
        else
            animator.SetInteger("AnimState", 0);
    }

    private void OnMouseDown()
    {
        if(enable_targeting)
        {
            spell_to_hit.SpellBehaviour(this);
            spell_to_hit.EnemyBehaviour(this);
        }
    }

    public abstract IEnumerator Turn();
    protected abstract IEnumerator Attack();
    protected abstract IEnumerator MoveAway();
    protected abstract IEnumerator MoveTowards(List<string> disabledDir);

    public int GetHealth()
    {
        return healthPoints;
    }

    public void DealDamage(int damage)
    {
        healthPoints -= damage;
    }

    public void SetTargeting(Spell spell)
    {
        enable_targeting = true;
        spell_to_hit = spell;
    }

    public void ResetTargeting()
    {
        enable_targeting = false;
        spell_to_hit = null;
    }

    protected void Hurt()
    {
        animator.SetTrigger("Hurt");
    }

    protected void Kill()
    {
        animator.SetTrigger("Death");
        MapManager.Instance.DisposeOfEnemy(this);
    }

    public bool IsAnimatorInState(string state)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(state);
    }

    public void SetHurtTrigger()
    {
        hit_trigger = true;
    }

    public AnimationClip GetAnimationClip(string name)
    {
        if (!animator) return null; // no animator

        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == name)
            {
                return clip;
            }
        }
        return null; // no clip by that name
    }

    //protected static IEnumerator WhilePlaying(this Animation animation,
    //                                           string animationName)
    //{
    //    animation.PlayQueued(animationName);
    //    yield return animation.WhilePlaying();
    //}

    protected IEnumerator WaitForAnimation(Animator animator, string name)
    {
        do
        {
            yield return null;
        } while (!animator.GetCurrentAnimatorStateInfo(0).IsName(name));
    }

    protected IEnumerator MoveMonster(Vector3 destination)
    {
        isMoving = true;
        yield return StartCoroutine(MovementManager.Instance.SmoothMovement(gameObject, destination, monsterSpeed));
        isMoving = false;
    }

    //return: true - good to go, false - obstacle
    protected bool CheckForCollision(string dir)
    {
        Vector2 destination = new Vector2();

        switch (dir)
        {
            case "up":
                destination = new Vector2(transform.position.x, transform.position.y + 2.5f);
                break;
            case "down":
                destination = new Vector2(transform.position.x, transform.position.y - 2.5f);
                break;
            case "left":
                destination = new Vector2(transform.position.x - 2.4f, transform.position.y);
                break;
            case "right":
                destination = new Vector2(transform.position.x + 2.4f, transform.position.y);
                break;
        }

        RaycastHit2D hit = Physics2D.Raycast(destination, Vector2.zero);
        if (hit.collider != null)
            return false;
        return true;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}
