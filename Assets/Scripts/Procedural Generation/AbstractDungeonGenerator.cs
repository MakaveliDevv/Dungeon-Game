using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class AbstractDungeonGenerator : MonoBehaviour
{
    [SerializeField] protected TilemapVisualizer tilemapVisualizer = null;
    [SerializeField] protected Vector2Int startPosition = Vector2Int.zero;

    [SerializeField] protected List<BoundsInt> roomsList;
    [SerializeField] protected List<GameObject> enemiesType = new();
    [SerializeField] protected List<GameObject> enemies = new();
    [SerializeField] protected float enemySpawnRadius = 5f;
    [SerializeField] protected float checkRadius = 5f; 
    [SerializeField] protected int maxSpawnPoints = 10;
    public int enemiesAmount;


    [SerializeField] protected bool randomPathGeneration = false;
    public float distanceBetweenRooms;
    public float brushSizeX, brushSizeY;


    public void GenerateDungeon() 
    {
        tilemapVisualizer.Clear();
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
    
}
