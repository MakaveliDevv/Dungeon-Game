using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class AbstractDungeonGenerator : MonoBehaviour
{
    [Header("Tilemap Visualization")]
    [SerializeField] protected TilemapVisualizer tilemapVisualizer = null;
    [SerializeField] protected Vector2Int startPosition = Vector2Int.zero;

    [Header("Dungeon Stuff")]
    [SerializeField] protected Dungeon dungeon; // Scriptable object dungeon
    [SerializeField] protected List<BoundsInt> roomsList;
    [SerializeField] protected bool randomPathGeneration = false;
    protected float distanceBetweenRooms;
    [SerializeField] protected float brushSizeX, brushSizeY;

    [Header("Spawnpoints")]
    [SerializeField] protected List<Vector2Int> spawnPoints_List = new(); // List to double check amount of spawn points
    [SerializeField] protected List<GameObject> gameObject_SpawnPoints_List = new(); // List to for the gameobjects spawnpoints
    [SerializeField] protected GameObject spawnPointPrefab;
    [SerializeField] protected int maxSpawnPoints = 10;
    [SerializeField] protected int spawnPointAmount;
    [SerializeField] protected Vector2Int spawnPoint;



    [Header("Enemy Stuff")]
    [SerializeField] protected List<GameObject> enemiesType = new();
    protected List<GameObject> enemies = new();
    [SerializeField] protected float enemySpawnRadius = 5f;
    [SerializeField] protected int enemiesAmount;
    [SerializeField] protected float checkRadius = 5f; 

    [Header("Player stuff")]
    [SerializeField] protected GameObject playerPrefab;
    [SerializeField] protected List<GameObject> playerList = new();

    public void GenerateDungeon() 
    {
        tilemapVisualizer.Clear();
        DestroySpawnPointsInScene();

        spawnPoints_List.Clear();
        gameObject_SpawnPoints_List.Clear();

        DestroyPlayerInScene();
        playerList.Clear();

        DestroyEnemiesInScene();
        enemies.Clear();

        RunProceduralGeneration();
    }

    protected abstract void RunProceduralGeneration();

    private void DestroyEnemiesInScene()
    {
        // Find all enemy GameObjects in the scene and destroy them
        foreach (GameObject enemy in enemies)
        {
            DestroyImmediate(enemy);
        }
    }

    private void DestroyPlayerInScene() 
    {
        foreach (GameObject player in playerList)
        {
            DestroyImmediate(player);
        }
    }

    private void DestroySpawnPointsInScene() 
    {
        foreach (GameObject point in gameObject_SpawnPoints_List)
        {
            DestroyImmediate(point);
        }
    }
    
}
