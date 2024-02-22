using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillBase : MonoBehaviour
{
    public GameObject target;
    public GameObject owner;
    public Skill source;
    public List<GameObject> alreadyHitObjects = new List<GameObject>();

    public Vector3 clickedPoint;

    /// <summary>
    /// Use this when you need initialization
    /// </summary>
    public abstract void GetOn();
}
