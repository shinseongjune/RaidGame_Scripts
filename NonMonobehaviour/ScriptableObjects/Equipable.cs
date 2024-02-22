using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Item
{

    [CreateAssetMenu(fileName = "NewEquipable", menuName = "ScriptableObjects/Equipable")]
    public class Equipable : Scriptable_ItemBase, IComparable<Equipable>
    {
        public enum Type
        {
            HELMET,
            ARMOR,
            WEAPON
        }

        public Type type;
        public Rarity rarity;

        [SerializeField]
        public List<GameObject> effectsPrefabs = new();

        public int CompareTo(Equipable other)
        {
            int rarityComp = rarity.CompareTo(other.rarity);
            if (rarityComp != 0)
            {
                return rarityComp;
            }
            else
            {
                return itemName.CompareTo(other.itemName);
            }
        }

        //TODO: 외형 적용
        //public GameObject skillPrefab;
        //public List<GameObject> afterEffectPrefabs = new List<GameObject>();
    }
}
