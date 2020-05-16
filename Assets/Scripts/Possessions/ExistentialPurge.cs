using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExistentialPurge : Spell
{
    private Enemy target_enemy;
    private GameObject halo;

    // Start is called before the first frame update
    protected override void Start()
    {
        spell_name = "Existential Purge";
        range = 4;
        radius = 2;
        direction = -1;
        hitpoints = 0;
        ko = true;
        targeted = true;
        tier = 10;

        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (target_enemy != null)
            if (target_enemy.IsAnimatorInState("Hurt"))
                Invoke("EndSpell", 0.5f);
    }

    public override void EnemyBehaviour(Enemy target)
    {
        target.SetHurtTrigger();
        target.GetComponent<SpriteRenderer>().material = Resources.Load("Materials/White") as Material;
        target_enemy = target;
    }

    public override void SpellBehaviour(Vector3 target)
    {
        //not needed in this spell
        throw new System.NotImplementedException();
    }

    public override void SpellBehaviour(Enemy target)
    {
        GameObject purgePref = Resources.Load("Objects/PurgeLight") as GameObject;
        halo = Instantiate(purgePref);
        halo.transform.position = target.transform.position;

        StartCoroutine(Dawn(halo.GetComponent<Light>()));
    }

    private void EndSpell()
    {
        Destroy(target_enemy.gameObject);
        Destroy(halo);

        GameObject.Find("MapManager").GetComponent<MapManager>().DisposeOfEnemy(target_enemy);
        battleManager.EndSpell();
    }

    private IEnumerator Dawn(Light light)
    {
        while (light.range < 8)
        {
            light.range += 0.05f;
            light.intensity += 0.005f;

            yield return null;
        }

        yield return null;
    }
}
