using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class WallGenerator
{

    public static void CreateWalls(HashSet<Vector2Int> _floorPos, TilemapVisualizer _tilemapVisualizer) 
    {
        var basicWallPosition = FindWallsInDirections(_floorPos, Direction2D.cardinalDirectionsList);
        foreach (var position in basicWallPosition)
        {
            _tilemapVisualizer.PaintWallTiles(position);
        }
    }

    private static HashSet<Vector2Int> FindWallsInDirections(HashSet<Vector2Int> _floorPos, List<Vector2Int> _directionList) 
    {
        HashSet<Vector2Int> walls = new();

        foreach (var position in _floorPos)
        {
            foreach (var direction in _directionList)
            {
                var wallPos = position + direction;
                if(!_floorPos.Contains(wallPos)) 
                {
                    walls.Add(wallPos);
                }
            }
        }

        return walls;
    }
}
