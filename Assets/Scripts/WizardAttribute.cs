using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class WizardAttribute : MonoBehaviour
{
    private int value = 100;
    private int scale_mod_x = 20;
    private int scale_mod_y = 2;
    private GameObject bar;
    private GameObject icon;
    private Text number;
    private string font_path = "Fonts/pixelfont";
    private string icon_path;

    // Start is called before the first frame update
    void Start()
    {
        bar = gameObject;

        icon = GameObject.Find(name + "Icon");
        number = GameObject.Find(name + "Value").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    ChangeValue(true, 20);
        //    Debug.Log("down");
        //}
    }

    public void ChangeValue(bool add, int element)
    {
        if (add)
            value += element;
        else
            value -= element;

        bar.transform.localScale = new Vector3(scale_mod_x * value / 100, scale_mod_y, 1);
        number.text = Convert.ToString(value);
    }
}
