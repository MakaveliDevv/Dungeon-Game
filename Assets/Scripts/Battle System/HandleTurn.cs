using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HandleTurn
{
    public string Attacker; // Name of attacker
    public string Type;
    public GameObject attackersGobj; // Who attacks
    public GameObject attackersTarget; // Who is going to be attacked

    // Which attack is performed
    public BaseAttack chosenAttack;

}
