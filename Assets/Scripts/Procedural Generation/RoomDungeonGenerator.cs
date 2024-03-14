using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    [SerializeField] private int minRoomWidth = 4, minRoomHeight = 4;
    [SerializeField] private int dungeonWidth = 20, dungeonHeight = 20;

    [SerializeField] [Range(0, 10)] private int offset = 1;
    [SerializeField] private bool randomWalkRooms = false;

    public float distanceBetweenRooms;
    public float brushSizeX, brushSizeY;


    protected override void RunProceduralGeneration()
    {
        CreateRooms();
    }

    private void CreateRooms()
    {
        var roomsList = ProceduralGenerationAlgorithms.BinarySpacePartitioning(new BoundsInt((Vector3Int)startPosition, new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth, minRoomHeight);
        HashSet<Vector2Int> floor = new();

        if (randomWalkRooms) floor = CreateRoomsRandomly(roomsList);
        else floor = CreateSimpleRooms(roomsList);

        List<Vector2Int> roomCenters = new();
        foreach (var _room in roomsList)
        {
            Vector2Int center = (Vector2Int)Vector3Int.RoundToInt(_room.center);
            roomCenters.Add(center);
        }

        HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
        floor.UnionWith(corridors);

        tilemapVisualizer.PaintFloorTiles(floor);
        WallGeneratorr.CreateWalls(floor, tilemapVisualizer);
    }

    private HashSet<Vector2Int> CreateRoomsRandomly(List<BoundsInt> _roomsList)
    {
        HashSet<Vector2Int> floor = new();
        for (int i = 0; i < _roomsList.Count; i++)
        {
            var roomBounds = _roomsList[i];
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            var roomFloor = StartRandomWalk(randomWalkParameters, roomCenter);

            foreach (var _position in roomFloor)
            {
                if(_position.x >= (roomBounds.xMin + offset) 
                && _position.x <= (roomBounds.xMax - offset) 
                && _position.y >= (roomBounds.yMin + offset) 
                && _position.y <= (roomBounds.yMax - offset)) 
                    floor.Add(_position);
            }
        }

        return floor;
    }

    // private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> _roomCenters)
    // {
    //     HashSet<Vector2Int> corridors = new();
    //     var currentRoomCenter = _roomCenters[Random.Range(0, _roomCenters.Count)];
    //     _roomCenters.Remove(currentRoomCenter);
        
    //     while(_roomCenters.Count > 0)
    //     {
    //         Vector2Int closestPoint = FindClosestPointTo(currentRoomCenter, _roomCenters);
    //         _roomCenters.Remove(closestPoint);

    //         HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closestPoint);
    //         currentRoomCenter = closestPoint;
    //         corridors.UnionWith(newCorridor);
    //     }

    //     return corridors;
    // }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> _roomCenters)
    {
        HashSet<Vector2Int> corridors = new();
        var currentRoomCenter = _roomCenters[Random.Range(0, _roomCenters.Count)];
        _roomCenters.Remove(currentRoomCenter);
        
        while(_roomCenters.Count > 0)
        {
            Vector2Int closestPoint = FindClosestPointTo(currentRoomCenter, _roomCenters);
            _roomCenters.Remove(closestPoint);

            HashSet<Vector2Int> corridor = CreateCorridor(currentRoomCenter, closestPoint);

            // Convert newCorridor from HashSet<Vector2Int> to List<Vector2Int>
            List<Vector2Int> newCorridorList = new(corridor);

            // Apply increase method here
            newCorridorList = IncreaseCorridorBrush(newCorridorList);

            // Convert newCorridorList back to HashSet<Vector2Int>
            HashSet<Vector2Int> expandedCorridor = new(newCorridorList);

            // Add the expanded corridor to the set of corridors
            corridors.UnionWith(expandedCorridor);

            currentRoomCenter = closestPoint;
        }

        return corridors;
    }




    private HashSet<Vector2Int> CreateCorridor(Vector2Int _currentRoomCenter, Vector2Int _destination)
    {
        HashSet<Vector2Int> corridor = new();
        var position = _currentRoomCenter;
        corridor.Add(position); 

        while(position.y != _destination.y) 
        {
            if(_destination.y > position.y) 
            {
                position += Vector2Int.up; 

            } else if(_destination.y < position.y) 
            {
                position += Vector2Int.down;
            }

            corridor.Add(position);
        }

        while(position.x != _destination.x) 
        {
            if(_destination.x > position.x) 
            {
                // MOVE RIGHT
                position += Vector2Int.right;

            } else if(_destination.x < position.x) 
            {
                // MOVE LEFT
                position += Vector2Int.left;
            }

            corridor.Add(position);
        }

        return corridor;
    }


    private Vector2Int FindClosestPointTo(Vector2Int _currentRoomCenter, List<Vector2Int> _roomCenters)
    {
        Vector2Int closestPoint = Vector2Int.zero;
        distanceBetweenRooms = float.MaxValue;

        foreach (var _position in _roomCenters)
        {
            float currentDistance = Vector2.Distance(_position, _currentRoomCenter);
            if(currentDistance < distanceBetweenRooms) 
            {
                // Find closest point
                distanceBetweenRooms = currentDistance;
                closestPoint = _position;
            }
        }

        return closestPoint;
    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> _roomsList)
    {
        HashSet<Vector2Int> floor = new();
        foreach (var _room in _roomsList)
        {
            for (int col = offset; col < _room.size.x - offset ; col++)
            {
                for (int row = offset; row < _room.size.y - offset ; row++)
                {
                    Vector2Int position = (Vector2Int)_room.min + new Vector2Int(col, row);
                    floor.Add(position);
                }
            }
        }

        return floor;
    }


    private List<Vector2Int> IncreaseCorridorBrush(List<Vector2Int> _corridor)
    {
        List<Vector2Int> newCorridor = new();

        for (int i = 1; i < _corridor.Count; i++)
        {
            for (int x = -1; x < brushSizeX; x++)
            {
                for (int y = -1; y < brushSizeY; y++)
                {
                    newCorridor.Add(_corridor[i - 1] + new Vector2Int(x, y));
                }
            }            
        }

        return newCorridor;
    }
}
