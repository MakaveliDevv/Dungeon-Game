using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleStateMachine : MonoBehaviour
{
    // ENUMS
    public enum BattleStates 
    {
        WAIT,
        TAKEACTION,
        PERFORMACTION,
        CHECKALIVE,
        WIN,
        LOSE
    }

    public enum HeroGUI 
    {
        ACTIVATE,
        WAITING,
        DONE
    }

    public BattleStates battleStates;

    // LIST
    public List<HandleTurn> performList = new();
    public List<GameObject> herosInBattle = new();
    public List<GameObject> enemiesInBattle = new();

    // HERO INPUT
    public HeroGUI heroInput;
    private HandleTurn handleTurnScript_HeroChoise;
    [HideInInspector] public List<GameObject> herosToManage = new();

    // PANELS UI
    public GameObject attackParentPanel;
    public GameObject actionPanel;
    public GameObject magicPanel;
    public GameObject enemySelectPanel;

    // SPACERS UI
    public Transform targetSelectSpacer;
    public Transform actionSpacer;
    public Transform magicSpacer;

    // BUTTONS UI
    public GameObject actionBtn;
    public GameObject targetButton;
    public GameObject magicAttButton;
    private readonly List<GameObject> atkBtns = new();
    private readonly List<GameObject> enemyBtns = new();

    // SPAWN
    public List<Transform> spawnPoints = new();

    // TURN
    public bool heroTurn, enemyTurn;
    public float waitBeforeAttack;

    // private GameManager gameManager;


    void Awake() 
    {
        // gameManager = GameObject.FindFirstObjectByType<GameManager>();
        // gameManager.battleWon = false;
        GameManager.instance.battleWon = false;
        
        for (int i = 0; i < GameManager.instance.enemyAmount; i++)
        {
            GameObject newEnemy = Instantiate(GameManager.instance.enemiesToBattle[i], spawnPoints[i].position, Quaternion.identity) as GameObject; // Instantiate a new game object which resembles the enemy
            newEnemy.name = newEnemy.GetComponent<EnemyStateMachine>().enemy.TheName + "_" + (i + 1);
            newEnemy.GetComponent<EnemyStateMachine>().enemy.TheName = newEnemy.name; // Pass the new enemy name to the string
            enemiesInBattle.Add(newEnemy); // Add the enemy in the list
        }
    }

    void Start()
    {
        heroTurn = true;

        // Find all heroes in the scene
        herosInBattle.AddRange(GameObject.FindGameObjectsWithTag("Hero").OrderBy(hero => hero.name)); 

        // Turn the battle state to wait
        battleStates = BattleStates.WAIT;

        // Set the hero input to active
        heroInput = HeroGUI.ACTIVATE;

        // PANELS
        attackParentPanel.SetActive(false);
        actionPanel.SetActive(false);
        enemySelectPanel.SetActive(false);
        magicPanel.SetActive(false);

        // Show the target buttons
        EnemyButtons();
    }

    void Update()
    {
        // Enter the battle scene
        switch (battleStates)
        {
            // Start at the wait state
            case(BattleStates.WAIT):

                // Check if the performlist is populated, which is probably not up untill an action is chose
                if(performList.Count > 0) // When is the performlist populated? When an action is collected or an input hero is done
                {
                    battleStates = BattleStates.TAKEACTION;
                }

            break;

            case(BattleStates.TAKEACTION):
                // Get the first performer from the list, which is most likely the enemy since the enemy doesnt wait for an input and goes by a timer
                GameObject performer = GameObject.Find(performList[0].Attacker);

                // Check if its an enemy
                if(performList[0].Type == "Enemy") 
                {   
                    // If so, then get the EnemyStateMachine
                    EnemyStateMachine ESM = performer.GetComponent<EnemyStateMachine>();

                    // Check for each hero in battle
                    for (int i = 0; i < herosInBattle.Count; i++)
                    {
                        // Check if the target is equal to a hero in the list
                        if(performList[0].attackersTarget == herosInBattle[i])
                        {
                            // Set the game object type of target to attack to the attackers target (HandleTurn class which is used by enemy and hero)
                            ESM.targetToAttack = performList[0].attackersTarget;

                            // Change the enemy state to the action state
                            ESM.currentState = EnemyStateMachine.TurnState.ACTION; 
            
                        } else 
                        {
                            // If there is no target, assign a random target from the heros list 
                            performList[0].attackersTarget = herosInBattle[Random.Range(0, herosInBattle.Count)];

                            // Do the same as above
                            ESM.targetToAttack = performList[0].attackersTarget;
                            ESM.currentState = EnemyStateMachine.TurnState.ACTION; 
                        }
                    }
                }

                // Check if the first performer is a hero
                if(performList[0].Type == "Hero") 
                {
                    // Get the hero sate machine script
                    HeroStateMachine HSM = performer.GetComponent<HeroStateMachine>();

                    // Do the same as with the enemy
                    HSM.targetToAttack = performList[0].attackersTarget;
                    HSM.currentState = HeroStateMachine.TurnState.ACTION;
                }
                
                battleStates = BattleStates.PERFORMACTION;

            break;

            case(BattleStates.PERFORMACTION):
                // Idle state
                // Nothing happens in this state because the action is being performed
            break;

            case(BattleStates.CHECKALIVE):
                if(herosInBattle.Count < 1) 
                {
                    battleStates = BattleStates.LOSE;

                } else if(enemiesInBattle.Count < 1)
                {
                    battleStates = BattleStates.WIN;
                    
                } else 
                {
                    ClearAttackPanel();
                    heroInput = HeroGUI.ACTIVATE;
                }

            break;

            case(BattleStates.WIN):
                GameManager.instance.battleWon = true;
                for (int i = 0; i < herosInBattle.Count; i++)
                {
                    herosInBattle[i].GetComponent<HeroStateMachine>().currentState = HeroStateMachine.TurnState.WAITING;

                    if(GameManager.instance.LoadSceneAfterBattle()) 
                    {
                        GameManager.instance.gameState = GameManager.GameStates.WORLDSTATE;
                        GameManager.instance.enemiesToBattle.Clear();
                    }
                }

            break;

            case(BattleStates.LOSE):
                // Do something

                // Make the player lose some points like, hp or so
                

                // Check if player hp is zero

                // If so then show game over screen

                // Switch to end game screen
                SceneManager.LoadScene("GameOverScene");
            break;
        }
        
        
        // HERO INPUT
        switch (heroInput) 
        {
            case(HeroGUI.ACTIVATE):
                if(herosToManage.Count > 0)
                {
                    herosToManage[0].transform.Find("Selector").gameObject.SetActive(true);
                    handleTurnScript_HeroChoise = new HandleTurn(); // Create new handle turn instance

                    attackParentPanel.SetActive(true);
                    actionPanel.SetActive(true);
                    
                    CreateAttackBTNS(); // Populate the attack panel with attack buttons
                    
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
        performList.Add(_input); // A point where the performlist gets populated
    }

    public void EnemyButtons() 
    {
        // Clean up everything that is already created
        foreach(GameObject _enemyBtn in enemyBtns) 
        {
            Destroy(_enemyBtn);
        }
        enemyBtns.Clear();

        // Proceed further with creating buttons
        foreach (GameObject _enemy in enemiesInBattle)
        {
            GameObject newButton = Instantiate(targetButton) as GameObject;
            newButton.name = "Target: " + _enemy.name;

            SelectButton button = newButton.GetComponent<SelectButton>();
            TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();
            EnemyStateMachine curEnemy = _enemy.GetComponent<EnemyStateMachine>();
            
            buttonText.text = curEnemy.enemy.TheName;
            button.enemyObj = _enemy; 

            newButton.transform.SetParent(targetSelectSpacer, false);
            enemyBtns.Add(newButton);
        }
    }

    public void Input1() // Attack button
    {
        handleTurnScript_HeroChoise.Attacker = herosToManage[0].name;
        handleTurnScript_HeroChoise.attackersGobj = herosToManage[0];
        handleTurnScript_HeroChoise.Type = "Hero";
        handleTurnScript_HeroChoise.chosenAttack = herosToManage[0].GetComponent<HeroStateMachine>().baseHero.attacks[0];

        actionPanel.SetActive(false);
        enemySelectPanel.SetActive(true);
    }

    public void ChooseTarget(GameObject _chosenEnemy) // Gets called in the Select button script 
    {
       handleTurnScript_HeroChoise.attackersTarget = _chosenEnemy;
       heroInput = HeroGUI.DONE;
    }

    private void HeroInputDone()
    {
        performList.Add(handleTurnScript_HeroChoise); // A point where the performlist gets populated. The enemy input gets handled quicker.
        ClearAttackPanel();

        herosToManage[0].transform.Find("Selector").gameObject.SetActive(false);
        herosToManage.RemoveAt(0);
        heroInput = HeroGUI.ACTIVATE;
    }

    private void ClearAttackPanel()
    {
        enemySelectPanel.SetActive(false);
        actionPanel.SetActive(false);
        magicPanel.SetActive(false);
        attackParentPanel.SetActive(false);

        foreach (GameObject _atkBtn in atkBtns)
        {
            Destroy(_atkBtn);
        }
        atkBtns.Clear();
    }

    private void CreateAttackBTNS() 
    {
        // Button to go to the physical att panel
        GameObject attackButton = Instantiate(actionBtn) as GameObject; 
        TextMeshProUGUI attackButtonText = attackButton.transform.Find("AttackText").gameObject.GetComponent<TextMeshProUGUI>();
        attackButton.name = "PhysicalAtk Button"; 
        attackButtonText.text = "Physical"; 

        // Add Input1 method to the physical att button AND set the button in the actionSpacer UI and to the list of buttons
        attackButton.GetComponent<Button>().onClick.AddListener( () => Input1() );
        attackButton.transform.SetParent(actionSpacer, false);
        atkBtns.Add(attackButton);

        // Button to go to the magic att panel
        GameObject magicAttackButton = Instantiate(actionBtn) as GameObject;
        TextMeshProUGUI magicButtonText = magicAttackButton.transform.Find("AttackText").gameObject.GetComponent<TextMeshProUGUI>();
        magicAttackButton.name = "MagicAtk Button";
        magicButtonText.text = "Magic";
        
        // Same as above but with another input method
        magicAttackButton.GetComponent<Button>().onClick.AddListener( () => Input3() );
        magicAttackButton.transform.SetParent(actionSpacer, false);
        atkBtns.Add(magicAttackButton);

        // Create magic att abilities buttons
        if(herosToManage[0].GetComponent<HeroStateMachine>().baseHero.magicAttacks.Count > 0) // Check if the selected hero has at least 1 magic att ability
        {
            foreach (BaseAttack magicAttAbility in herosToManage[0].GetComponent<HeroStateMachine>().baseHero.magicAttacks) // Loop through the list of magic attacks
            {
                GameObject abilityBtn = Instantiate(magicAttButton) as GameObject;
                TextMeshProUGUI abilityBtnText = abilityBtn.transform.Find("SpellText").gameObject.GetComponent<TextMeshProUGUI>();
                abilityBtnText.text = magicAttAbility.name;
                abilityBtn.name = "Magic Att Button: " + abilityBtnText.text + ", " + herosToManage[0].name;
            
                AttackButton ATB = abilityBtn.GetComponent<AttackButton>(); // Fetch the attack button script from the instantiated button
                ATB.magicAttackToPerform = magicAttAbility; // Assign the magic att ability to the magic attack to perform
                abilityBtn.transform.SetParent(magicSpacer, false);
                atkBtns.Add(abilityBtn);
            }
        } else 
        {
            // If not then make the button not interactable
            magicAttackButton.GetComponent<Button>().interactable = false;
        }
    }

    public void Input3() // Switching to magic attacks 
    {
        actionPanel.SetActive(false);
        magicPanel.SetActive(true);
    }

    public void Input4(BaseAttack _chosenMagicSpell) // Magic att Panel
    {
        handleTurnScript_HeroChoise.Attacker = herosToManage[0].name;
        handleTurnScript_HeroChoise.attackersGobj = herosToManage[0];
        handleTurnScript_HeroChoise.Type = "Hero";

        handleTurnScript_HeroChoise.chosenAttack = _chosenMagicSpell;
        magicPanel.SetActive(false);
        enemySelectPanel.SetActive(true);
    }
}
