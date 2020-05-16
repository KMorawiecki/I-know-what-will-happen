using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleButton : MonoBehaviour
{
    private int elevate = 5;

    public void OnMouseOverFun()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + elevate);
    }

    public void OnMouseExitFun()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - elevate);
    }
}
