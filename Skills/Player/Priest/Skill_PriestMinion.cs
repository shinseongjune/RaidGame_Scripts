using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_PriestMinion : SkillBase
{
    public GameObject prefab_summonee;

    public override void GetOn()
    {
        Instantiate(prefab_summonee, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
