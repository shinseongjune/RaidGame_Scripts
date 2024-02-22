using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipable_WoodArmor : ItemBase
{
    public override void GetOn()
    {
        StatMod mod = new StatMod(Stat.Type.Armor, StatMod.Type.BaseAdd, 10, "WoodArmor_Armor_10");

        owner.GetComponent<Stats>()[(int)mod.TargetStat].AppendMod(mod);

        Destroy(gameObject);
    }
}
