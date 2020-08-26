using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu]
public class Item : ScriptableObject
{
    public string type;
    public string displayType;
    public int price;
    public int sellback;
    public bool sellable;
    public string displayName;
    public int level;
    public string description;

}
