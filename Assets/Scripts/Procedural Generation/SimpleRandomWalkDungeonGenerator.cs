using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimpleRandomWalkDungeonGenerator : AbstractDungeonGenerator
{
    [SerializeField] protected SimpleRandomWalkSO randomWalkParameters; // Scriptable object dungeon

    protected override void RunProceduralGeneration() 
    {
        HashSet<Vector2Int> floorPositions = StartRandomWalk(randomWalkParameters, startPosition);
        tilemapVisualizer.Clear();
        tilemapVisualizer.PaintFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
    }

    protected HashSet<Vector2Int> StartRandomWalk(SimpleRandomWalkSO _parameters, Vector2Int _position)
    {
        var currentPosition = _position;
        HashSet<Vector2Int> floorPositions = new();

        for (int i = 0; i < _parameters.iterations; i++)
        {
            var path = ProceduralGenerationAlgorithms.SimpleRandomWalk(currentPosition, _parameters.walkLength);
            floorPositions.UnionWith(path);

            // RANDOM ITERATION CHECK
            if(_parameters.startRandomlyEachIteration) 
            {
                currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count));
            }
        }
        
        return floorPositions;
    }
}
