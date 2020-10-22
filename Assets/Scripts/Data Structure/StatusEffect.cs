using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu]
public class StatusEffect : ScriptableObject
{
    public string name;
    public int turns;
    public string description;

    public int crit;
    public int bth;
    public int defense;
    public float boost;

    public List<Armour.Resistance> resistances;

    public bool self;
}
