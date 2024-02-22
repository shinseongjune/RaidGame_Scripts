using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Scriptable_ItemBase : ScriptableObject
{
    [Min(0)]
    public int id;

    public string itemName;
    public string description;

    public Sprite sprite;
}
