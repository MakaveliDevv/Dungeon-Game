using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    [SerializeField] private int corridorLength = 14, corridorCount = 5;
    [SerializeField] [Range(.1f, 1f)] private float roomPercent = .8f;
    public SimpleRandomWalkSO roomGenerationParameters;
    protected override void RunProceduralGeneration()
    {
        CorridorGenerator();
    }

    private void CorridorGenerator()
    {
        HashSet<Vector2Int> floorPositions = new();

        CreateCorridors(floorPositions);

        tilemapVisualizer.PaintFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
    }

    private void CreateCorridors(HashSet<Vector2Int> floorPositions)
    {
        var currentPosition = startPosition;
        for (int i = 0; i < corridorCount; i++)
        {
            var corridor = ProceduralGenerationAlgorithms.RandomWalkCorridor(currentPosition, corridorLength);
            currentPosition = corridor[corridor.Count - 1];
            floorPositions.UnionWith(corridor);
        }
    }
}
