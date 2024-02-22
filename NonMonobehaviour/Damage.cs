using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Damage
{
    public enum Type
    {
        Physical,
        Fire,
        Cold,
        Lightning
    }

    public float damage;
    public Type type;

    public GameObject source;
    public GameObject target;

    //TODO:관통 등등
}
