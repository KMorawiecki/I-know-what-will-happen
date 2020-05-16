using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private GameManager gameManager;
    private MovementManager movementManager;
    private BattleManager battleManager;

    private GameObject canvas;
    private Menu_Icon eqButton;
    private Menu_Icon colButton;
    private Menu_Icon statButton;
    private Menu_Icon spellButton;
    private GameObject healthHolder;
    private GameObject intHolder;
    private GameObject manaHolder;
    private GameObject fightButton;
    private GameObject moveButton;
    private GameObject swordIcon;
    private GameObject staffIcon;

    private GameObject rightMenuHolder;
    private GameObject spellHolder;

    private readonly float icon_speed = 15f;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        movementManager = GameObject.Find("MovementManager").GetComponent<MovementManager>();
        canvas = GameObject.Find("Canvas");

        rightMenuHolder = new GameObject("RightMenuHolder");
        rightMenuHolder.transform.parent = canvas.transform;
        rightMenuHolder.transform.localPosition = new Vector3(0, 0, 0);
        rightMenuHolder.transform.localScale = new Vector3(1, 1, 1);
        rightMenuHolder.AddComponent<Rigidbody2D>().gravityScale = 0;

        GameObject eqPrefab = Resources.Load<GameObject>("Objects/EquipementButton");
        GameObject eqGO = Instantiate(eqPrefab, rightMenuHolder.transform);
        eqButton = eqGO.GetComponent<Menu_Icon>();

        GameObject spPrefab = Resources.Load<GameObject>("Objects/SpellsButton");
        GameObject spGO = Instantiate(spPrefab, rightMenuHolder.transform);
        spellButton = spGO.GetComponent<Menu_Icon>();

        GameObject colPrefab = Resources.Load<GameObject>("Objects/CollectablesButton");
        GameObject colGO = Instantiate(colPrefab, rightMenuHolder.transform);
        colButton = colGO.GetComponent<Menu_Icon>();

        GameObject stPrefab = Resources.Load<GameObject>("Objects/StatsButton");
        GameObject stGO = Instantiate(stPrefab, rightMenuHolder.transform);
        statButton = stGO.GetComponent<Menu_Icon>();

        GameObject healthPrefab = Resources.Load<GameObject>("Objects/HealthHolder");
        healthHolder = Instantiate(healthPrefab, canvas.transform);

        GameObject manaPrefab = Resources.Load<GameObject>("Objects/ManaHolder");
        manaHolder = Instantiate(manaPrefab, canvas.transform);

        GameObject integrityPrefab = Resources.Load<GameObject>("Objects/IntegrityHolder");
        intHolder = Instantiate(integrityPrefab, canvas.transform);

        GameObject fightPrefab = Resources.Load<GameObject>("Objects/FightButton");
        fightButton = Instantiate(fightPrefab, canvas.transform);
        fightButton.GetComponent<Button>().onClick.AddListener(() => gameManager.SetFight(true));
        fightButton.SetActive(false);

        GameObject movePrefab = Resources.Load<GameObject>("Objects/MoveButton");
        moveButton = Instantiate(movePrefab, canvas.transform);
        moveButton.GetComponent<Button>().onClick.AddListener(() => gameManager.SetMove(true));
        moveButton.SetActive(false);
    }

    public void ChangeToBattle(BattleManager manager)
    {
        ShowBattleButtons();

        battleManager = manager;

        GameObject swordPref = Resources.Load<GameObject>("Objects/Sword");
        GameObject staffPref = Resources.Load<GameObject>("Objects/MageStaff");

        swordIcon = Instantiate(swordPref, new Vector3(10, 0, 0), Quaternion.identity);
        staffIcon = Instantiate(staffPref, new Vector3(-10, 0, 0), Quaternion.identity);

        manager.SetFightIcons(swordIcon, staffIcon);
    }

    public void ChangeToNormal()
    {
        HideBattleButtons();

        battleManager = null;

        Destroy(swordIcon);
        Destroy(staffIcon);
    }

    private void ShowBattleButtons()
    {
        fightButton.SetActive(true);
        moveButton.SetActive(true);
    }

    private void HideBattleButtons()
    {
        fightButton.SetActive(false);
        moveButton.SetActive(false);
    }

    //to change between fight and normal menu
    public void ChangeMenus()
    {
        //StopAllCoroutines();
        StartCoroutine(movementManager.SmoothMovement(rightMenuHolder, new Vector3(2.5f, rightMenuHolder.transform.position.y), icon_speed));
        StartCoroutine(movementManager.SmoothMovement(swordIcon, new Vector3(7.5f, 0), icon_speed));
        StartCoroutine(movementManager.SmoothMovement(staffIcon, new Vector3(-7.5f, 0), icon_speed));
    }

    //change menus back
    public void RestoreMenus()
    {
        //StopAllCoroutines();
        StartCoroutine(movementManager.SmoothMovement(rightMenuHolder, new Vector3(0, rightMenuHolder.transform.position.y), icon_speed));
        StartCoroutine(movementManager.SmoothMovement(swordIcon, new Vector3(10f, 0), icon_speed));
        StartCoroutine(movementManager.SmoothMovement(staffIcon, new Vector3(-10f, 0), icon_speed));
    }
}
