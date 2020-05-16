using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellHolder : FightAtributeHolder
{
    protected override void Start()
    {
        base.Start();

        AddSpells();
        Deactivate();
    }
}
