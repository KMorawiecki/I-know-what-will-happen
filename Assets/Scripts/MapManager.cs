using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    private string path;
    private string jsonString;
    private RoomInfo[] map;
    private int currentRoom = 0;
    private float shakeTime = Convert.ToSingle(10);
    private float swayingTime = Convert.ToSingle(50);
    private bool roomMoving = false;
    private Vector2 startingPosition = new Vector2(0, 0);
    private GameObject closedDoor;
    private GameObject openedDoor;
    private Minimap minimap;

    private GameObject upDoorInstance;
    private GameObject downDoorInstance;
    private GameObject leftDoorInstance;
    private GameObject rightDoorInstance;
    private GameObject currentRoomInstance;

    public GameObject firstRoom;
    public GameObject secondRoom;
    public MovementManager moveManager;

    public Text temp;

    // Start is called before the first frame update
    void Start()
    {
        path = Application.dataPath + "/Data/DungeonMap.json";
        jsonString = File.ReadAllText(path);
        map = JsonHelper.FromJson<RoomInfo>(jsonString);
        closedDoor = Resources.Load<GameObject>("Objects/Closed_door");
        openedDoor = Resources.Load<GameObject>("Objects/Opened_door");

        RoomInfo roomInfo = map[currentRoom];

        currentRoomInstance = InstantiateSelector(roomInfo.variant % 2, 0, 1, firstRoom, secondRoom, new Vector3(0, 0, 0));
        upDoorInstance = InstantiateSelector(roomInfo.uplock, -1, 1, closedDoor, openedDoor, new Vector3(0, Convert.ToSingle(3.5), 0));
        upDoorInstance.transform.parent = currentRoomInstance.transform;
        downDoorInstance = InstantiateSelector(roomInfo.downlock, -1, 1, closedDoor, openedDoor, new Vector3(0, Convert.ToSingle(-3.5), 0));
        downDoorInstance.transform.parent = currentRoomInstance.transform;
        leftDoorInstance = InstantiateSelector(roomInfo.leftlock, -1, 1, closedDoor, openedDoor, new Vector3(Convert.ToSingle(-4.5), 0, 0));
        leftDoorInstance.transform.parent = currentRoomInstance.transform;
        rightDoorInstance = InstantiateSelector(roomInfo.rightlock, -1, 1, closedDoor, openedDoor, new Vector3(Convert.ToSingle(4.5), 0, 0));
        rightDoorInstance.transform.parent = currentRoomInstance.transform;

        temp.text = Convert.ToString(roomInfo.variant);
    }

    private void Update()
    {
        
    }

    public IEnumerator UpdateRoom(int newroom, string dir)
    {
        roomMoving = true;

        RoomInfo newRoomInfo = map[newroom];
        Vector3 newRoomStart;

        minimap.WentInDirection(dir);

        if (newRoomInfo.left == currentRoom)
        {
            newRoomStart = new Vector3(20, 0, 0);
            startingPosition = new Vector2(-3, 0);
        }
        else if (newRoomInfo.right == currentRoom)
        {
            newRoomStart = new Vector3(-20, 0, 0);
            startingPosition = new Vector2(3, 0);
        }
        else if (newRoomInfo.up == currentRoom)
        {
            newRoomStart = new Vector3(0, -10, 0);
            startingPosition = new Vector2(0, Convert.ToSingle(1.5));
        }
        else
        {
            newRoomStart = new Vector3(0, 10, 0);
            startingPosition = new Vector2(0, Convert.ToSingle(-1.5));
        }

        GameObject nextRoomInstance = InstantiateSelector(newRoomInfo.variant % 2, 0, 1, firstRoom, secondRoom, newRoomStart);
        GameObject nextUpDoorInstance = InstantiateSelector(newRoomInfo.uplock, -1, 1, closedDoor, openedDoor, new Vector3(newRoomStart.x, Convert.ToSingle(newRoomStart.y + 3.5), 0));
        nextUpDoorInstance.transform.parent = nextRoomInstance.transform;
        GameObject nextDownDoorInstance = InstantiateSelector(newRoomInfo.downlock, -1, 1, closedDoor, openedDoor, new Vector3(newRoomStart.x, Convert.ToSingle(newRoomStart.y - 3.5), 0));
        nextDownDoorInstance.transform.parent = nextRoomInstance.transform;
        GameObject nextLeftDoorInstance = InstantiateSelector(newRoomInfo.leftlock, -1, 1, closedDoor, openedDoor, new Vector3(Convert.ToSingle(newRoomStart.x - 4.5), newRoomStart.y, 0));
        nextLeftDoorInstance.transform.parent = nextRoomInstance.transform;
        GameObject nextRightDoorInstance = InstantiateSelector(newRoomInfo.rightlock, -1, 1, closedDoor, openedDoor, new Vector3(Convert.ToSingle(newRoomStart.x + 4.5), newRoomStart.y, 0));
        nextRightDoorInstance.transform.parent = nextRoomInstance.transform;

        switch (dir)
        {
            case "left":
                StartCoroutine(moveManager.SmoothMovement(currentRoomInstance, new Vector3(20, 0, 0), swayingTime));
                StartCoroutine(moveManager.SmoothMovement(nextRoomInstance, new Vector3(0, 0, 0), swayingTime));
                yield return new WaitUntil(() => currentRoomInstance.transform.position == new Vector3(20, 0, 0));
                break;
            case "right":
                StartCoroutine(moveManager.SmoothMovement(currentRoomInstance, new Vector3(-20, 0, 0), swayingTime));
                StartCoroutine(moveManager.SmoothMovement(nextRoomInstance, new Vector3(0, 0, 0), swayingTime));
                yield return new WaitUntil(() => currentRoomInstance.transform.position == new Vector3(-20, 0, 0));
                break;
            case "up":
                StartCoroutine(moveManager.SmoothMovement(currentRoomInstance, new Vector3(0, -10, 0), swayingTime));
                StartCoroutine(moveManager.SmoothMovement(nextRoomInstance, new Vector3(0, 0, 0), swayingTime));
                yield return new WaitUntil(() => currentRoomInstance.transform.position == new Vector3(0, -10, 0));
                break;
            case "down":
                StartCoroutine(moveManager.SmoothMovement(currentRoomInstance, new Vector3(0, 10, 0), swayingTime));
                StartCoroutine(moveManager.SmoothMovement(nextRoomInstance, new Vector3(0, 0, 0), swayingTime));
                yield return new WaitUntil(() => currentRoomInstance.transform.position == new Vector3(0, 10, 0));
                break;
        }

        yield return new WaitUntil(() => nextRoomInstance.transform.position == new Vector3(0, 0, 0));

        currentRoom = newroom;

        DestroyObject(currentRoomInstance);

        currentRoomInstance = nextRoomInstance;
        upDoorInstance = nextUpDoorInstance;
        downDoorInstance = nextDownDoorInstance;
        leftDoorInstance = nextLeftDoorInstance;
        rightDoorInstance = nextRightDoorInstance;

        temp.text = Convert.ToString(newRoomInfo.variant);

        roomMoving = false;

        yield return null;
    }

    private void DestroyObject(GameObject obj)
    {
        if (obj != null)
            Destroy(obj);
    }

    //so I dont have to write 10 times same thing
    private GameObject InstantiateSelector(int givenValue, int firstOption, int secondOption, GameObject firstObject, GameObject secondObject, Vector3 position)
    {
        if (firstOption == givenValue)
            return Instantiate(firstObject, position, Quaternion.identity);
        else if (secondOption == givenValue)
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
                return map[currentRoom].uplock == -1 ? false : true;
            case "down":
                return map[currentRoom].downlock == -1 ? false : true;
            case "left":
                return map[currentRoom].leftlock == -1 ? false : true;
            case "right":
                return map[currentRoom].rightlock == -1 ? false : true;
        }

        return false;
    }

    public RoomInfo GetRoom()
    {
        return map[currentRoom];
    }

    public GameObject GetRoomInstance()
    {
        return currentRoomInstance;
    }

    public void ShakeThatDoor(string dir)
    {
        GameObject doorToShake = null;

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

        Animator doorAnim = doorToShake.GetComponent<Animator>();

        doorAnim.SetTrigger("EntryTry");
    }

    public bool IsRoomMoving()
    {
        return roomMoving;
    }

    public Vector2 GetStartingPosition()
    {
        return startingPosition;
    }

    public void SetMinimap(Minimap minimap)
    {
        this.minimap = minimap;
    }
}
