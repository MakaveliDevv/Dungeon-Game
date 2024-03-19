using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimpleRandomWalkDungeonGenerator : AbstractDungeonGenerator
{
    [SerializeField] protected Dungeon dungeon; // Scriptable object dungeon

    // THIS IS FOR THE SIMPLE DUNGEON GENERATION
    protected override void RunProceduralGeneration() 
    {
        // HashSet<Vector2Int> floorPositions = StartRandomWalk(randomWalkParameters, startPosition);
        // tilemapVisualizer.Clear();
        // tilemapVisualizer.PaintFloorTiles(floorPositions);
        // WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
    }

    protected HashSet<Vector2Int> StartRandomPath(Dungeon _dungeon, Vector2Int _position)
    {
        var currentPosition = _position;
        HashSet<Vector2Int> floorPositions = new();

        for (int i = 0; i < _dungeon.iterations; i++)
        {
            var path = ProceduralGenerationAlgorithms.RandomPathGeneration(currentPosition, _dungeon.walkRadius);
            floorPositions.UnionWith(path);

            // RANDOM ITERATION CHECK
            if(_dungeon.startRandomlyEachIteration) 
            {
                currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count));
            }
        }
        
        return floorPositions;
    }
}
