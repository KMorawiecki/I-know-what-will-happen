using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class BattleManager: MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    private Wizard player;
    private List<Enemy> enemies;
    private GameObject room;
    private Vector3Int mouseCell;
    private GameObject sword;
    private GameObject staff;
    private Button move_butt;
    private Button fight_butt;

    private GameObject activeSpell;

    private float player_speed = 15F;

    private bool playerTurn;
    private bool enemyMoving;
    private bool playerMoving;

    private bool moveUI = false;    //is ui already set?
    private bool fightUI = false;

    private bool moveBlock = false;    //can we move this turn?
    private bool fightBlock = false;

    //player position in tile logic
    private int player_x;
    private int player_y;

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

    private void Start()
    {
        playerTurn = false;
        playerMoving = false;

        player = GameObject.Find("LightBandit(Clone)").GetComponent<Wizard>();
        move_butt = GameObject.Find("MoveButton(Clone)").GetComponent<Button>();
        fight_butt = GameObject.Find("FightButton(Clone)").GetComponent<Button>();

        room = MapManager.Instance.GetRoomInstance();
        enemies = MapManager.Instance.GetEnemies();

        if(player.transform.position.x > 0)
        {
            player_x = 6;
            player_y = 0;
        }
        else if (player.transform.position.x < 0)
        {
            player_x = -6;
            player_y = 0;
        }
        else if (player.transform.position.y < 0)
        {
            player_x = 0;
            player_y = -3;
        }
        else if (player.transform.position.y < 0)
        {
            player_x = 0;
            player_y = 3;
        }
    }

    private void Update()
    {
        //suspend spell
            if (activeSpell != null)
            if (Input.GetKey("escape"))
                EndSpell();

        //check if enemies are gone
        if (enemies.Count == 0)
            StartCoroutine(EndBattle());

        //get tile pos
        if (moveUI)
        {
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var noZ = new Vector3(pos.x, pos.y);
            mouseCell = room.transform.GetComponent<Grid>().WorldToCell(noZ);
        }

        if (!playerTurn && !enemyMoving)
        {
            enemyMoving = true;
            StartCoroutine(MoveEnemies());
        }
        else if (playerTurn)
        {
            //move player
            if (Input.GetMouseButtonDown(0) && moveUI && !playerMoving)
                if (Math.Abs(ConvertToTiles().x - player_x) <= player.GetMovementRange() * 3 && Math.Abs(ConvertToTiles().y - player_y) <= player.GetMovementRange() * 3)
                    StartCoroutine(MovePlayer(ConvertToTiles()));

            if ((Input.GetKey("m") || GameManager.Instance.GetMove()) && !moveUI && !moveBlock)
            {
                moveUI = true;
                GameManager.Instance.SetFight(false);
                SetupMovementUI();
            }

            if ((Input.GetKey("f") || GameManager.Instance.GetFight()) && !fightUI)
            {
                fightUI = true;
                GameManager.Instance.SetMove(false);
                SetupFightingUI();
            }

            if (fightUI)
            {
                //hit some zombies
                if (Input.GetMouseButtonDown(0))
                {
                    //Get the mouse position on the screen and send a raycast into the game world from that position.
                    Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

                    //If something was hit, the RaycastHit2D.collider will not be null.
                    if (hit.collider != null)
                    {
                        Spell_Button sb = hit.collider.gameObject.GetComponent<Spell_Button>();
                        if (sb != null)
                            sb.InvokeSpell();
                    }
                }
            }
        }
    }

    private IEnumerator MovePlayer(Vector2 click_pos)
    {
        if (click_pos.x <= 7 && click_pos.x >= -7 && click_pos.y <= 4 && click_pos.y >= -4)
        {
            Debug.Log("x: " + Convert.ToString(click_pos.x));
            Debug.Log("y: " + Convert.ToString(click_pos.y));

            playerMoving = true;

            switch (click_pos.x >= player.transform.position.x)
            {
                case true:
                    player.SetRight();
                    break;
                case false:
                    player.SetLeft();
                    break;
            }
            player.SetMoving(true);

            //Vector3 destination = room.transform.GetComponent<Grid>().CellToWorld(new Vector3Int(Convert.ToInt32(click_pos.x), Convert.ToInt32(click_pos.y), 0));
            float y_world = click_pos.y / 3;
            y_world = y_world * 2.5f;

            float x_world = click_pos.x / 3;
            x_world = x_world * 2.4f + 0.5f;

            Vector3 destination = new Vector3(x_world, y_world, 0);

            yield return StartCoroutine(MovementManager.Instance.SmoothMovement(player.gameObject, destination, player_speed));

            player_x = (int) click_pos.x;
            player_y = (int) click_pos.y;

            player.SetMoving(false);

            playerMoving = false;
            playerTurn = false;

            UnsetupMovementUI();

            //TODO: will enable when turns are implemented
            //GameManager.Instance.SetMove(false);
            //moveUI = false;
            //move_butt.interactable = false;
            //moveBlock = true;
        }

        yield return null;
    }

    private Vector2 ConvertToTiles()
    {
        int x;
        int y;

        if (Math.Abs(mouseCell.x) % 3 == 0)
            x = mouseCell.x;
        else if (Math.Abs(mouseCell.x) % 3 == 1)
            x = (mouseCell.x < 0 ? mouseCell.x + 1 : mouseCell.x - 1);
        else
            x = (mouseCell.x < 0 ? mouseCell.x - 1 : mouseCell.x + 1);

        if (Math.Abs(mouseCell.y) % 3 == 0)
            y = mouseCell.y;
        else if (Math.Abs(mouseCell.y) % 3 == 1)
            y = (mouseCell.y < 0 ? mouseCell.y + 1 : mouseCell.y - 1);
        else
            y = (mouseCell.y < 0 ? mouseCell.y - 1 : mouseCell.y + 1);

        return new Vector2(x, y);
    }

    private IEnumerator MoveEnemies()
    {
        foreach (Enemy enemy in enemies)
            yield return StartCoroutine(EnemyTurn(enemy));

        playerTurn = true;
        enemyMoving = false;
    }

    private IEnumerator EnemyTurn(Enemy enemy)
    {
        yield return StartCoroutine(enemy.Turn());
    }

    private void SetupMovementUI()
    {
        //redo fighting icons
        if(fightUI)
            UIManager.Instance.RestoreMenus();

        Tilemap lines = room.transform.Find("BattleLines").GetComponent<Tilemap>();
        foreach (var position in lines.cellBounds.allPositionsWithin)
        {
            //TODO: do smth to color change
            Debug.Log("mapping done");
        }

        fightUI = false;
    }

    private void UnsetupMovementUI()
    {
        //TODO: do
    }

    private void SetupFightingUI()
    {
        //TODO: redo color change
        UIManager.Instance.ChangeMenus();

        moveUI = false;
    }

    private void UnSetupFightingUI()
    {
        //TODO: redo color change
        GameManager.Instance.SetFight(false);
        UIManager.Instance.RestoreMenus();

        fightUI = false;
    }

    public void SetFightIcons(GameObject sword, GameObject staff)
    {
        this.sword = sword;
        this.staff = staff;
    }

    public void SetSpell(GameObject spell)
    {
        activeSpell = spell;
    }

    public void BlockSwordAndStaff()
    {
        staff.GetComponent<FightAtributeHolder>().BlockUI();
        sword.GetComponent<FightAtributeHolder>().BlockUI();
    }

    public void EndSpell()
    {
        Destroy(activeSpell);
        staff.GetComponent<FightAtributeHolder>().UnblockUI();
        sword.GetComponent<FightAtributeHolder>().UnblockUI();
        UnSetupFightingUI();
        foreach (Enemy enemy in MapManager.Instance.GetEnemies())
        {
            enemy.ResetTargeting();
        }

        playerTurn = false;
    }

    public IEnumerator EndBattle()
    {
        yield return StartCoroutine(MapManager.Instance.ChangeToNormal());
        MovementManager.Instance.ResetMovementCooldown();
        Destroy(gameObject);
    }

    public float ConvBoardToReal(int board)
    {
        return Convert.ToSingle(board * 2.5);
    }
}
