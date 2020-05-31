using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    private string path;
    private string jsonString;
    private RoomInfo[] map;
    private int currentRoom = 0;
    private bool roomMoving = false;
    private readonly float shakeTime = 10F;
    private readonly float swayingTime = 50F;
    private readonly float alpha_factor = 0.4F;      //battle lines transparency
    private readonly float battle_size = 1.6F;
    private readonly float increase_speed = 0.002F;
    private readonly float fading_speed = 0.01F;
    private readonly float player_to_battle_duration = 50F;
    private readonly float room_scale_x = 0.5F;
    private readonly float room_scale_y = 0.5F;

    private List<Enemy> enemyList = new List<Enemy>(); //lists all opponents in the room
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

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

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

        HideBattleLines(currentRoomInstance);

        minimap = Resources.Load<Minimap>("Objects/Minimap");
        minimap = Instantiate(minimap, new Vector3(-8, 2, 0), Quaternion.identity);
    }

    //private void Update()
    //{
    //    Debug.Log("x: " + currentRoomInstance.transform.localScale.x);
    //    Debug.Log("y: " + currentRoomInstance.transform.localScale.y);
    //    Debug.Log("sqr: " + (new Vector3(0, 0, -1) - moveManager.GetPlayerPosition()).sqrMagnitude);
    //}

    public IEnumerator UpdateRoom(int newroom, string dir)
    {
        roomMoving = true;

        RoomInfo newRoomInfo = map[newroom];
        Vector3 newRoomStart;

        minimap.MoveCurrent(dir, newRoomInfo);

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

        HideBattleLines(nextRoomInstance);

        switch (dir)
        {
            case "left":
                StartCoroutine(MovementManager.Instance.SmoothMovement(nextRoomInstance, new Vector3(0, 0, 0), swayingTime));
                yield return StartCoroutine(MovementManager.Instance.SmoothMovement(currentRoomInstance, new Vector3(20, 0, 0), swayingTime));
                break;
            case "right":
                StartCoroutine(MovementManager.Instance.SmoothMovement(nextRoomInstance, new Vector3(0, 0, 0), swayingTime));
                yield return StartCoroutine(MovementManager.Instance.SmoothMovement(currentRoomInstance, new Vector3(-20, 0, 0), swayingTime));
                break;
            case "up":
                StartCoroutine(MovementManager.Instance.SmoothMovement(nextRoomInstance, new Vector3(0, 0, 0), swayingTime));
                yield return StartCoroutine(MovementManager.Instance.SmoothMovement(currentRoomInstance, new Vector3(0, -10, 0), swayingTime));
                break;
            case "down":
                StartCoroutine(MovementManager.Instance.SmoothMovement(nextRoomInstance, new Vector3(0, 0, 0), swayingTime));
                yield return StartCoroutine(MovementManager.Instance.SmoothMovement(currentRoomInstance, new Vector3(0, 10, 0), swayingTime));
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

        //add monsters
        if (newRoomInfo.enemy_num != 0)
            StartCoroutine(ChangeToBattle());

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

    public void ChangeCluster(int toChange, int prefab)
    {
        foreach (RoomInfo element in map)
        {
            if (element.visited == toChange)
                element.visited = prefab;
        }
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

    private void AddMonsters()
    {
        int divider = (int) Math.Pow(10, map[currentRoom].enemy_num - 1);
        enemyList.Clear();

        int x_list = map[currentRoom].enemy_x;
        int y_list = map[currentRoom].enemy_y;
        string name_list = map[currentRoom].enemy_name + "_";
        string hp_list = map[currentRoom].enemy_hp + "_";

        for (int i = 0; i < map[currentRoom].enemy_num; i++)
        {
            int x_pos = ((x_list / divider) - 3) * 3;
            int y_pos = ((y_list / divider) - 2) * 3;

            string name = name_list.Substring(1, name_list.IndexOf("_", 1) - 1);
            int hp = Convert.ToInt32(hp_list.Substring(1, hp_list.IndexOf("_", 1) - 1));

            x_list -= (x_list / divider) * divider;
            y_list -= (y_list / divider) * divider;
            name_list = name_list.Substring(name_list.IndexOf("_", 1));
            hp_list = hp_list.Substring(hp_list.IndexOf("_", 1));

            //Enemy monster = Instantiate(GameManager.Instance.GetMonster(name), currentRoomInstance.transform.GetComponent<Grid>().CellToWorld(new Vector3Int(x_pos, y_pos, 0)), Quaternion.identity);
            float y_world = y_pos / 3;
            y_world = y_world * 2.5f;

            float x_world = x_pos / 3;
            x_world = x_world * 2.4f + 0.5f;

            Enemy monster = Instantiate(GameManager.Instance.GetMonster(name), new Vector3(x_world, y_world, 0), Quaternion.identity);
            enemyList.Add(monster);

            if (monster.GetHealth() > hp)
                monster.DealDamage(monster.GetHealth() - hp);

            divider = divider / 10;
        }
    }

    public List<Enemy> GetEnemies()
    {
        return enemyList;
    }

    public void DisposeOfEnemy(Enemy enemy)
    {
        enemyList.Remove(enemy);
    }

    ///////////////////////////////////BATTLE METHODS
    private IEnumerator ChangeToBattle()
    {
        leftDoorInstance.SetActive(false);
        rightDoorInstance.SetActive(false);
        upDoorInstance.SetActive(false);
        downDoorInstance.SetActive(false);
        minimap.Hide();

        Vector3 desiredScale = battle_size * currentRoomInstance.transform.localScale;
        Vector3 playerGoal = MovementManager.Instance.GetPlayerPosition() * 1.7F;
        StartCoroutine(MovementManager.Instance.RashMovement(GameManager.Instance.GetPlayer().gameObject, playerGoal, player_to_battle_duration));
        yield return StartCoroutine(IncreaseSize(currentRoomInstance, battle_size, increase_speed));
        //yield return new WaitUntil(() => (desiredScale.x <= currentRoomInstance.transform.localScale.x && desiredScale.y <= currentRoomInstance.transform.localScale.y));
        //                                  //  && (playerGoal - moveManager.GetPlayerPosition()).sqrMagnitude <= float.Epsilon));

        //StopAllCoroutines();
        GameManager.Instance.ChangeToBattle();
        AddMonsters();

        StartCoroutine(FadeInBattleLines(currentRoomInstance, fading_speed));
        yield return null;
    }

    public IEnumerator ChangeToNormal()
    {
        yield return StartCoroutine(FadeOutBattleLines(currentRoomInstance, fading_speed));

        Vector3 playerGoal = new Vector3(0, 0, -1);
        StartCoroutine(MovementManager.Instance.RashMovement(GameManager.Instance.GetPlayer().gameObject, playerGoal, player_to_battle_duration));
        yield return StartCoroutine(DecreaseSize(currentRoomInstance, battle_size, increase_speed));

        //StopAllCoroutines();
        GameManager.Instance.ChangeToNormal();

        leftDoorInstance.SetActive(true);
        rightDoorInstance.SetActive(true);
        upDoorInstance.SetActive(true);
        downDoorInstance.SetActive(true);
        minimap.Show();

        yield return null;
    }

    private IEnumerator IncreaseSize(GameObject room, float factor, float speed)
    {
        Vector3 desiredScale = factor*room.transform.localScale;
        float scale = room.transform.localScale.x;

        while (desiredScale.x > room.transform.localScale.x || desiredScale.y > room.transform.localScale.y)
        {
            room.transform.localScale = Vector3.one * scale;
            scale += speed;
            yield return null;
        }

        yield return null;
    }

    private IEnumerator DecreaseSize(GameObject room, float factor, float speed)
    {
        float scale = room.transform.localScale.x;

        while (room_scale_x < room.transform.localScale.x || room_scale_y < room.transform.localScale.y)
        {
            room.transform.localScale = Vector3.one * scale;
            scale -= speed;
            yield return null;
        }

        yield return null;
    }

    private IEnumerator FadeInBattleLines(GameObject room, float speed)
    {
        Tilemap lines = room.transform.Find("BattleLines").GetComponent<Tilemap>();
        float alpha = 0.0F;

        while(alpha < alpha_factor)
        {
            foreach (var position in lines.cellBounds.allPositionsWithin)
            {
                lines.SetTileFlags(position, TileFlags.None);
                lines.SetColor(position, new Color(1, 1, 1, lines.GetColor(position).a + speed));
            }

            alpha += speed;
            yield return null;
        }

        yield return null;
    }

    private IEnumerator FadeOutBattleLines(GameObject room, float speed)
    {
        Tilemap lines = room.transform.Find("BattleLines").GetComponent<Tilemap>();
        float alpha = alpha_factor;

        while (alpha > 0.0f)
        {
            foreach (var position in lines.cellBounds.allPositionsWithin)
            {
                lines.SetTileFlags(position, TileFlags.None);
                lines.SetColor(position, new Color(1, 1, 1, lines.GetColor(position).a - speed));
            }

            alpha -= speed;
            yield return null;
        }

        yield return null;
    }

    private void HideBattleLines(GameObject room)
    {
        //hide battle lines
        Tilemap lines = room.transform.Find("BattleLines").GetComponent<Tilemap>();
        foreach (var position in lines.cellBounds.allPositionsWithin)
        {
            lines.SetTileFlags(position, TileFlags.None);
            lines.SetColor(position, new Color(1, 1, 1, 0));
        }
    }
}
