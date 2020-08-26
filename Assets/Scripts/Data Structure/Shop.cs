using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu]
public class Shop : ScriptableObject
{
    public string name;
    public List<Item> items;
}
