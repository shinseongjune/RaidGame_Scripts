using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Item
{
    public enum Rarity
    {
        COMMON,
        RARE,
        LEGENDARY,
    }

    [CreateAssetMenu(fileName = "NewItemMaterial", menuName = "ScriptableObjects/ItemMaterial")]
    public class ItemMaterial : Scriptable_ItemBase, IComparable<ItemMaterial>
    {
        public Rarity rarity;

        public int CompareTo(ItemMaterial other)
        {
            return id.CompareTo(other.id);
        }
    }
}
