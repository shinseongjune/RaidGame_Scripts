using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipable_BurningSword : ItemBase
{
    public override void GetOn()
    {
        StatMod mod = new StatMod(Stat.Type.Might, StatMod.Type.BaseAdd, 60, "BurningSword_Might_60");

        owner.GetComponent<Stats>()[(int)mod.TargetStat].AppendMod(mod);

        Destroy(gameObject);
    }
}
