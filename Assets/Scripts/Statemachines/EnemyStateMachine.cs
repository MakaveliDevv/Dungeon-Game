using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    public BattleStateMachine BSM; 
    public BaseEnemy enemy;

    public enum TurnState 
    {
        PROCESSING,
        CHOOSEACTION,
        WAITING,
        ACTION,
        DEAD
    }

    public TurnState currentState;
    private Vector2 startPosition;
    public GameObject selector;    
    
    // For the progressbar
    private float cur_cooldown;
    private float max_cooldown = 1f;


    // Time for action
    private bool actionStarted = false;
    private float animSpeed = 10f;
    public GameObject targetToAttack;

    void Start()
    {
        currentState = TurnState.PROCESSING;
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
        startPosition = transform.position;
        selector.SetActive(false);
    }

    void Update()
    {
        switch (currentState)
        {
            case (TurnState.PROCESSING):
                ProgressBar();

            break;

            case (TurnState.CHOOSEACTION):
                ChooseAction();

            break;

            case (TurnState.WAITING):
                // Idle state

            break;

            case (TurnState.ACTION):
                StartCoroutine(TimeForAction());

            break;

            case (TurnState.DEAD):

            break;
            
            // default:
        }
    }

    private void  ProgressBar() 
    {
        cur_cooldown += Time.deltaTime;
        if(cur_cooldown >= max_cooldown) // If current cooldown reaches the maximum cooldown, then the processing state is over
        {
            currentState = TurnState.CHOOSEACTION;
        }
    }

    private void ChooseAction() 
    {
        if(BSM.performList.Count == 0) 
        {
            HandleTurn myAttack = new HandleTurn();
            myAttack.Attacker = enemy.TheName;
            myAttack.Type = "Enemy";
            myAttack.attackersGobj = this.gameObject;
            myAttack.attackersTarget = BSM.herosInBattle[Random.Range(0, BSM.herosInBattle.Count)]; // Randomize the target

            // Choose attack
            int num = Random.Range(0, enemy.attacks.Count);
            myAttack.chosenAttack = enemy.attacks[num];

            // 
            
            // Debug
            Debug.Log(this.gameObject.name + " has choosen " + myAttack.chosenAttack.AttackName + " and does " + myAttack.chosenAttack.attackDamage + " damage");

            BSM.CollectActions(myAttack);
            currentState = TurnState.WAITING;   
        }
    }

    private IEnumerator TimeForAction() 
    {
        if(actionStarted) 
        {
            yield break;            
        }
        
        actionStarted = true;

        // Animate the enemy near the hero to attack
        Vector2 targetPosition = new Vector2(targetToAttack.transform.position.x + 1.5f, targetToAttack.transform.position.y);
        while(MoveTowardsTarget(targetPosition)) { yield return null; } // Change while loop to something else

        // Wait a bit
        yield return new WaitForSeconds(1f);

        // Do damage
        DoDamage();

        // Animate back to start position
        Vector2 initialPosition = startPosition;
        Debug.Log(initialPosition);

        while(MoveTowardsTarget(initialPosition)) { yield return null; } // Change while loop to something else

        // Remove this performer from the list in the BSM
        BSM.performList.RemoveAt(0);

        // Reset the BSM -> WAIT
        BSM.battleStates = BattleStateMachine.BattleStates.WAIT;

        // End coroutine
        actionStarted = false;

        // Reset the enemy state
        cur_cooldown = 0f;
        currentState = TurnState.PROCESSING;
    }

    private bool MoveTowardsTarget(Vector2 _targetPosition)
    {
        Vector2 newPosition = Vector2.MoveTowards(transform.position, _targetPosition, animSpeed * Time.deltaTime);
        transform.position = newPosition;
        return newPosition != _targetPosition;
    }

    public void DoDamage() 
    {
        float calc_damage = enemy.curATK + BSM.performList[0].chosenAttack.attackDamage;
        // Get the hsm which represents the hero being attacked
        targetToAttack.GetComponent<HeroStateMachine>().TakeDamge(calc_damage);
    }
}
