using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    public MapManager mapManager;
    private bool movementCooldown = false;
    private string dir;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!movementCooldown && !mapManager.isDoorMoving())
        {
            if (Input.GetKey("up"))
                dir = "up";
            if (Input.GetKey("down"))
                dir = "down";
            if (Input.GetKey("right"))
                dir = "right";
            if (Input.GetKey("left"))
                dir = "left";

            TryToMove(dir);
        }

        try
        {
            if (Input.GetKeyUp(dir))
            {
                movementCooldown = false;
                dir = null;
            }
        }
        catch (ArgumentException e)
        { }
    }

    IEnumerator DelayAction()
    {
        yield return new WaitForSeconds(1);
    }

    void TryToMove(string dir)
    {
        if (dir == null)
            return;
        if(mapManager.IsDoorOpen(dir))
        {
            movementCooldown = true;

            int newroom = 0;

            switch(dir)
            {
                case "left":
                    newroom = mapManager.GetRoom().left;
                    break;
                case "right":
                    newroom = mapManager.GetRoom().right;
                    break;
                case "down":
                    newroom = mapManager.GetRoom().down;
                    break;
                case "up":
                    newroom = mapManager.GetRoom().up;
                    break;
                default:
                    Debug.Log("Incorrect input");
                    break;
            }

            mapManager.UpdateRoom(newroom);
        }
        else
            StartCoroutine(mapManager.ShakeThatDoor(dir));
    }
}
