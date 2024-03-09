using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProceduralGenerationAlgorithms
{
    // METHOD TO GENERATE A RANDOM PATH
    public static HashSet<Vector2Int> SimpleRandomWalk(Vector2Int startPosition, int walkLength) 
    {
        HashSet<Vector2Int> path = new()
        {
            startPosition
        };
        
        var previousPosition = startPosition;

        for (int i = 0; i < walkLength; i++)
        {
            var newPosition = previousPosition + Direction2D.GetRandomCardinalDirection();
            path.Add(newPosition);

            previousPosition = newPosition;
        }

        return path;
    }

    public static List<Vector2Int> RandomWalkCorridor(Vector2Int startPosition, int corridorLength) 
    {
        List<Vector2Int> corridor = new();

        var direction = Direction2D.GetRandomCardinalDirection();
        var currentPosition = startPosition;
        corridor.Add(currentPosition); // This is actually the start position
        
        for (int i = 0; i < corridorLength; i++)
        {
            currentPosition += direction;
            corridor.Add(currentPosition);
        }

        return corridor;
    }
}

public static class Direction2D
{
    // METHOD FOR THE DIRECTIONS
    public static List<Vector2Int> cardinalDirectionsList = new()
    {
        new Vector2Int(0, 1), // UP 
        new Vector2Int(0, -1), // DOWN
        new Vector2Int(1, 0), // RIGHT
        new Vector2Int(-1, 0), // LEFT
    };

    public static Vector2Int GetRandomCardinalDirection() 
    {
        return cardinalDirectionsList[Random.Range(0, cardinalDirectionsList.Count)];
    }
}
