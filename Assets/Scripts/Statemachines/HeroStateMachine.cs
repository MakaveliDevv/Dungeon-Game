using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HeroStateMachine : MonoBehaviour
{
    private BattleStateMachine BSM; 

    public BaseHero hero;
    
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

    // Dead
    private bool isAlive = true;

    // Hero panel
    private HeroPanelStats stats;
    public GameObject heroPanel;
    private Transform heroPanelSpacer;


    void Start()
    {
        selector.SetActive(false);
        heroPanelSpacer = GameObject.Find("BattleCanvas").transform.Find("BattlePanel").transform.Find("HeroPanel").transform.Find("HeroPanelSpacer");
        
        // Create panel, fill in info
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
                if(!isAlive) 
                {
                    return;
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

                    // Remove input from performlist
                    for (int i = 0; i < BSM.performList.Count; i++)
                    {
                        if(BSM.performList[i].attackersGobj == this.gameObject) 
                        {
                            BSM.performList.Remove(BSM.performList[i]);
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
                    isAlive = false;
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
        DoDamage();

        // Animate back to start position
        Vector2 initialPosition = startPosition;

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

    public void DoDamage() 
    {
        float calc_damage = hero.curATK + BSM.performList[0].chosenAttack.attackDamage;
        targetToAttack.GetComponent<EnemyStateMachine>().TakeDamge(calc_damage); // Get the esm which represents the hero being attacked
    }

    public void TakeDamge(float _damageAmount) 
    {
        hero.curHP -= _damageAmount;
        if(hero.curHP <= 0) 
        {
            hero.curHP = 0;
            currentState = TurnState.DEAD;
        }

        UpdateHeroPanel();
    }

    private void CreateHeroPanel() 
    {
        heroPanel = Instantiate(heroPanel) as GameObject;
        stats = heroPanel.GetComponent<HeroPanelStats>();

        stats.heroName.text = hero.TheName;
        stats.heroHP.text = "HP: " + hero.curHP;
        stats.heroMP.text = "MP: " + hero.curMP;
        this.progressBar = stats.progressBar;

        heroPanel.transform.SetParent(heroPanelSpacer, false);
    }

    private void UpdateHeroPanel() 
    {
        stats.heroHP.text = "HP: " + hero.curHP;
        stats.heroMP.text = "MP: " + hero.curMP;
    }
}
