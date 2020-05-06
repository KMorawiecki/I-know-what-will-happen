using UnityEngine;
using System.Collections;

public class Wizard: MonoBehaviour {

    private float      m_speed = 50f;

    private Animator            m_animator;
    private Rigidbody2D         m_body2d;
    private bool                m_grounded = false;
    private bool                m_combatIdle = false;
    private bool                m_isDead = false;
    private bool                moving = false;
    private bool                swap_dir = false;

    private int health;
    private int integrity;
    private int mana;

    // Use this for initialization
    void Start () {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
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

    public bool IsInBattle()
    {
        return m_combatIdle;
    }
}
