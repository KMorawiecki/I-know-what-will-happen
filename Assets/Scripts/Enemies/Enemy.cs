using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public string animationState;

    protected int healthPoints;
    protected MovementManager moveManager;
    protected Animator animator;

    private bool hit_trigger = false;
    private bool enable_targeting = false;
    private Spell spell_to_hit;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        if(hit_trigger)
        {
            Hurt();
            hit_trigger = false;
        }
    }

    private void OnMouseDown()
    {
        if(enable_targeting)
        {
            spell_to_hit.SpellBehaviour(this);
            spell_to_hit.EnemyBehaviour(this);
        }
    }

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
        GameObject.Find("MapManager").GetComponent<MapManager>().DisposeOfEnemy(this);
    }

    public bool IsAnimatorInState(string state)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(state);
    }

    public void SetHurtTrigger()
    {
        hit_trigger = true;
    }
}
