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

        if(!GameObject.Find("HeroCharacter")) 
        {
            GameObject hero = Instantiate(heroCharacter, nextHeroPosition, Quaternion.identity) as GameObject;
            hero.name = "HeroCharacter";
        }
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

                // Go to idle state
                gameState = GameStates.IDLESTATE;

            break;

            case(GameStates.IDLESTATE):

            break;
            // default:
        }
    }

    public void LoadNextScene() 
    {
        SceneManager.LoadScene(SceneToLoad);
    }

    public void LoadSceneAfterBattle() 
    {
        SceneManager.LoadScene(LastScene);
    }

    private void RandomEncounter() 
    {
        if(isWalking && canGetEncounter) 
        {
            // Create random encounter number
            if(Random.Range(0, 2000) < 10) 
            {
                Debug.Log("I got attacked");
                gotAttacked = true;
            }
        }
    }

    private void StartBattle() 
    {
        enemyAmount = Random.Range(1, curRegion.maxEnemiesEncounter + 1);

        // Which enemies
        for (int i = 0; i < enemyAmount; i++)
        {
            enemiesToBattle.Add(curRegion.possibleEnemies[Random.Range(0, curRegion.possibleEnemies.Count)]);
        }

        // Hero
        lastHeroPosition = GameObject.Find("HeroCharacter").transform.position; // Save hero position for when leaving the battle
        LastScene = SceneManager.GetActiveScene().name;

        // Load level
        SceneManager.LoadScene(curRegion.BattleScene);

        // Reset hero
        isWalking = false;
        gotAttacked = false;
        canGetEncounter = false;
    }
}
