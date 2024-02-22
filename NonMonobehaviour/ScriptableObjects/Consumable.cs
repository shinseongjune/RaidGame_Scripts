using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Item
{
    [CreateAssetMenu(fileName = "NewConsumable", menuName = "ScriptableObjects/Consumable")]
    public class Consumable : Scriptable_ItemBase, IComparable<Consumable>
    {
        public enum Type
        {
            THROW,
            INSTANT
        }

        public Type type;

        [Min(1)]
        public int maxCount;

        //TODO: skill slots에서 range 구현.
        public float range;

        public float coolDown;

        public GameObject itemPrefab;
        public List<GameObject> afterEffectPrefabs = new List<GameObject>();

        public int CompareTo(Consumable other)
        {
            return id.CompareTo(other.id);
        }
    }
}