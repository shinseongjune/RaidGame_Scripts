using Item;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterBaseData", menuName = "ScriptableObjects/CharacterBaseData")]
public class CharacterBaseData : ScriptableObject, IComparable<CharacterBaseData>
{
    [Min(0)]
    public int id;

    public string Name;

    public float MaxHP;
    public float MaxMP;
    public float Might;
    public float Armor;
    public float FireResist;
    public float ColdResist;
    public float LightningResist;
    public float CritChance;
    public float CritDamage;

    public float MovementSpeed;

    public GameObject prefab;
    public GameObject mapPrefab;

    public List<ItemMaterial> rewards;

    public Sprite sprite;

    public int CompareTo(CharacterBaseData other)
    {
        return id.CompareTo(other.id);
    }
}
