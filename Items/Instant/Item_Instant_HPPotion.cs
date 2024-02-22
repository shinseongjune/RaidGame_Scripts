using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Instant_HPPotion : ItemBase
{
    public float value;

    public override void GetOn()
    {
        owner.GetComponent<ControlComponent>().Healed(value);

        Destroy(gameObject);
    }
}

