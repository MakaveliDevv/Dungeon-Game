using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleStateMachine : MonoBehaviour
{
    public enum BattleStates 
    {
        WAIT,
        TAKEACTION,
        PERFORMACTION
    }

    public BattleStates battleStates;
    public List<HandleTurn> performList = new List<HandleTurn>();

    public List<GameObject> playersInBattle = new List<GameObject>();
    public List<GameObject> enemiesInBattle = new List<GameObject>();

    public enum HeroGUI 
    {
        ACTIVATE,
        WAITING,
        INPUT1,
        INPUT2,
        DONE
    }

    public HeroGUI heroInput;
    public List<GameObject> herosToManage = new List<GameObject>();
    private HandleTurn herosAttack;

    public GameObject targetButton;
    public Transform spacer;

    void Start()
    {
        battleStates = BattleStates.WAIT;
        enemiesInBattle.AddRange(GameObject.FindGameObjectsWithTag("Enemy")); // Change this later, finding gameobject is not the best way to add in a list(use instance). And also by radius or anything like that
        playersInBattle.AddRange(GameObject.FindGameObjectsWithTag("Player")); 

        TargetButtons();

    }

    void Update()
    {
        switch (battleStates)
        {
            case(BattleStates.WAIT):
                // Handle the turns
                if(performList.Count > 0) 
                {
                    battleStates = BattleStates.TAKEACTION;
                }

            break;

            case(BattleStates.TAKEACTION):
                GameObject performer = GameObject.Find(performList[0].Attacker);

                if(performList[0].Type == "Enemy") 
                {
                    EnemyStateMachine ESM = performer.GetComponent<EnemyStateMachine>();
                    ESM.targetToAttack = performList[0].attackersTarget;
                    ESM.currentState = EnemyStateMachine.TurnState.ACTION; // This triggers the Action state
                }

                if(performList[0].Type == "Hero") 
                {
                    
                }

                battleStates = BattleStates.PERFORMACTION;

            break;

            case(BattleStates.PERFORMACTION):

            break;
        }
    }

    public void CollectActions(HandleTurn _input) 
    {
        performList.Add(_input);
    }

    void TargetButtons() 
    {
        foreach (GameObject _enemy in enemiesInBattle)
        {
            GameObject newButton = Instantiate(targetButton);
            SelectButton button = newButton.GetComponent<SelectButton>();

            // Connection to the enemy state machine of the currently selected game object (enemy)
            EnemyStateMachine curEnemy = _enemy.GetComponent<EnemyStateMachine>();

            TextMeshPro buttonText = newButton.GetComponentInChildren<TextMeshPro>();
            buttonText.text = curEnemy.enemy.Name;

            // Need to pass in the content we need for the battle inside of the button, which is the enemy itself as the game object 
            button.enemyObj = _enemy;

            newButton.transform.SetParent(spacer);
        }
    }
}
