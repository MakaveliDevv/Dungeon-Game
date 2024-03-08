using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerSwing : BaseAttack
{
    public HammerSwing() 
    {
        AttackName = "Hammer Swing";
        AttackDescription = "A heavy hammer swing meant for destruction";
        attackDamage = 15f;
        attackCost = 0;
    }
}
