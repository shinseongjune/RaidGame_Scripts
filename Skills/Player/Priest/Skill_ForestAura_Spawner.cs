using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_ForestAura_Spawner : SkillBase
{
    public GameObject prefab_ForestAura;
    float range = 8f;

    public override void GetOn()
    {
        int mask = LayerMask.GetMask("Player");

        Collider[] allies = Physics.OverlapSphere(transform.position, range, mask);

        foreach(Collider c in allies)
        {
            ControlComponent control = c.transform.root.GetComponent<ControlComponent>();

            if (control != null && !control.isDead)
            {
                Skill_ForestAura aura = Instantiate(prefab_ForestAura, control.transform.position, control.transform.rotation).GetComponent<Skill_ForestAura>();
                aura.followee = control.gameObject;
                aura.GetOn();
            }
        }

        Destroy(gameObject);
    }
}
