using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BattleManager: MonoBehaviour
{
    private Wizard player;
    private GameManager gameManager;
    private List<Enemy> enemies;
    private GameObject room;
    private MovementManager movementManager;
    Vector3Int mouseCell;

    private float player_speed = 15F;

    private bool playerTurn;
    private bool enemyMoving;
    private bool playerMoving;
    private bool moveUI = false;

    private void Start()
    {
        playerTurn = true;
        playerMoving = false;

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        movementManager = GameObject.Find("MovementManager").GetComponent<MovementManager>();
        player = GameObject.Find("LightBandit(Clone)").GetComponent<Wizard>();

        room = gameManager.GetRoom();
    }

    private void Update()
    {
        if (!playerTurn && !enemyMoving)
        {
            foreach (Enemy enemy in enemies)
                EnemyTurn(enemy);
            enemyMoving = true;
        }
        else if (playerTurn)
        {
            if ((Input.GetKey("m") || gameManager.GetMove()) && !moveUI)
                SetupMovementUI();

            if (moveUI)
            {
                var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var noZ = new Vector3(pos.x, pos.y);
                mouseCell = room.transform.GetComponent<Grid>().WorldToCell(noZ);
            }
        }

        if (Input.GetMouseButtonDown(0) && moveUI && !playerMoving)
        {
            StartCoroutine(MovePlayer());
        }
    }

    private IEnumerator MovePlayer()
    {
        if (mouseCell.x <= 7 && mouseCell.x >= -7 && mouseCell.y <= 4 && mouseCell.y >= -4)
        {
            int x;
            int y;

            playerMoving = true;

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

            switch (x >= player.transform.position.x)
            {
                case true:
                    player.SetRight();
                    break;
                case false:
                    player.SetLeft();
                    break;
            }
            player.SetMoving(true);

            Vector3 destination = room.transform.GetComponent<Grid>().CellToWorld(new Vector3Int(x, y, 0));

            StartCoroutine(movementManager.SmoothMovement(player.gameObject, destination, player_speed));
            yield return new WaitUntil(() => player.gameObject.transform.position.x == destination.x && player.gameObject.transform.position.y == destination.y);

            player.SetMoving(false);

            playerMoving = false;
        }

        yield return null;
    }

    private void EnemyTurn(Enemy enemy)
    {
        //TODO: AImejbi?
    }

    private void SetupMovementUI()
    {
        Tilemap lines = room.transform.Find("BattleLines").GetComponent<Tilemap>();
        foreach (var position in lines.cellBounds.allPositionsWithin)
        {
            //TODO: do smth to color change
            Debug.Log("mapping done");
        }

        moveUI = true;
    }
}
