using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private GameManager gameManager;

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

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        canvas = GameObject.Find("Canvas");

        GameObject eqPrefab = Resources.Load<GameObject>("Objects/EquipementButton");
        GameObject eqGO = Instantiate(eqPrefab, canvas.transform);
        eqButton = eqGO.GetComponent<Menu_Icon>();

        GameObject spPrefab = Resources.Load<GameObject>("Objects/SpellsButton");
        GameObject spGO = Instantiate(spPrefab, canvas.transform);
        spellButton = spGO.GetComponent<Menu_Icon>();

        GameObject colPrefab = Resources.Load<GameObject>("Objects/CollectablesButton");
        GameObject colGO = Instantiate(colPrefab, canvas.transform);
        colButton = colGO.GetComponent<Menu_Icon>();

        GameObject stPrefab = Resources.Load<GameObject>("Objects/StatsButton");
        GameObject stGO = Instantiate(stPrefab, canvas.transform);
        statButton = stGO.GetComponent<Menu_Icon>();

        GameObject healthPrefab = Resources.Load<GameObject>("Objects/HealthHolder");
        healthHolder = Instantiate(healthPrefab, canvas.transform);

        GameObject manaPrefab = Resources.Load<GameObject>("Objects/ManaHolder");
        manaHolder = Instantiate(manaPrefab, canvas.transform);

        GameObject integrityPrefab = Resources.Load<GameObject>("Objects/IntegrityHolder");
        intHolder = Instantiate(integrityPrefab, canvas.transform);

        GameObject fightPrefab = Resources.Load<GameObject>("Objects/FightButton");
        fightButton = Instantiate(fightPrefab, canvas.transform);
        fightButton.SetActive(false);

        GameObject movePrefab = Resources.Load<GameObject>("Objects/MoveButton");
        moveButton = Instantiate(movePrefab, canvas.transform);
        moveButton.GetComponent<Button>().onClick.AddListener(() => gameManager.SetMove());
        moveButton.SetActive(false);
    }

    public void ChangeToBattle()
    {
        ShowBattleButtons();
    }

    private void ShowBattleButtons()
    {
        fightButton.SetActive(true);
        moveButton.SetActive(true);
    }
}
