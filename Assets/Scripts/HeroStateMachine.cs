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

    [SerializeField] private TurnState currentState;
    [SerializeField] private float cur_cooldown; // Remove serializefield later
    public float maxCooldown = .75f;

    public Image progressBar;
    public GameObject selector;
    void Start()
    {
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>(); // Change to instance
        currentState = TurnState.PROCESSING;
        selector.SetActive(false);
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

            break;

            case (TurnState.DEAD):

            break;
            
            // default:
        }
    }

    private void ProgressBar() 
    {
        cur_cooldown += Time.deltaTime;
        float calc_cooldown = cur_cooldown / maxCooldown; // Calculation of the cool down
        progressBar.transform.localScale = new Vector3(Mathf.Clamp(calc_cooldown, 0, 1), progressBar.transform.localScale.y, progressBar.transform.localScale.z);

        if(cur_cooldown >= maxCooldown) // If current cooldown reaches the maximum cooldown, then the processing state is over
        {
            currentState = TurnState.ADDTOLIST;
        }

    }
}
