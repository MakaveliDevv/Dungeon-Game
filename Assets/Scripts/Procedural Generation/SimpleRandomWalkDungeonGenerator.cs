using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimpleRandomWalkDungeonGenerator : AbstractDungeonGenerator
{
    [SerializeField] private SimpleRandomWalkSO randomWalkParameters; // Refers to a dungeon

    protected override void RunProceduralGeneration() 
    {
        HashSet<Vector2Int> floorPositions = StartRandomWalk(randomWalkParameters);
        tilemapVisualizer.Clear();
        tilemapVisualizer.PaintFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
    }

    protected HashSet<Vector2Int> StartRandomWalk(SimpleRandomWalkSO parameters)
    {
        var currentPosition = startPosition; // Vector2.zero
        HashSet<Vector2Int> floorPositions = new();

        for (int i = 0; i < parameters.iterations; i++)
        {
            var path = ProceduralGenerationAlgorithms.SimpleRandomWalk(currentPosition, parameters.walkLength);
            floorPositions.UnionWith(path);

            // RANDOM ITERATION CHECK
            if(parameters.startRandomlyEachIteration) 
            {
                currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count));
            }
        }
        
        return floorPositions;
    }
}
