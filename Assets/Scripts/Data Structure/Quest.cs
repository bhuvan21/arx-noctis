using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Quest : ScriptableObject
{
    public string sceneName;
    public string questName;
    public string questDesc;
    public Vector3 entryPoint;
    public Vector3 entryScale;
    public List<Item> drops = new List<Item>();
}
