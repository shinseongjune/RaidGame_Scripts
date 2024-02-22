using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Item
{
    [CreateAssetMenu(fileName = "NewRecipe", menuName = "ScriptableObjects/MaterialRecipe")]
    public class MaterialRecipe : ScriptableObject, IComparable<MaterialRecipe>
    {
        [Serializable]
        public struct MaterialCount
        {
            public ItemMaterial material;
            
            [Min(1)]
            public int count;
        }

        [Min(0)]
        public int id;

        public List<MaterialCount> materials = new();

        public Scriptable_ItemBase result;

        public int CompareTo(MaterialRecipe other)
        {
            return id.CompareTo(other.id);
        }
    }
}