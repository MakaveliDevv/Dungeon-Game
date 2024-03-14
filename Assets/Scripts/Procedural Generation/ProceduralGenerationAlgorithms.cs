using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProceduralGenerationAlgorithms
{
    // METHOD TO GENERATE A RANDOM PATH
    public static HashSet<Vector2Int> SimpleRandomWalk(Vector2Int _startPosition, int _walkLength) 
    {
        HashSet<Vector2Int> path = new()
        {
            _startPosition
        };
        
        var previousPosition = _startPosition;

        for (int i = 0; i < _walkLength; i++)
        {
            var newPosition = previousPosition + Directionss2D.GGetRandomCardinalDirection();
            path.Add(newPosition);

            previousPosition = newPosition;
        }

        return path;
    }

    // METHOD TO GENERATE A CORRIDOR
    public static List<Vector2Int> RandomWalkCorridor(Vector2Int _startPosition, int _corridorLength) 
    {
        List<Vector2Int> corridor = new();

        var direction = Directionss2D.GGetRandomCardinalDirection();
        var currentPosition = _startPosition;
        corridor.Add(currentPosition); // This is actually the start position
        
        for (int i = 0; i < _corridorLength; i++)
        {
            currentPosition += direction;
            corridor.Add(currentPosition);
        }

        return corridor;
    }

    // ALGORITHM TO GENERATE SPLITTED ROOMS IN AN AREA
    public static List<BoundsInt> BinarySpacePartitioning(BoundsInt _spaceToSplit, int _minWidth, int _minHeight) 
    {
        Queue<BoundsInt> roomsQueue = new();
        List<BoundsInt> roomsList = new();

        roomsQueue.Enqueue(_spaceToSplit);

        while(roomsQueue.Count > 0) 
        {
            var room = roomsQueue.Dequeue();
            if(room.size.x >= _minWidth && room.size.y >= _minHeight) 
            {
                if(Random.value < .5f) 
                {
                    if(room.size.y >= _minHeight * 2) 
                        SplitHorizontally(_minHeight, roomsQueue, room);

                    else if(room.size.x >= _minWidth * 2) 
                        SplitVertically(_minWidth, roomsQueue, room);

                    else
                    roomsList.Add(room);                    
                    
                } else 
                {
                    if(room.size.x >= _minWidth * 2) 
                        SplitVertically(_minWidth, roomsQueue, room);
 
                    else if(room.size.y >= _minHeight * 2) 
                        SplitHorizontally(_minHeight, roomsQueue, room);
                    
                    else
                    roomsList.Add(room);    
                }
            }
        }

        return roomsList;
    }

    private static void SplitVertically(int _minWidth, Queue<BoundsInt> _roomsQueue, BoundsInt _room)
    {
        var xSplit = Random.Range(1, _room.size.x);
        BoundsInt room1 = new(_room.min, new Vector3Int(xSplit, _room.size.y, _room.size.z));
        BoundsInt room2 = new(new Vector3Int(_room.min.x + xSplit, _room.min.y, _room.min.z), new Vector3Int(_room.size.x - xSplit, _room.size.y, _room.size.z));

        _roomsQueue.Enqueue(room1);
        _roomsQueue.Enqueue(room2);
    }

    private static void SplitHorizontally(int _minHeight, Queue<BoundsInt> _roomsQueue, BoundsInt _room)
    {
        var ySplit = Random.Range(1, _room.size.y); // minHeight, room.size.y - minHeight
        BoundsInt room1 = new(_room.min, new Vector3Int(_room.size.x, ySplit, _room.size.z));
        BoundsInt room2 = new(new Vector3Int(_room.min.x, _room.min.y + ySplit, _room.min.z), new Vector3Int(_room.size.x, _room.size.y - ySplit, _room.size.z));

        _roomsQueue.Enqueue(room1);
        _roomsQueue.Enqueue(room2);
    }
}

public static class Directionss2D
{
    // METHOD FOR THE DIRECTIONS
    public static List<Vector2Int> cardinalDirectionsList = new()
    {
        new Vector2Int(0, 1), // UP 
        new Vector2Int(0, -1), // DOWN
        new Vector2Int(1, 0), // RIGHT
        new Vector2Int(-1, 0), // LEFT
    };

    public static Vector2Int GGetRandomCardinalDirection() 
    {
        return cardinalDirectionsList[Random.Range(0, cardinalDirectionsList.Count)];
    }
}

