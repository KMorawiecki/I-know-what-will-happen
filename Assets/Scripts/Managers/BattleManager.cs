using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class BattleManager: MonoBehaviour
{
    private Wizard player;
    private GameManager gameManager;
    private MovementManager movementManager;
    private UIManager uiManager;
    private MapManager mapManager;
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

    private void Start()
    {
        playerTurn = true;
        playerMoving = false;

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        movementManager = GameObject.Find("MovementManager").GetComponent<MovementManager>();
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        player = GameObject.Find("LightBandit(Clone)").GetComponent<Wizard>();
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        move_butt = GameObject.Find("MoveButton(Clone)").GetComponent<Button>();
        fight_butt = GameObject.Find("FightButton(Clone)").GetComponent<Button>();

        room = gameManager.GetRoom();
        enemies = mapManager.GetEnemies();

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
            foreach (Enemy enemy in enemies)
                EnemyTurn(enemy);
            enemyMoving = true;
        }
        else if (playerTurn)
        {
            //move player
            if (Input.GetMouseButtonDown(0) && moveUI && !playerMoving)
                if (Math.Abs(ConvertToTiles().x - player_x) <= player.GetMovementRange()*3 && Math.Abs(ConvertToTiles().y - player_y) <= player.GetMovementRange()*3)
                    StartCoroutine(MovePlayer(ConvertToTiles()));

            if ((Input.GetKey("m") || gameManager.GetMove()) && !moveUI && !moveBlock)
            {
                moveUI = true;
                gameManager.SetFight(false);
                SetupMovementUI();
            }

            if ((Input.GetKey("f") || gameManager.GetFight()) && !fightUI)
            {
                fightUI = true;
                gameManager.SetMove(false);
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

            Vector3 destination = room.transform.GetComponent<Grid>().CellToWorld(new Vector3Int(Convert.ToInt32(click_pos.x), Convert.ToInt32(click_pos.y), 0));

            StartCoroutine(movementManager.SmoothMovement(player.gameObject, destination, player_speed));
            yield return new WaitUntil(() => player.gameObject.transform.position.x == destination.x && player.gameObject.transform.position.y == destination.y);

            player_x = (int) click_pos.x;
            player_y = (int) click_pos.y;

            player.SetMoving(false);

            playerMoving = false;

            UnsetupMovementUI();
            gameManager.SetMove(false);
            moveUI = false;
            move_butt.interactable = false;
            moveBlock = true;
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

    private void EnemyTurn(Enemy enemy)
    {
        //TODO: AImejbi?
    }

    private void SetupMovementUI()
    {
        //redo fighting icons
        if(fightUI)
            uiManager.RestoreMenus();

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
        uiManager.ChangeMenus();

        moveUI = false;
    }

    private void UnSetupFightingUI()
    {
        //TODO: redo color change
        gameManager.SetFight(false);
        uiManager.RestoreMenus();

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
        foreach (Enemy enemy in mapManager.GetEnemies())
        {
            enemy.ResetTargeting();
        }
    }

    public IEnumerator EndBattle()
    {
        yield return StartCoroutine(mapManager.ChangeToNormal());
        Destroy(gameObject);
    }
}
