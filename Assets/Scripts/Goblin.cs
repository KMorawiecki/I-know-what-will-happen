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
    }

}
