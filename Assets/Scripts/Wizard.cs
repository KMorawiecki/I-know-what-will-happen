using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wizard: MonoBehaviour {

    private float      m_speed = 50f;

    private Animator            m_animator;
    private Rigidbody2D         m_body2d;
    private bool                m_grounded = false;
    private bool                m_combatIdle = false;
    private bool                m_isDead = false;
    private bool                moving = false;
    private bool                swap_dir = false;

    private int health = 100;
    private int integrity = 100;
    private int mana = 100;

    private WizardAttribute health_bar;
    private WizardAttribute integrity_bar;
    private WizardAttribute mana_bar;

    private List<string> playerSpells;
    private int movement_range;

    // Use this for initialization
    void Start () {
        health_bar = UIManager.Instance.GetBar("health");
        mana_bar = UIManager.Instance.GetBar("health");
        integrity_bar = UIManager.Instance.GetBar("health");

        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        playerSpells = new List<string>();
        movement_range = 1;

        playerSpells.Add("ExistentialPurge");
    }
	
	// Update is called once per frame
	void Update () {

        // -- Handle input and movement --
        float inputX = Input.GetAxis("Horizontal");

        // Swap direction of sprite depending on walk direction
        if (swap_dir)
            GetComponent<SpriteRenderer>().flipX = true;
        else
            GetComponent<SpriteRenderer>().flipX = false;

        // Move
        if (moving)
            m_animator.SetInteger("AnimState", 2);
        else
            m_animator.SetInteger("AnimState", 0);

        //Combat Idle
        if (m_combatIdle && !moving)
            m_animator.SetInteger("AnimState", 1);

        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeed", m_body2d.velocity.y);

        // -- Handle Animations --
        //Death
        //if (Input.GetKeyDown("e")) {
        //    if(!m_isDead)
        //        m_animator.SetTrigger("Death");
        //    else
        //        m_animator.SetTrigger("Recover");

        //    m_isDead = !m_isDead;
        //}



        ////Hurt
        //else if (Input.GetKeyDown("q"))
        //    m_animator.SetTrigger("Hurt");

        ////Attack
        //else if(Input.GetMouseButtonDown(0)) {
        //    m_animator.SetTrigger("Attack");
        //}

        ////Change between idle and combat idle
        //else if (Input.GetKeyDown("f"))
        //    m_combatIdle = !m_combatIdle;


    }

    public float GetSpeed()
    {
        return m_speed;
    }

    public void SetMoving(bool setter)
    {
        moving = setter;
    }

    public bool IsMoving()
    {
        return moving;
    }

    public void SetLeft()
    {
        swap_dir = false;
    }

    public void SetRight()
    {
        swap_dir = true;
    }

    public void BattleStance()
    {
       // m_animator.SetInteger("AnimState", 1);
        m_combatIdle = true;
    }

    public void NormalStance()
    {
        m_combatIdle = false;
    }

    public bool IsInBattle()
    {
        return m_combatIdle;
    }

    public List<string> GetSpells()
    {
        return playerSpells;
    }

    public int GetMovementRange()
    {
        return movement_range;
    }

    public void Damage(int points)
    {
        m_animator.SetTrigger("Hurt");
        if (points >= health)
        {
            health_bar.ChangeValue(false, health);
            Invoke("Die", 1f);
        }
        else
            health_bar.ChangeValue(false, points);
    }

    public void Die()
    {
        m_animator.SetTrigger("death");
    }
}
