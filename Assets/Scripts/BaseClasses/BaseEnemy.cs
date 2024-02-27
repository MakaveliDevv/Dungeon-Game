using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseEnemy : BaseClass
{    

    public enum Type 
    {
        GRASS,
        FIRE,
        WATER,
        ELEKTRIC
    }

    public enum Rarity 
    {
        COMMON,
        UNCOMMON,
        RARE,
        SUPERRARE
    }    

    [SerializeField] private Type enemyType;
    [SerializeField] private Rarity rarity;
}
