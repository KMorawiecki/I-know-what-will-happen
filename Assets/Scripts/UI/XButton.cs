using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XButton : MonoBehaviour
{
    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        GameObject go = GameObject.Find("GameManager");
        gameManager = go.GetComponent<GameManager>();
    }

    public void CloseTab()
    {
        gameManager.CloseCurrentTab();
    }
}
