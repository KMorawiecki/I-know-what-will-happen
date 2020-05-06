using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    public MapManager mapManager;
    private Wizard player;
    private bool movementCooldown = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {         
        if (!movementCooldown && !player.IsInBattle())
        {
            if (Input.GetKey("up"))
                StartCoroutine(InRoomMovement(new Vector3(0, Convert.ToSingle(1.5), 0), "up"));
            if (Input.GetKey("down"))
                StartCoroutine(InRoomMovement(new Vector3(0, Convert.ToSingle(-1.5), 0), "down"));
            if (Input.GetKey("right"))
                StartCoroutine(InRoomMovement(new Vector3(3, 0, 0), "right"));
            if (Input.GetKey("left"))
                StartCoroutine(InRoomMovement(new Vector3(-3, 0, 0), "left"));
        }
    }

    private IEnumerator TryToMoveRoom(string dir)
    {
        yield return new WaitUntil(() => !player.IsMoving());

        if (mapManager.IsDoorOpen(dir))
        {
            int newroom = 0;

            switch (dir)
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

            player.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            StartCoroutine(mapManager.UpdateRoom(newroom, dir));
            player.transform.position = new Vector3(mapManager.GetStartingPosition().x, mapManager.GetStartingPosition().y, -1);
            if (mapManager.GetStartingPosition().x < 0)
                player.SetRight();
            else if (mapManager.GetStartingPosition().x > 0)
                player.SetLeft();
            yield return new WaitUntil(() =>  mapManager.IsRoomMoving() == false);
            player.gameObject.GetComponent<SpriteRenderer>().enabled = true;
        }
        else
            mapManager.ShakeThatDoor(dir);

        movementCooldown = false;
        yield return null;
    }

    public IEnumerator InRoomMovement(Vector3 end, string chosenDir)
    {
        movementCooldown = true;

        if (end.x > player.gameObject.transform.position.x)
            player.SetRight();
        else if (end.x < player.gameObject.transform.position.x)
            player.SetLeft();

        StartCoroutine(SmoothMovement(player.gameObject, end, player.GetSpeed()));
        player.SetMoving(true);
        yield return new WaitUntil(() => player.gameObject.transform.position.x == end.x && player.gameObject.transform.position.y == end.y);
        player.SetMoving(false);
        StartCoroutine(TryToMoveRoom(chosenDir));
        yield return null;
    }

    //Co-routine for moving units from one space to next, takes a parameter end to specify where to move to.
    public IEnumerator SmoothMovement(GameObject obj, Vector3 end, float duration)
    {
        //Calculate the remaining distance to move based on the square magnitude of the difference between current position and end parameter. 
        //Square magnitude is used instead of magnitude because it's computationally cheaper.
        float sqrRemainingDistance = new Vector2(obj.transform.position.x - end.x, obj.transform.position.y - end.y).sqrMagnitude;
        Rigidbody2D rgbd = obj.GetComponent<Rigidbody2D>();

        //While that distance is greater than a very small amount (Epsilon, almost zero):
        while (sqrRemainingDistance > float.Epsilon)
        {
            //Find a new position proportionally closer to the end, based on the moveTime
            Vector3 newPostion = Vector3.MoveTowards(rgbd.position, end, duration * Time.deltaTime);

            //Call MovePosition on attached Rigidbody2D and move it to the calculated position.
            rgbd.MovePosition(newPostion);

            //Recalculate the remaining distance after moving.
            sqrRemainingDistance = new Vector2(obj.transform.position.x - end.x, obj.transform.position.y - end.y).sqrMagnitude;

            //Return and loop until sqrRemainingDistance is close enough to zero to end the function
            yield return null;
        }

        yield return null;
    }

    public void SetPlayer(Wizard wiz)
    {
        player = wiz;
    }

    public Vector3 GetPlayerPosition()
    {
        return player.transform.position;
    }
}
