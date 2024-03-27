using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class WallGeneratorr
{
    // public static void CreateWalls(HashSet<Vector2Int> floorPositions, TilemapVisualizer tilemapVisualizer) 
    // {
    //     var basicWallPosition = FindWallsInDirections(floorPositions, Directionss2D.cardinalDirectionsList);
    //     foreach (var position in basicWallPosition)
    //     {
    //         tilemapVisualizer.PaintSingleBasicWall(position);   
    //     }
    // }

    public static HashSet<Vector2Int> CreateWalls(HashSet<Vector2Int> floorPositions, TilemapVisualizer tilemapVisualizer)
    {
        List<Vector2Int> directionList = Directionss2D.cardinalDirectionsList; // Assuming directions are hardcoded
        HashSet<Vector2Int> wallPositions = FindWallsInDirections(floorPositions, directionList);

        foreach (var position in wallPositions)
        {
            tilemapVisualizer.PaintSingleBasicWall(position);   
        }

        return wallPositions;
    }

    public static HashSet<Vector2Int> FindWallsInDirections(HashSet<Vector2Int> floorPositions, List<Vector2Int> directionList)
    {
        HashSet<Vector2Int> wallPositions = new();
        foreach (var position in floorPositions)
        {
            foreach (var direction in directionList)
            {
                var neighbourPosition = position + direction;
                if(!floorPositions.Contains(neighbourPosition)) 
                {
                    wallPositions.Add(neighbourPosition);
                }
            }
        }

        return wallPositions;
    }
}
