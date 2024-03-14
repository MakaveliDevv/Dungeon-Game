using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GeneratorAlgorithms : MonoBehaviour
{
    public static HashSet<Vector2Int> RandomPathGeneration(Vector2Int _startPos, int _walkRadius) 
    {
        // Add the start position, which is zero, into the list as a first point
        HashSet<Vector2Int> path = new() 
        {
            _startPos
        };

        var previousPos = _startPos; // Assign the start position to the previous position

        for (int i = 0; i < _walkRadius; i++)
        {
            var newPos = previousPos + Direction2D.GetRandomCardinalDirection(); // Vector2.zero + random cardinal direction
            path.Add(newPos);

            previousPos = newPos; // Assign the previous postion to the new position so the position gets updated
        }

        return path;
    }


    // THIS METHOD GENERATE SPLITTED ROOMS BASED ON THE SIZE OF THE DUNGEON AND OF THE ROOMS
    public static List<BoundsInt> BinarySpacePartitioning(BoundsInt _spaceToSplit, int _minWidthRoom, int _minHeightRoom) 
    {
        Queue<BoundsInt> roomQueue = new();
        List<BoundsInt> roomList = new();

        roomQueue.Enqueue(_spaceToSplit); // Add the space to split into the queue

        if(roomQueue.Count > 0) // Check if the queue containts more than 1 element
        {
            var room = roomQueue.Dequeue(); // Remove the first element (_spaceToSplit) and assign it to a variable as a room

            if(room.size.x >= _minWidthRoom && room.size.y >= _minHeightRoom) // Check if size of room is equal to greater than the minimal width and height 
            {
                if(Random.value < .5f) 
                {
                    // If the value is less than .5f, split the room first horizontally then vertically
                    if(room.size.y >= _minHeightRoom * 2) 
                    {
                        // Split horizontally
                        SplitHorizontally(_minHeightRoom, roomQueue, room);

                    } else if (room.size.x >= _minWidthRoom * 2) 
                    {
                        // Split vertically
                        SplitVertically(_minHeightRoom, roomQueue, room);

                    } else 
                    {
                        roomList.Add(room); // If it's not greater than the min heigh and width, then it's already generated correctly, so add it to the list

                    }
                } else // If the value is greater than .5f, split the room first vertically then horizontally 
                {
                    if(room.size.x >= _minWidthRoom * 2) 
                    {
                        // Split vertically
                        SplitVertically(_minHeightRoom, roomQueue, room);

                    } else if(room.size.y >= _minHeightRoom * 2) 
                    {
                        // Split horizontally
                        SplitHorizontally(_minHeightRoom, roomQueue, room);

                    } else 
                    {
                        roomList.Add(room);
                    }
                }
            }
        }

        return roomList;
    }

    private static void SplitVertically(int _minWidth, Queue<BoundsInt> _roomQueue, BoundsInt _room) 
    {
        var xSplit = Random.Range(1, _room.size.x);
        BoundsInt room1 = new(_room.min, new Vector3Int(xSplit, _room.size.y, _room.size.x));
        BoundsInt room2 = new(new Vector3Int(_room.min.x + xSplit, _room.min.y, _room.min.z), new Vector3Int(_room.size.x - xSplit, _room.size.x, _room.size.y));

        _roomQueue.Enqueue(room1);
        _roomQueue.Enqueue(room2);
    }

    private static void SplitHorizontally(int _minHeight, Queue<BoundsInt> _roomQueue, BoundsInt _room) 
    {
        var Ysplit = Random.Range(1, _room.size.y);
        BoundsInt room1 = new(_room.min, new Vector3Int(_room.size.x, Ysplit, _room.size.z));
        BoundsInt room2 = new(new Vector3Int(_room.min.x, _room.min.y + Ysplit, _room.min.z), new Vector3Int(_room.size.x, _room.size.y - Ysplit, _room.size.z));

        _roomQueue.Enqueue(room1);
        _roomQueue.Enqueue(room2);
    }
}


public static class Direction2D 
{
    public static List<Vector2Int> cardinalDirectionsList = new() 
    {
        new(0, 1), // UP
        new(1, 0), // RIGHT
        new(0, -1), // DOWN
        new(-1, 0) // LEFT
    };

    public static Vector2Int GetRandomCardinalDirection() 
    {
        return cardinalDirectionsList[Random.Range(0, cardinalDirectionsList.Count)];
    }
}
