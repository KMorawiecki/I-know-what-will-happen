using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class MapManager : MonoBehaviour
{
    private string path;
    private string jsonString;
    private RoomInfo[] map;
    private int currentRoom = 0;
    private float shakeTime = Convert.ToSingle(10);
    private bool doorMoving = false;
    private GameObject closedDoor;
    private GameObject openedDoor;

    private GameObject upDoorInstance;
    private GameObject downDoorInstance;
    private GameObject leftDoorInstance;
    private GameObject rightDoorInstance;
    private GameObject currentRoomInstance;

    public GameObject firstRoom;
    public GameObject secondRoom;

    // Start is called before the first frame update
    void Start()
    {
        path = Application.dataPath + "/Data/DungeonMap.json";
        jsonString = File.ReadAllText(path);
        map = JsonHelper.FromJson<RoomInfo>(jsonString);
        closedDoor = Resources.Load<GameObject>("Objects/Closed_door");
        openedDoor = Resources.Load<GameObject>("Objects/Opened_door");

        UpdateRoom(currentRoom);
    }

    public void UpdateRoom(int newroom)
    {
        currentRoom = newroom;

        RoomInfo roomInfo = map[currentRoom];

        DestroyObject(currentRoomInstance);
        DestroyObject(leftDoorInstance);
        DestroyObject(rightDoorInstance);
        DestroyObject(upDoorInstance);
        DestroyObject(downDoorInstance);

        currentRoomInstance = InstantiateSelector(Convert.ToString(roomInfo.variant), "0", "1", firstRoom, secondRoom, new Vector3(0, 0, 0));
        upDoorInstance = InstantiateSelector(roomInfo.uplock, "locked", "unlocked", closedDoor, openedDoor, new Vector3(0, Convert.ToSingle(3.5), 0));
        downDoorInstance = InstantiateSelector(roomInfo.downlock, "locked", "unlocked", closedDoor, openedDoor, new Vector3(0, Convert.ToSingle(-3.5), 0));
        leftDoorInstance = InstantiateSelector(roomInfo.leftlock, "locked", "unlocked", closedDoor, openedDoor, new Vector3(Convert.ToSingle(- 4.5), 0, 0));
        rightDoorInstance = InstantiateSelector(roomInfo.rightlock, "locked", "unlocked", closedDoor, openedDoor, new Vector3(Convert.ToSingle(4.5), 0, 0));
    }

    private void DestroyObject(GameObject obj)
    {
        if (obj != null)
            Destroy(obj);
    }

    //so I dont have to write 10 times same thing
    private GameObject InstantiateSelector(string givenString, string firstOption, string secondOption, GameObject firstObject, GameObject secondObject, Vector3 position)
    {
        if (firstOption == givenString)
            return Instantiate(firstObject, position, Quaternion.identity);
        else if (secondOption == givenString)
            return Instantiate(secondObject, position, Quaternion.identity);
        else
        {
            Debug.Log("Incorrect value to instantiate");
            return null;
        }
    }

    public bool IsDoorOpen(string dir)
    {
        switch(dir)
        {
            case "up":
                return map[currentRoom].uplock == "locked" ? false : true;
            case "down":
                return map[currentRoom].downlock == "locked" ? false : true;
            case "left":
                return map[currentRoom].leftlock == "locked" ? false : true;
            case "right":
                return map[currentRoom].rightlock == "locked" ? false : true;
        }

        return false;
    }

    public RoomInfo GetRoom()
    {
        return map[currentRoom];
    }

    public IEnumerator ShakeThatDoor(string dir)
    {
        GameObject doorToShake = null;
        Vector3 currentPosition;
        Vector3 leftPosition;
        Vector3 rightPosition;

        doorMoving = true;

        switch (dir)
        {
            case "up":
                doorToShake = upDoorInstance;
                break;
            case "down":
                doorToShake = downDoorInstance;
                break;
            case "left":
                doorToShake = leftDoorInstance;
                break;
            case "right":
                doorToShake = rightDoorInstance;
                break;
        }

        currentPosition = doorToShake.transform.position;
        leftPosition = new Vector3(currentPosition.x - Convert.ToSingle(0.25), currentPosition.y, 0);
        rightPosition = new Vector3(currentPosition.x + Convert.ToSingle(0.25), currentPosition.y, 0);

        StartCoroutine(SmoothMovement(doorToShake, leftPosition));
        yield return new WaitUntil(() => doorToShake.transform.position == leftPosition);
        StartCoroutine(SmoothMovement(doorToShake, rightPosition));
        yield return new WaitUntil(() => doorToShake.transform.position == rightPosition);
        StartCoroutine(SmoothMovement(doorToShake, leftPosition));
        yield return new WaitUntil(() => doorToShake.transform.position == leftPosition);
        StartCoroutine(SmoothMovement(doorToShake, currentPosition));

        doorMoving = false;

        yield return null;
    }

    //Co-routine for moving units from one space to next, takes a parameter end to specify where to move to.
    protected IEnumerator SmoothMovement(GameObject obj, Vector3 end)
    {
        //Calculate the remaining distance to move based on the square magnitude of the difference between current position and end parameter. 
        //Square magnitude is used instead of magnitude because it's computationally cheaper.
        float sqrRemainingDistance = (obj.transform.position - end).sqrMagnitude;
        Rigidbody2D rgbd = obj.GetComponent<Rigidbody2D>();

        //While that distance is greater than a very small amount (Epsilon, almost zero):
        while (sqrRemainingDistance > float.Epsilon)
        {
            //Find a new position proportionally closer to the end, based on the moveTime
            Vector3 newPostion = Vector3.MoveTowards(rgbd.position, end, shakeTime * Time.deltaTime);

            //Call MovePosition on attached Rigidbody2D and move it to the calculated position.
            rgbd.MovePosition(newPostion);

            //Recalculate the remaining distance after moving.
            sqrRemainingDistance = (obj.transform.position - end).sqrMagnitude;

            //Return and loop until sqrRemainingDistance is close enough to zero to end the function
            yield return null;
        }

        yield return null;
    }

    public bool isDoorMoving()
    {
        return doorMoving;
    }
}
