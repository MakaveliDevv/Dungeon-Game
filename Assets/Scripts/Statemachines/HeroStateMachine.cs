using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HeroStateMachine : MonoBehaviour
{
    private BattleStateMachine BSM; 

    public BaseHero baseHero;
    
    public enum TurnState 
    {
        PROCESSING,
        ADDTOLIST,
        WAITING,
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

    // Dead
    private bool alive = true;

    // Hero panel
    private HeroPanelStats stats;
    public GameObject heroPanelUI;
    private Transform heroPanelSpacer;


    void Start()
    {
        selector.SetActive(false);
        heroPanelSpacer = GameObject.Find("BattleCanvas").transform.Find("BattlePanel").transform.Find("HeroPanel").transform.Find("HeroPanelSpacer");
    
        // Create panel
        CreateHeroPanel();

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

            case (TurnState.ACTION):
                StartCoroutine(TimeForAction());
                
            break;

            case (TurnState.DEAD):
                if(!alive) 
                {
                    return; // Do nothing if hero still alive
                } else 
                {   
                    // Change tag
                    this.gameObject.tag = "DeadHero";

                    // Not attackable by enemy
                    BSM.herosInBattle.Remove(this.gameObject);

                    // Not managable
                    BSM.herosToManage.Remove(this.gameObject);

                    // Deactivate the selector
                    selector.SetActive(false);

                    // Reset GUI
                    BSM.actionPanel.SetActive(false);
                    BSM.enemySelectPanel.SetActive(false);

                    // Remove inputs from performlist
                    if(BSM.herosInBattle.Count > 0) 
                    {
                        for (int i = 0; i < BSM.performList.Count; i++)
                        {
                            if(i != 0) 
                            {
                                if(BSM.performList[i].attackersGobj == this.gameObject) 
                                {
                                    BSM.performList.Remove(BSM.performList[i]);
                                }

                                // Check if the target of the enemy is this hero
                                if(BSM.performList[i].attackersTarget == this.gameObject) 
                                {
                                    // Change target to random target
                                    BSM.performList[i].attackersTarget = BSM.herosInBattle[Random.Range(0, BSM.herosInBattle.Count)];
                                }
                            }
                        }
                    }

                    // Change color / play animation
                    SpriteRenderer spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null)
                    {
                        spriteRenderer.material.color = new Color32(105, 105, 105, 255);
                    }
                    else
                    {
                        Debug.LogError("SpriteRenderer component not found on the game object.");
                    }

                    // Reset hero input
                    BSM.battleStates = BattleStateMachine.BattleStates.CHECKALIVE;
                    alive = false;
                }
            break;
            
            // default:
        }
    }

    private void ProgressBar() 
    {
        curCooldown += Time.deltaTime;
        float calc_cooldown = curCooldown / maxCooldown; // Calculation of the cool down
        progressBar.transform.localScale = new Vector3(Mathf.Clamp(calc_cooldown, 0, 1), progressBar.transform.localScale.y, progressBar.transform.localScale.z);

        if(curCooldown >= maxCooldown)
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
        DoDamage();

        // Animate back to start position
        Vector2 initialPosition = startPosition;
        while(MoveTowardsTarget(initialPosition)) { yield return null; } // Change while loop to something else

        // Remove this performer from the list in the BSM
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

        // End coroutine
        actionStarted = false;
    }

    private bool MoveTowardsTarget(Vector2 _targetPosition)
    {
        Vector2 newPosition = Vector2.MoveTowards(transform.position, _targetPosition, animSpeed * Time.deltaTime);
        transform.position = newPosition;
        return newPosition != _targetPosition;
    }

    public void DoDamage() 
    {
        float calc_damage = baseHero.curATK + BSM.performList[0].chosenAttack.attackDamage;
        targetToAttack.GetComponent<EnemyStateMachine>().TakeDamge(calc_damage); // Get the esm which represents the hero being attacked
    }

    public void TakeDamge(float _damageAmount) 
    {
        baseHero.curHP -= _damageAmount;
        if(baseHero.curHP <= 0) 
        {
            baseHero.curHP = 0;
            currentState = TurnState.DEAD;
        }

        UpdateHeroPanel();
    }

    private void CreateHeroPanel() 
    {
        heroPanelUI = Instantiate(heroPanelUI) as GameObject;
        stats = heroPanelUI.GetComponent<HeroPanelStats>();

        stats.heroName.text = baseHero.TheName;
        stats.heroHP.text = "HP: " + baseHero.curHP;
        stats.heroMP.text = "MP: " + baseHero.curMP;
        this.progressBar = stats.progressBar;

        heroPanelUI.transform.SetParent(heroPanelSpacer, false);
    }

    private void UpdateHeroPanel() 
    {
        stats.heroHP.text = "HP: " + baseHero.curHP;
        stats.heroMP.text = "MP: " + baseHero.curMP;
    }
}
