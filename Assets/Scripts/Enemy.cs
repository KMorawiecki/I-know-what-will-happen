using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    protected int healthPoints;
    protected MovementManager moveManager;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }

    public int GetHealth()
    {
        return healthPoints;
    }

    public void DealDamage(int damage)
    {
        healthPoints -= damage;
    }
}
