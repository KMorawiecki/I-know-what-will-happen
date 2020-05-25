using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Menu_Icon : MonoBehaviour
{
    private GameObject eqTabInstance;

    public void Enlarge()
    {
        RectTransform this_transform = gameObject.GetComponent<RectTransform>();
        this_transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Convert.ToSingle(1.6)*this_transform.sizeDelta.y);
        this_transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Convert.ToSingle(1.6) * this_transform.sizeDelta.x);
    }

    public void Belittle()
    {
        RectTransform this_transform = gameObject.GetComponent<RectTransform>();
        this_transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Convert.ToSingle(0.625) * this_transform.sizeDelta.y);
        this_transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Convert.ToSingle(0.625) * this_transform.sizeDelta.x);
    }

    public void ShowEQ()
    {
        GameManager.Instance.DisableRoom();
        GameObject eqTabPrefab = Resources.Load<GameObject>("Objects/EquipmentTab");
        eqTabInstance = Instantiate(eqTabPrefab, new Vector3(0,0,0), Quaternion.identity);

        eqTabInstance.transform.SetParent(GameObject.Find("Canvas").transform);
        eqTabInstance.name = "CurrentTab";
    }
}
