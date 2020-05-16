using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingHolder : FightAtributeHolder
{
    protected override void Start()
    {
        base.Start();

        Deactivate();
    }
}
