using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : BaseAttack
{
    public Slash() 
    {
        AttackName = "Slash";
        AttackDescription = "A quick and stable slash";
        attackDamage = 10f;
        attackCost = 0;
    }
}
