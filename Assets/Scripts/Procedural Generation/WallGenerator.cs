using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class WallGenerator
{
    public static void CreateWalls(HashSet<Vector2Int> floorPositions, TilemapVisualizer tilemapVisualizer) 
    {
        var basicWallPosition = FindWallsInDirections(floorPositions, Direction2D.cardinalDirectionsList);
        foreach (var _position in basicWallPosition)
        {
            tilemapVisualizer.PaintSingleBasicWall(_position);   
        }
    }

    private static HashSet<Vector2Int> FindWallsInDirections(HashSet<Vector2Int> floorPositions, List<Vector2Int> directionList)
    {
        HashSet<Vector2Int> wallPositions = new();
        foreach (var _position in floorPositions)
        {
            foreach (var _direction in directionList)
            {
                var neighbourPosition = _position + _direction;
                if(!floorPositions.Contains(neighbourPosition)) 
                {
                    wallPositions.Add(neighbourPosition);
                }
            }
        }

        return wallPositions;
    }
}
