using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu]
public class BattleClass : ScriptableObject
{
    public string className;
    public List<Attack> attacks;

}
