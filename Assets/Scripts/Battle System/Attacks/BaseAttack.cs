using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseAttack : MonoBehaviour
{
    public string AttackName;
    public string AttackDescription;
    public float attackDamage;
    public float attackCost; // Mana cost
}
