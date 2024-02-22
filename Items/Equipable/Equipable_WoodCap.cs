using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipable_WoodCap : ItemBase
{
    public override void GetOn()
    {
        StatMod mod = new StatMod(Stat.Type.Armor, StatMod.Type.BaseAdd, 5, "WoodCap_Armor_5");

        owner.GetComponent<Stats>()[(int)mod.TargetStat].AppendMod(mod);

        Destroy(gameObject);
    }
}
