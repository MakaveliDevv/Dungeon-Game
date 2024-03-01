using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HeroStateMachine : MonoBehaviour
{
    private BattleStateMachine BSM; 

    [SerializeField] private BaseHero hero;
    
    public enum TurnState 
    {
        PROCESSING,
        ADDTOLIST,
        WAITING,
        SELECTING,
        ACTION,
        DEAD
    }

    public TurnState currentState;
    private float maxCooldown = 1f;
    private float curCooldown;

    public Image progressBar;
    public GameObject selector;

    // IENumerator
    public GameObject targetToAttack;
    private Vector2 startPosition;
    private bool actionStarted = false;
    private float animSpeed = 10f;

    void Start()
    {
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>(); // Change to instance
        currentState = TurnState.PROCESSING;
        startPosition = transform.position;
    }

    void Update()
    {
        switch (currentState)
        {
            case (TurnState.PROCESSING):
                ProgressBar();

            break;

            case (TurnState.ADDTOLIST):
                BSM.herosToManage.Add(this.gameObject);

                currentState = TurnState.WAITING;

            break;

            case (TurnState.WAITING):
                // Idle state

            break;

            case (TurnState.SELECTING):

            break;

            case (TurnState.ACTION):
                StartCoroutine(TimeForAction());
                
            break;

            case (TurnState.DEAD):

            break;
            
            // default:
        }
    }

    private void ProgressBar() 
    {
        curCooldown += Time.deltaTime;
        float calc_cooldown = curCooldown / maxCooldown; // Calculation of the cool down
        progressBar.transform.localScale = new Vector3(Mathf.Clamp(calc_cooldown, 0, 1), progressBar.transform.localScale.y, progressBar.transform.localScale.z);

        if(curCooldown >= maxCooldown) // If current cooldown reaches the maximum cooldown, then the processing state is over
        {
            currentState = TurnState.ADDTOLIST;
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
        Vector2 targetPosition = new Vector2(targetToAttack.transform.position.x - 1.5f, targetToAttack.transform.position.y);
        while(MoveTowardsTarget(targetPosition)) { yield return null; } // Change while loop to something else

        // Wait a bit
        yield return new WaitForSeconds(1f);

        // Do damage

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
        curCooldown = 0f;
        currentState = TurnState.PROCESSING;
    }

    private bool MoveTowardsTarget(Vector2 _targetPosition)
    {
        Vector2 newPosition = Vector2.MoveTowards(transform.position, _targetPosition, animSpeed * Time.deltaTime);
        transform.position = newPosition;
        return newPosition != _targetPosition;
    }
    
    public void TakeDamge(float _getDamageAmount) 
    {

    }
}
