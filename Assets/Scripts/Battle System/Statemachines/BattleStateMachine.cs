using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
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
    [HideInInspector] public List<HandleTurn> performList = new();
    [HideInInspector] public List<GameObject> herosInBattle = new();
    [HideInInspector] public List<GameObject> enemiesInBattle = new();

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

    void Awake() 
    {
        for (int i = 0; i < GameManager.instance.enemyAmount; i++)
        {
            GameObject newEnemy = Instantiate(GameManager.instance.enemiesToBattle[i], spawnPoints[i].position, Quaternion.identity) as GameObject;
            newEnemy.name = newEnemy.GetComponent<EnemyStateMachine>().enemy.TheName + "_" + (i + 1);
            newEnemy.GetComponent<EnemyStateMachine>().enemy.TheName = newEnemy.name;
            enemiesInBattle.Add(newEnemy);
        }
    }

    void Start()
    {
        herosInBattle.AddRange(GameObject.FindGameObjectsWithTag("Hero").OrderBy(hero => hero.name)); 
        battleStates = BattleStates.WAIT;
        heroInput = HeroGUI.ACTIVATE;

        // PANELS
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
                        if(performList[0].attackersTarget == herosInBattle[i])
                        {
                            ESM.targetToAttack = performList[0].attackersTarget;
                            ESM.currentState = EnemyStateMachine.TurnState.ACTION; 
            
                        } else 
                        {
                            performList[0].attackersTarget = herosInBattle[Random.Range(0, herosInBattle.Count)];

                            ESM.targetToAttack = performList[0].attackersTarget;
                            ESM.currentState = EnemyStateMachine.TurnState.ACTION; 
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
                for (int i = 0; i < herosInBattle.Count; i++)
                {
                    herosInBattle[i].GetComponent<HeroStateMachine>().currentState = HeroStateMachine.TurnState.WAITING;

                    GameManager.instance.LoadSceneAfterBattle();
                    GameManager.instance.gameState = GameManager.GameStates.WORLDSTATE;
                    GameManager.instance.enemiesToBattle.Clear();
                }

            break;

            case(BattleStates.LOSE):
                // Do something
            break;
        }
        
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
        performList.Add(handleTurnScript_HeroChoise);
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
