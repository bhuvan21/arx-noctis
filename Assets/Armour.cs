using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[CreateAssetMenu]
public class Armour : Item
{
    public string equipType;

    [Serializable]
    public struct Stat
    {
        public string name;
        public int value;

        public Stat(string statName, int statValue)
        {
            name = statName;
            value = statValue;
        }
    }
    [Serializable]
    public struct Resistance
    {
        public string name;
        public int value;

        public Resistance(string resistanceName, int resistanceValue)
        {
            name = resistanceName;
            value = resistanceValue;
        }
    }


    public List<Stat> stats = new List<Stat> 
    {
        new Stat("power", 0),
        new Stat("immunity", 0),
        new Stat("endurance", 0),
        new Stat("wisdom", 0),
        new Stat("luck", 0),
        new Stat("recovery", 0)
    };
    public List<Resistance> resistances = new List<Resistance>
    {
        new Resistance("fire", 0),
        new Resistance("water", 0),
        new Resistance("darkness", 0),
        new Resistance("light", 0),
        new Resistance("wind", 0),
        new Resistance("stone", 0),
        new Resistance("ice", 0),
        new Resistance("nature", 0),
        new Resistance("metal", 0),
    };
}
