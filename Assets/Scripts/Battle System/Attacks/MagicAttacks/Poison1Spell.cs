using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison1Spell : BaseAttack
{
    public Poison1Spell () 
    {
        AttackName = "Poison1";
        AttackDescription = "Basic poison spell which drags damage over time.";
        attackDamage = 5f;
        attackCost = 5f;
    }
}
