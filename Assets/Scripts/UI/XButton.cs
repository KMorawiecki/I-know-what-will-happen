using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XButton : MonoBehaviour
{
    public void CloseTab()
    {
        GameManager.Instance.CloseCurrentTab();
    }
}
