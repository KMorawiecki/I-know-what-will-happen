using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spell : MonoBehaviour
{
    protected int range;
    protected int radius;
    protected int direction;
    protected int hitpoints;
    protected int tier;
    protected string spell_name;
    protected bool ko;
    protected bool targeted;
    protected bool chosenTarget = false;
    protected int mana_cost;

    private Vector3Int mouseCell;
    private Grid roomGrid;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        roomGrid = MapManager.Instance.transform.GetComponent<Grid>();
    }

    protected virtual void Update()
    {
        if (!targeted)
        {
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var noZ = new Vector3(pos.x, pos.y);
            mouseCell = roomGrid.WorldToCell(noZ);

            int x;
            int y;

            if (Input.GetMouseButtonDown(0) && !chosenTarget)
            {
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

                chosenTarget = true;
                SpellBehaviour(roomGrid.CellToWorld(new Vector3Int(x, y, 0)));
            }
        }
    }

    public abstract void EnemyBehaviour(Enemy target);
    public abstract void SpellBehaviour(Vector3 target);
    public abstract void SpellBehaviour(Enemy target);
}
