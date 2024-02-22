using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipable_IronSword : ItemBase
{
    public override void GetOn()
    {
        StatMod mod = new StatMod(Stat.Type.Might, StatMod.Type.BaseAdd, 20, "IronSword_Might_20");

        owner.GetComponent<Stats>()[(int)mod.TargetStat].AppendMod(mod);

        Destroy(gameObject);
    }
}
