using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProceduralGenerationAlgorithms
{
    // METHOD TO GENERATE A RANDOM PATH
    public static HashSet<Vector2Int> RandomPathGeneration(Vector2Int _startPos, int _walkRadius) 
    {
        HashSet<Vector2Int> path = new()
        {
            _startPos
        };
        
        var previousPos = _startPos;

        for (int i = 0; i < _walkRadius; i++)
        {
            var newPos = previousPos + Directionss2D.GGetRandomCardinalDirection();
            path.Add(newPos);

            previousPos = newPos;
        }

        return path;
    }

    // ALGORITHM TO GENERATE SPLITTED ROOMS IN AN AREA
    public static List<BoundsInt> BinarySpacePartitioning(BoundsInt _spaceToSplit, int _minWidthRoom, int _minHeightRoom) 
    {
        Queue<BoundsInt> roomsQueue = new();
        List<BoundsInt> roomsList = new();

        roomsQueue.Enqueue(_spaceToSplit); // Add the space to split into the queue

        while(roomsQueue.Count > 0) // Check if the queue containts more than 1 element
        {
            var room = roomsQueue.Dequeue();
            if(room.size.x >= _minWidthRoom && room.size.y >= _minHeightRoom) // Check if size of room is equal to greater than the minimal width and height
            {
                if(Random.value < .5f) 
                {
                    if(room.size.y >= _minHeightRoom * 2) 
                        SplitHorizontally(_minHeightRoom, roomsQueue, room);

                    else if(room.size.x >= _minWidthRoom * 2) 
                        SplitVertically(_minWidthRoom, roomsQueue, room);

                    else
                    roomsList.Add(room);                    
                    
                } else 
                {
                    if(room.size.x >= _minWidthRoom * 2) 
                        SplitVertically(_minWidthRoom, roomsQueue, room);
 
                    else if(room.size.y >= _minHeightRoom * 2) 
                        SplitHorizontally(_minHeightRoom, roomsQueue, room);
                    
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

