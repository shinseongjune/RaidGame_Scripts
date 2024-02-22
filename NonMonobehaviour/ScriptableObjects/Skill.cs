using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "ScriptableObjects/Skill")]
public class Skill : ScriptableObject, IComparable<Skill>
{
    [Min(0)]
    public int id;

    public enum Type
    {
        PROJECTILE,
        PLACE,
        TARGET,
        INSTANT,
    }

    public string skillName;
    public string description;

    public Type type;

    //TODO: skill slots에서 range 구현.
    public float range;

    public enum CostStat
    {
        HP,
        MP,
    }

    public CostStat costStat = CostStat.MP;

    public float cost;
    public float coolDown;

    public float preDelay;
    public float postDelay;

    public GameObject skillPrefab;
    public List<GameObject> afterEffectPrefabs = new List<GameObject>();

    public int CompareTo(Skill other)
    {
        return id.CompareTo(other.id);
    }

    public Sprite sprite;
    //TODO: 모션타입, 태그리스트(평타,공격/마법,투사체~~,근접/원거리 기타등등)
}

//TODO:나중) 패시브, 토글, 자동실행(그냥 bool로 해도 될지도?) 등
//TODO:바닥표시자 변수.