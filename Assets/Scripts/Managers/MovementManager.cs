using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    public static MovementManager Instance { get; private set; }

    private Wizard player;
    private bool movementCooldown = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
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

        if (MapManager.Instance.IsDoorOpen(dir))
        {
            int newroom = 0;

            switch (dir)
            {
                case "left":
                    newroom = MapManager.Instance.GetRoom().left;
                    break;
                case "right":
                    newroom = MapManager.Instance.GetRoom().right;
                    break;
                case "down":
                    newroom = MapManager.Instance.GetRoom().down;
                    break;
                case "up":
                    newroom = MapManager.Instance.GetRoom().up;
                    break;
                default:
                    Debug.Log("Incorrect input");
                    break;
            }

            player.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            StartCoroutine(MapManager.Instance.UpdateRoom(newroom, dir));
            player.transform.position = new Vector3(MapManager.Instance.GetStartingPosition().x, MapManager.Instance.GetStartingPosition().y, -1);
            if (MapManager.Instance.GetStartingPosition().x < 0)
                player.SetRight();
            else if (MapManager.Instance.GetStartingPosition().x > 0)
                player.SetLeft();
            yield return new WaitUntil(() =>  MapManager.Instance.IsRoomMoving() == false);
            player.gameObject.GetComponent<SpriteRenderer>().enabled = true;
        }
        else
            MapManager.Instance.ShakeThatDoor(dir);

        if (MapManager.Instance.GetRoom().enemy_num == 0)
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

        player.SetMoving(true);
        yield return StartCoroutine(SmoothMovement(player.gameObject, end, player.GetSpeed()));
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

            //Vector3 newPostion = rgbd.position + new Vector2(end.x, end.y)/duration;

            //Call MovePosition on attached Rigidbody2D and move it to the calculated position.
            rgbd.MovePosition(newPostion);

            //Recalculate the remaining distance after moving.
            sqrRemainingDistance = new Vector2(obj.transform.position.x - end.x, obj.transform.position.y - end.y).sqrMagnitude;

            //Return and loop until sqrRemainingDistance is close enough to zero to end the function
            yield return null;
        }

        yield return null;
    }

    public IEnumerator RashMovement(GameObject obj, Vector3 end, float duration)
    {
        float sqrRemainingDistance = new Vector2(obj.transform.position.x - end.x, obj.transform.position.y - end.y).sqrMagnitude;
        float prevSqrRemainingDistance = new Vector2(obj.transform.position.x - end.x, obj.transform.position.y - end.y).sqrMagnitude;
        Rigidbody2D rgbd = obj.GetComponent<Rigidbody2D>();
        Vector2 adder = new Vector2(end.x - rgbd.position.x, end.y - rgbd.position.y) / duration;

        //check if on place or past it
        while (sqrRemainingDistance > float.Epsilon && sqrRemainingDistance <= prevSqrRemainingDistance)
        {
            prevSqrRemainingDistance = sqrRemainingDistance;

            Vector3 newPostion = rgbd.position + adder;

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

    public void ResetMovementCooldown()
    {
        movementCooldown = false;
    }
}
