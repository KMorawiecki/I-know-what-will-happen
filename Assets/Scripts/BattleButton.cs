using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleButton : MonoBehaviour
{
    private int elevate = 5;

    public void OnMouseOverFun()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + elevate);
    }

    public void OnMouseExitFun()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y - elevate);
    }
}
