using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectButton : MonoBehaviour
{
    public GameObject enemyObj;
    private GameObject selector;

    public void SelectEnemy() 
    {
        GameObject.Find("BattleManager").GetComponent<BattleStateMachine>().ChooseTarget(enemyObj); // Save input of the enemy
    }

    public void HideSelector() 
    {
        selector = enemyObj.transform.Find("Selector").gameObject;
        selector.SetActive(false);
    }

    public void ShowSelector() 
    {
        selector = enemyObj.transform.Find("Selector").gameObject;
        selector.SetActive(true);
    }  
}
