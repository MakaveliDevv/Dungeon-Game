using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public RegionData curRegion;

    // Enum
    public enum GameStates 
    {
        WORLDSTATE,
        TOWNSTATE,
        BATTLESTATE,
        IDLESTATE
    }

    public GameStates gameState;

    // Hero
    public GameObject heroCharacter;

    // Positions
    public Vector2 nextHeroPosition;
    public Vector2 lastHeroPosition; // Battle

    // Scenes
    public string SceneToLoad;
    public string LastScene; // Battle

    // Bool
    public bool isWalking = false, canGetEncounter = false, gotAttacked = false;

    // Battle
    public List<GameObject> enemiesToBattle = new();
    public int enemyAmount;

    // Spawnpoint
    public string NextSpawnPoint;


    // List for the encounterd enemies
    public List<string> encounterdEnemy = new();
    
    public bool battleWon;

    void Awake()
    {
        // Check if instance exist
        if(instance == null) 
        {
            // If not, set the instance to this
            instance = this;

        } else if(instance != null) // If exist but is not this instance 
        {
            // Destroy it
            Destroy(gameObject);
        }
        

        // Set this to be not destroyable
        DontDestroyOnLoad(gameObject);

        // if(!GameObject.Find("HeroCharacter")) 
        // {
        //     GameObject hero = Instantiate(heroCharacter, nextHersioPotion, Quaternion.identity) as GameObject;
        //     hero.name = "HeroCharacter";
        // }
    }

    void Update() 
    {
        switch (gameState)
        {
            case(GameStates.WORLDSTATE):
                if(isWalking) 
                {
                    RandomEncounter();
                }

                if(gotAttacked) 
                {
                    gameState = GameStates.BATTLESTATE;
                }

            break;

            case(GameStates.TOWNSTATE):

            break; 

            case(GameStates.BATTLESTATE):
                // Load battle scene
                StartBattle();
                // SceneManager.LoadScene(curRegion.BattleScene);

                // Go to idle state
                gameState = GameStates.IDLESTATE;

            break;

            case(GameStates.IDLESTATE):

            break;
            // default:
        }

        // if(battleWon) 
        // {
        //     Debug.Log(battleWon);
        //     StartCoroutine(WaitBeforeFlagToFalse());
        //     // foreach (GameObject enemy in encounterdEnemy)
        //     // {
        //     //     // Destroy(enemy);
        //     //     // encounterdEnemy.Clear();
        //     //     Debug.Log(enemy.name + ": Dungeon Enemy Succesfully Destroyed");
        //     // }
        // } 
    }

    // public void LoadNextScene() 
    // {
    //     SceneManager.LoadScene(SceneToLoad);
    // }

    IEnumerator WaitBeforeFlagToFalse() 
    {
        yield return new WaitForSeconds(1f);
        battleWon = false;
    }

    public bool LoadSceneAfterBattle() 
    {
        if(battleWon) 
        {
            SceneManager.LoadScene(LastScene);
            return true;
        }

        return false;
    }

    private void RandomEncounter() 
    {
        if(isWalking && canGetEncounter) 
        {
            Debug.Log("I got attacked");
            gotAttacked = true;
        }
    }

    private void StartBattle() 
    {
        enemyAmount = Random.Range(1, curRegion.maxEnemiesEncounter + 1);

        // Which enemy
        for (int i = 0; i < enemyAmount; i++)
        {
            enemiesToBattle.Add(curRegion.possibleEnemies[Random.Range(0, curRegion.possibleEnemies.Count)]); // Get the enemy from the encounter
        }

        // Hero
        lastHeroPosition = GameObject.Find("HeroCharacter").transform.position; // Save hero position for when leaving the battle
        LastScene = SceneManager.GetActiveScene().name;

        // Load level
        SceneManager.LoadScene(curRegion.Scene);

        // Reset hero
        isWalking = false;
        gotAttacked = false;
        canGetEncounter = false;
    }
}
