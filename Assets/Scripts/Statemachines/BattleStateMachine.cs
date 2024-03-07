using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleStateMachine : MonoBehaviour
{
    public enum BattleStates 
    {
        WAIT,
        TAKEACTION,
        PERFORMACTION,
        CHECKALIVE,
        WIN,
        LOSE
    }

    public BattleStates battleStates;
    public List<HandleTurn> performList = new();

    public List<GameObject> herosInBattle = new();
    public List<GameObject> enemiesInBattle = new();

    public enum HeroGUI 
    {
        ACTIVATE,
        WAITING,
        DONE
    }

    // Hero input
    public HeroGUI heroInput;
    [HideInInspector] public List<GameObject> herosToManage = new();
    private HandleTurn herosChoise;

    // Panels
    public GameObject attackParentPanel;
    public GameObject actionPanel;
    public GameObject magicPanel;
    public GameObject enemySelectPanel;

    // Spacers
    public Transform targetSelectSpacer;
    public Transform actionSpacer;
    public Transform magicSpacer;

    // Buttons
    public GameObject actionBtn;
    public GameObject targetButton;
    public GameObject magicButton;
    private List<GameObject> atkBtns = new();

    // Enemy buttons
    private List<GameObject> enemyBtns = new();

    // Spawn points
    public List<Transform> spawnPoints = new();

    void Awake() 
    {
        for (int i = 0; i < GameManager.instance.enemyAmount; i++)
        {
            GameObject newEnemy = Instantiate(GameManager.instance.enemiesToBattle[i], spawnPoints[i].position, Quaternion.identity) as GameObject;
            newEnemy.name = newEnemy.GetComponent<EnemyStateMachine>().enemy.TheName + "_" + (i+1);
            newEnemy.GetComponent<EnemyStateMachine>().enemy.TheName = newEnemy.name;
            enemiesInBattle.Add(newEnemy);
        }
    }

    void Start()
    {
        battleStates = BattleStates.WAIT;
        heroInput = HeroGUI.ACTIVATE;

        herosInBattle.AddRange(GameObject.FindGameObjectsWithTag("Hero").OrderBy(hero => hero.name)); 

        // Panels
        attackParentPanel.SetActive(false);
        actionPanel.SetActive(false);
        enemySelectPanel.SetActive(false);
        magicPanel.SetActive(false);

        EnemyButtons();
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
                    for (int i = 0; i < herosInBattle.Count; i++)
                    {
                        if(performList[0].attackersTarget == herosInBattle[i]) // If currently attacked hero is in the herosbattle list
                        {
                            ESM.targetToAttack = performList[0].attackersTarget;
                            ESM.currentState = EnemyStateMachine.TurnState.ACTION; // This triggers the Action state
                            break;
                        } else 
                        {
                            performList[0].attackersTarget = herosInBattle[Random.Range(0, herosInBattle.Count)];

                            ESM.targetToAttack = performList[0].attackersTarget;
                            ESM.currentState = EnemyStateMachine.TurnState.ACTION; // This triggers the Action state
                        }
                    }
                }

                if(performList[0].Type == "Hero") 
                {
                    HeroStateMachine HSM = performer.GetComponent<HeroStateMachine>();
                    HSM.targetToAttack = performList[0].attackersTarget;
                    HSM.currentState = HeroStateMachine.TurnState.ACTION;
                }

                battleStates = BattleStates.PERFORMACTION;

            break;

            case(BattleStates.PERFORMACTION):
                // Idle state
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
                Debug.Log("You won the battle");
                for (int i = 0; i < herosInBattle.Count; i++)
                {
                    herosInBattle[i].GetComponent<HeroStateMachine>().currentState = HeroStateMachine.TurnState.WAITING;

                    GameManager.instance.LoadSceneAfterBattle();
                    GameManager.instance.gameState = GameManager.GameStates.WORLDSTATE;
                    GameManager.instance.enemiesToBattle.Clear();
                }

            break;

            case(BattleStates.LOSE):
                Debug.Log("You lost the battle");
            break;
        }
        
        switch (heroInput) 
        {
            case(HeroGUI.ACTIVATE):
                if(herosToManage.Count > 0)
                {
                    herosToManage[0].transform.Find("Selector").gameObject.SetActive(true);
                    herosChoise = new HandleTurn(); // Create new handle turn instance

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
        performList.Add(_input);
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
            EnemyStateMachine curEnemy = _enemy.GetComponent<EnemyStateMachine>();
            TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();
            
            buttonText.text = curEnemy.enemy.TheName;
            button.enemyObj = _enemy; // Pass in the content we need for the battle inside of the button, which is the enemy itself as the game object 


            newButton.transform.SetParent(targetSelectSpacer, false);
            enemyBtns.Add(newButton); // Add buttons to the list
        }
    }

    public void Input1() // Attack button
    {
        herosChoise.Attacker = herosToManage[0].name;
        herosChoise.attackersGobj = herosToManage[0];
        herosChoise.Type = "Hero";
        herosChoise.chosenAttack = herosToManage[0].GetComponent<HeroStateMachine>().baseHero.attacks[0];

        actionPanel.SetActive(false);
        enemySelectPanel.SetActive(true);
    }

    public void Input2(GameObject _chosenEnemy) // Select enemy button
    {
       herosChoise.attackersTarget = _chosenEnemy;
       heroInput = HeroGUI.DONE;
    }

    private void HeroInputDone()
    {
        performList.Add(herosChoise);
        
        // Clean the attack panel
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

        // Clean up the attackpanel
        foreach (GameObject _atkBtn in atkBtns)
        {
            Destroy(_atkBtn);
        }
        atkBtns.Clear();
    }

    private void CreateAttackBTNS() 
    {
        // Button for the the physical attack
        GameObject attackButton = Instantiate(actionBtn) as GameObject; // Create new magic attack button for an action
        TextMeshProUGUI attackButtonText = attackButton.transform.Find("AttackText").gameObject.GetComponent<TextMeshProUGUI>(); // Fetch the text 

        attackButton.name = "PhysicalAtk Button"; // Name the game object
        attackButtonText.text = "Physical"; // Name the text
        
        attackButton.GetComponent<Button>().onClick.AddListener( () => Input1() );
        attackButton.transform.SetParent(actionSpacer, false);
        atkBtns.Add(attackButton);

        // Button for the magic attack
        GameObject magicAttackButton = Instantiate(actionBtn) as GameObject;
        TextMeshProUGUI magicButtonText = magicAttackButton.transform.Find("AttackText").gameObject.GetComponent<TextMeshProUGUI>();

        magicAttackButton.name = "MagicAtk Button";
        magicButtonText.text = "Magic";
        
        magicAttackButton.GetComponent<Button>().onClick.AddListener( () => Input3() );
        magicAttackButton.transform.SetParent(actionSpacer, false);
        atkBtns.Add(magicAttackButton);

        // Create spell buttons
        if(herosToManage[0].GetComponent<HeroStateMachine>().baseHero.magicAttacks.Count > 0) // Check after creating magic button if there is any skill in that magicAtk list
        {
            foreach (BaseAttack spellAtk in herosToManage[0].GetComponent<HeroStateMachine>().baseHero.magicAttacks) // Loop through the list of magic attacks
            {
                GameObject spellBtn = Instantiate(magicButton) as GameObject;
                TextMeshProUGUI spellBtnText = spellBtn.transform.Find("SpellText").gameObject.GetComponent<TextMeshProUGUI>();

                spellBtnText.text = spellAtk.name;
                spellBtn.name = "Spell Button: " + spellBtnText.text + ", " + herosToManage[0].name;
            
                AttackButton ATB = spellBtn.GetComponent<AttackButton>(); // Fetch the attack button script from the instantiated button
                ATB.magicAttackToPerform = spellAtk; // Set the magicBtn to perform to the spell attack, which are the spells a player has. Fetch the information from the heros to manage list
                spellBtn.transform.SetParent(magicSpacer, false);
                atkBtns.Add(spellBtn);
            }
        } else 
        {
            magicAttackButton.GetComponent<Button>().interactable = false;
        }
    }

    public void Input3() // Switching to magic attacks 
    {
        actionPanel.SetActive(false); // Physical or magical atk
        magicPanel.SetActive(true);
    }

    public void Input4(BaseAttack _chosenMagicSpell) // Choose magic attack
    {
        herosChoise.Attacker = herosToManage[0].name;
        herosChoise.attackersGobj = herosToManage[0];
        herosChoise.Type = "Hero";

        herosChoise.chosenAttack = _chosenMagicSpell;
        magicPanel.SetActive(false);
        enemySelectPanel.SetActive(true);
    }
}
