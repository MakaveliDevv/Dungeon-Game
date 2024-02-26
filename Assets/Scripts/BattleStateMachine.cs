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
    private HandleTurn herosChoise;

    public GameObject targetButton;
    public Transform spacer;

    public GameObject attackPanel;
    public GameObject enemySelectPanel;


    void Start()
    {
        battleStates = BattleStates.WAIT;
        heroInput = HeroGUI.ACTIVATE;

        enemiesInBattle.AddRange(GameObject.FindGameObjectsWithTag("Enemy")); // Change this later, finding gameobject is not the best way to add in a list(use instance). And also by radius or anything like that
        playersInBattle.AddRange(GameObject.FindGameObjectsWithTag("Player")); 

        attackPanel.SetActive(false);
        enemySelectPanel.SetActive(false);

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
                    Debug.Log("Hero is attacking...");
                }

                battleStates = BattleStates.PERFORMACTION;

            break;

            case(BattleStates.PERFORMACTION):

            break;
        }
        
        switch (heroInput) 
        {
            case(HeroGUI.ACTIVATE):
                if(herosToManage.Count > 0)
                {
                    herosToManage[0].transform.Find("Selector").gameObject.SetActive(true);
                    herosChoise = new HandleTurn(); // Instance 

                    attackPanel.SetActive(true);
                    heroInput = HeroGUI.WAITING;
                }

            break;

            case(HeroGUI.WAITING):
                // Idle state

            break;

            case(HeroGUI.DONE):
                HeroInputDone();

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
            newButton.name = "TargetButton";

            SelectButton button = newButton.GetComponent<SelectButton>();
            EnemyStateMachine curEnemy = _enemy.GetComponent<EnemyStateMachine>();
            TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();
            
            buttonText.text = curEnemy.enemy.Name;
            button.enemyObj = _enemy; // Pass in the content we need for the battle inside of the button, which is the enemy itself as the game object 


            newButton.transform.SetParent(spacer, false);
        }
    }

    public void Input1() // Attack button
    {
        herosChoise.Attacker = herosToManage[0].name;
        herosChoise.attackersGobj = herosToManage[0];
        herosChoise.Type = "Hero";

        attackPanel.SetActive(false);
        enemySelectPanel.SetActive(true);
    }

    public void Input2(GameObject _chosenEnemy) 
    {
       herosChoise.attackersTarget = _chosenEnemy;
       heroInput = HeroGUI.DONE;
    }

    void HeroInputDone()
    {
        performList.Add(herosChoise);
        enemySelectPanel.SetActive(false);
        herosToManage[0].transform.Find("Selector").gameObject.SetActive(false);
        herosToManage.RemoveAt(0);
        heroInput = HeroGUI.ACTIVATE;
    }
}
