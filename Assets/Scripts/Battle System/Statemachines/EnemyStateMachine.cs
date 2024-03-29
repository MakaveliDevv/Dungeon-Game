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
    private float curCooldown;
    public float maxCooldown = 10f;


    // Time for action
    private bool actionStarted = false;
    private float animSpeed = 10f;
    public GameObject targetToAttack;


    // Enemy
    public GameObject dungeonEnemy; 
    private bool alive = true;

    void Start()
    {
        currentState = TurnState.PROCESSING;
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
        startPosition = transform.position;
        selector.SetActive(false);
    }

    void Update()
    {
        // Enter the turn state machine
        switch (currentState)
        {
            case (TurnState.PROCESSING): // Enemy gets into this state fomr the start of the game just at the same time as checking for a performlist in the bsm
                ProgressBar();

            break;

            case (TurnState.CHOOSEACTION):
                // Enemy gets into this state after the progressbar is full 
                // This triggers the takeaction state in bsm since this method adds an attacker in the performlist
                ChooseAction(); 

            break;

            case (TurnState.WAITING):
                // Idle state
                // Enemy turns into next sate in the bsm takeaction state

            break;

            case (TurnState.ACTION):
                StartCoroutine(TimeForAction()); // Enemy turns into this state from the bsm when the bsm is at the takeaction state

                BSM.enemyTurn = false;
                BSM.heroTurn = true;
            break;

            case (TurnState.DEAD):
                if(!alive) 
                {
                    return;
                } else 
                {
                    // Change tag of enemy
                    this.gameObject.tag = "DeadEnemy";

                    // Not attackable by hero
                    BSM.enemiesInBattle.Remove(this.gameObject);

                    // Disable the selector
                    selector.SetActive(false);

                    // Remove all inputs enemy attackss
                    if(BSM.enemiesInBattle.Count > 0) 
                    {
                        for (int i = 0; i < BSM.performList.Count; i++)
                        {
                            if(i != 0) 
                            {
                                if(BSM.performList[i].attackersGobj == this.gameObject) 
                                {
                                    BSM.performList.Remove(BSM.performList[i]);
                                }

                                // Check if the target of the hero is this enemy
                                if(BSM.performList[i].attackersTarget == this.gameObject) 
                                {
                                    BSM.performList[i].attackersTarget = BSM.enemiesInBattle[Random.Range(0, BSM.enemiesInBattle.Count)];
                                }
                            }
                        }
                    }

                    // Change the color / play dead animation
                    if(this.gameObject.TryGetComponent<SpriteRenderer>(out var spriteRenderer)) 
                    {
                        spriteRenderer.material.color = new Color32(105, 105, 105, 255);
                    } else 
                    {
                        Debug.LogError("SpriteRenderer component not found on the game object.");
                    }

                    alive = false;

                    // Reset enemy buttons
                    BSM.EnemyButtons();

                    // Check if this enemy is alive (turn to CHECK ALIVE state)
                    BSM.battleStates = BattleStateMachine.BattleStates.CHECKALIVE;
                }

            break;
            
            // default:
        }
    }

    private void  ProgressBar() 
    {
        if(BSM.enemyTurn && !BSM.heroTurn) 
        {
            curCooldown += Time.deltaTime;

            if(curCooldown >= maxCooldown) // If current cooldown reaches the maximum cooldown, then the processing state is over
            {
                currentState = TurnState.CHOOSEACTION;
            }
        }
        // float calc_cooldown = curCooldown / maxCooldown; // Calculation of the cool down
    }

    private void ChooseAction() 
    {
        // Check if performlist is empty
        if(BSM.performList.Count == 0) 
        {
            // If so then create a new instance of HandleTurn (this handles the information about the action)
            HandleTurn myAttack = new()
            {
                Attacker = enemy.TheName,
                Type = "Enemy",
                attackersGobj = this.gameObject,
                attackersTarget = BSM.herosInBattle[Random.Range(0, BSM.herosInBattle.Count)] // Randomize the target
            };

            // Initialize a random attack from the attacks in the enemy class
            int num = Random.Range(0, enemy.attacks.Count);

            // ChosenAttack holds all the information about the attack, it's a class from BaseAttack
            // myAttack = HandleTurn class
            // chosenAttack = BaseAttack class
            // enemy = BaseEnemy:BaseClass
            // attacks = BaseAttack
            myAttack.chosenAttack = enemy.attacks[num];
            
            // Debug
            Debug.Log(this.gameObject.name + " has choosen " + myAttack.chosenAttack.AttackName + " and does " + myAttack.chosenAttack.attackDamage + " damage");
            BSM.CollectActions(myAttack); // This will add the input to the collect actions

            currentState = TurnState.WAITING;   
        }
    }

    private IEnumerator TimeForAction() 
    {
        // Check if there is already an action started, by default not
        if(actionStarted) 
        {
            yield break;            
        }
        
        actionStarted = true;

        // Animate the enemy near the hero to attack
        Vector2 targetPosition = new(targetToAttack.transform.position.x, targetToAttack.transform.position.y + 1.5f);
        while(MoveTowardsTarget(targetPosition)) { yield return null; } // Change while loop to something else

        // Wait a bit
        yield return new WaitForSeconds(1f);

        // Do damage
        DoDamage();

        // Animate back to start position
        Vector2 initialPosition = startPosition;
        while(MoveTowardsTarget(initialPosition)) { yield return null; } // Change while loop to something else

        // Remove the enemy from the list in the BSM
        BSM.performList.RemoveAt(0);

        // Reset the battle state to waiting after completing an action
        if(BSM.battleStates != BattleStateMachine.BattleStates.WIN && BSM.battleStates != BattleStateMachine.BattleStates.LOSE) 
        {
            BSM.battleStates = BattleStateMachine.BattleStates.WAIT;

            // Reset the hero state
            curCooldown = 0f;
            currentState = TurnState.PROCESSING;
        } else 
        {
            // Set the hero state back to waiting (idle)
            currentState = TurnState.WAITING;
        }

        // // Reset the BSM -> WAIT
        // BSM.battleStates = BattleStateMachine.BattleStates.WAIT; // This checks for a performer in the list again


        // // Reset the enemy state
        // curCooldown = 0f;
        // currentState = TurnState.PROCESSING; // This triggers the progressbar to go again
        
        // End coroutine
        actionStarted = false;
        
        yield return new WaitForSeconds(BSM.waitBeforeAttack);

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

    public void TakeDamge(float _damageAmount) 
    {
        enemy.curHP -= _damageAmount;
        if(enemy.curHP <= 0) 
        {
            enemy.curHP = 0;
            currentState = TurnState.DEAD;
        }
    } 
}
