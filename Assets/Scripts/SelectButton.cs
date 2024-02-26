using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectButton : MonoBehaviour
{
    public GameObject enemyObj;

    public void SelectEnemy() 
    {
        GameObject.Find("BattleManager").GetComponent<BattleStateMachine>(); // Save input of the enemy
    }  
}
