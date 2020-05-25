using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FightAtributeHolder : MonoBehaviour
{
    GameObject skillHolder;
    SpriteRenderer spriteRenderer;

    protected bool block = false;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        skillHolder = gameObject.transform.Find("AbilityHolder").gameObject;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnMouseOver()
    {
        Activate();
    }

    public void OnMouseExit()
    {
        Deactivate();
    }

    protected void Activate()
    {
        if (!block)
        {
            spriteRenderer.color = new Color(0.3f, 0.3f, 0.3f);

            foreach (Transform child in transform)
                child.gameObject.SetActive(true);
        }
    }

    public void Deactivate()
    {
        spriteRenderer.color = new Color(1f, 1f, 1f);

        foreach (Transform child in transform)
            child.gameObject.SetActive(false);
    }

    public void BlockUI()
    {
        block = true;
    }

    public void UnblockUI()
    {
        block = false;
    }

    protected void AddSpells()
    {
        List<string> spells = GameManager.Instance.GetPlayer().GetSpells();

        foreach (string spell in spells)
        {
            Spell_Button sp_obj = Instantiate(Resources.Load<Spell_Button>("Objects/" + spell + "Icon"));
            sp_obj.transform.SetParent(transform);
            //TODO: make it in order
            sp_obj.transform.localPosition = new Vector3(0.02f, 0.375f, 0);
        }
    }
}
