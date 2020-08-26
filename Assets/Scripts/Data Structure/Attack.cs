using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu]
public class Attack : ScriptableObject
{
    public string name;
    public int placement;
    public float multiplier;
    public int startingCooldown;
    public int cooldown;
    public int mana;
    public string animName;
    public string desc;
    public List<StatusEffect> myEffects;
    public List<StatusEffect> theirEffects;
}
