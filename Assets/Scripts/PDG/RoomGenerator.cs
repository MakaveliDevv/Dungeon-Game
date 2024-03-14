using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RoomGenerator : RandomWalkGenerator
{
    [SerializeField] private bool randomWalkGeneration;

    public float distanceBetweenRooms;
    public float brushSizeX, bruhsSizeY;

    protected override void RunProceduralGeneration() 
    {
        CreateRooms();
    }

    private void CreateRooms() 
    {
        var roomList = GeneratorAlgorithms.BinarySpacePartitioning
        (
            new BoundsInt((Vector3Int)startPosition, new Vector3Int(dungeon.width, dungeon.height, 0)), dungeon.minRoomWidth, dungeon.minRoomHeight
        );

        HashSet<Vector2Int> floor = new();

        if(randomWalkGeneration) 
        {
            // Create not simple rooms
            CreateRoomsRandomly(roomList);
        
        } else 
        {
            // Create simple rooms
            CreateSimpleRooms(roomList);
        }

        List<Vector2Int> roomCenters = new();
        foreach (var room in roomList)
        {
            Vector2Int center = (Vector2Int)Vector3Int.RoundToInt(room.center); // Get the center of the room in roomList
            roomCenters.Add(center);
        }

        HashSet<Vector2Int> corridors = ConnectRooms(roomCenters); // Connect the corridors with each room center
        floor.UnionWith(corridors);

        tilemapVisualizer.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, tilemapVisualizer);
    }

    private HashSet<Vector2Int> CreateRoomsRandomly(List<BoundsInt> _roomList) 
    {
        HashSet<Vector2Int> floor = new();

        for (int i = 0; i < _roomList.Count; i++)
        {
            var roomBounds = _roomList[i];
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y)); // Assign the center of the room
            var roomFloor = StartRandomGeneration(dungeon, roomCenter);

            foreach (var position in roomFloor)
            {
                if(position.x >= (roomBounds.xMin + dungeon.offset)
                && position.x <= (roomBounds.xMax - dungeon.offset)
                && position.y >= (roomBounds.yMin + dungeon.offset)
                && position.y <= (roomBounds.yMax - dungeon.offset)) 
                {
                    floor.Add(position);
                }
            }
        }

        return floor;
    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> _roomCenters) 
    {
        HashSet<Vector2Int> connectPath = new();

        Debug.Log("_roomCenters count: " + _roomCenters.Count);

        var currentRoomCenter = _roomCenters[Random.Range(0, _roomCenters.Count)];
        _roomCenters.Remove(currentRoomCenter);

        while(_roomCenters.Count > 0) 
        {
            Vector2Int closestPoint = FindClosestPointTo(currentRoomCenter, _roomCenters);
            _roomCenters.Remove(closestPoint);

            HashSet<Vector2Int> corridor = CreateCorridor(currentRoomCenter, closestPoint);
            List<Vector2Int> convCorridorList = new(corridor); // Convert from HasSet to List

            convCorridorList = IncreaseBrushSize(convCorridorList); // Apply increase brush method here
            HashSet<Vector2Int> revCorridorList = new(convCorridorList); // Convert back to HashSet

            connectPath.UnionWith(revCorridorList);
            currentRoomCenter = closestPoint; // Update the room center
        }

        return connectPath;
    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int _currentRoomCenter, Vector2Int _destination) 
    {
        HashSet<Vector2Int> floor = new();
        var position = _currentRoomCenter;
        floor.Add(position);


        while(position.y != _destination.y) 
        {
            if(_destination.y > position.y) // If the closest point is greater than the current room center position on the Y axis
            {
                position += Vector2Int.up;

            } else if(_destination.y < position.y) 
            {
                position += Vector2Int.down;
            }

            floor.Add(position);
        } 

        while(position.x != _destination.x) 
        {
            if(_destination.x > position.x) // If the closest point is greater than the current room center position on the X axis 
            {
                position += Vector2Int.right;
            
            } else if(_destination.x < position.x) {
                position += Vector2Int.left;

            }

            floor.Add(position);
        }    
        
        return floor;
    }

    private Vector2Int FindClosestPointTo(Vector2Int _currentRoomCenter, List<Vector2Int> _roomCenters) 
    {
        Vector2Int closestPoint = Vector2Int.zero;
        distanceBetweenRooms = float.MaxValue;

        foreach (var positionRoomCenter in _roomCenters)
        {
            float currentDistance = Vector2.Distance(positionRoomCenter, _currentRoomCenter);
            if(currentDistance < distanceBetweenRooms) 
            {
                distanceBetweenRooms = currentDistance;
                closestPoint = positionRoomCenter;
            }
        }

        return closestPoint;
    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> _roomList) 
    {
        HashSet<Vector2Int> floor = new();
        foreach (var room in _roomList)
        {
            // Loop for the columns
            for (int col = dungeon.offset; col < room.size.x - dungeon.offset; col++)
            {
                // Loop for the rows
                for (int row = dungeon.offset; row < room.size.y - dungeon.offset; row++)
                {   
                    Vector2Int roomPosition = (Vector2Int)room.min + new Vector2Int(col, row);
                    floor.Add(roomPosition);
                }
            }
        }
        return floor;
    }

    private List<Vector2Int> IncreaseBrushSize(List<Vector2Int> _target) 
    {
        List<Vector2Int> brushPath = new();

        for (int i = 0; i < _target.Count; i++)
        {
            for (int x = 0; x < brushSizeX; x++)
            {
                for (int y = 0; y < bruhsSizeY; y++)
                {
                    brushPath.Add(_target[i - 1] + new Vector2Int(x, y));
                }
            }
        }
        return brushPath;
    }
}
