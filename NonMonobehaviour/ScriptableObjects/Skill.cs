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

    //TODO: skill slots���� range ����.
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
    //TODO: ���Ÿ��, �±׸���Ʈ(��Ÿ,����/����,����ü~~,����/���Ÿ� ��Ÿ���)
}

//TODO:����) �нú�, ���, �ڵ�����(�׳� bool�� �ص� ������?) ��
//TODO:�ٴ�ǥ���� ����.