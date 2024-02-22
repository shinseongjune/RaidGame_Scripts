using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Instant_MPPotion : ItemBase
{
    public float value;

    public override void GetOn()
    {
        owner.GetComponent<ControlComponent>().HealMana(value);

        Destroy(gameObject);
    }
}
