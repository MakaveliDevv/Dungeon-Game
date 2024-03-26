using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomDungeonGenerator : SimpleRandomWalkDungeonGenerator
{ 
    protected override void RunProceduralGeneration()
    {
        CreateRooms();
    }

    // private void CreateRooms()
    // {
    //     roomsList = ProceduralGenerationAlgorithms.BinarySpacePartitioning
    //     (
    //         new BoundsInt((Vector3Int)startPosition, new Vector3Int(dungeon.width, dungeon.height, 0)), dungeon.minRoomWidth, dungeon.minRoomHeight
    //     );
        
    //     HashSet<Vector2Int> floor = new();

    //     if (randomPathGeneration) floor = CreateRoomsRandomly(roomsList);
    //     else floor = CreateSimpleRooms(roomsList);

    //     List<Vector2Int> roomCenters = new();
    //     foreach (var _room in roomsList)
    //     {
    //         Vector2Int center = (Vector2Int)Vector3Int.RoundToInt(_room.center);
    //         roomCenters.Add(center);
    //     }

    //     HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
    //     floor.UnionWith(corridors);

    //     tilemapVisualizer.PaintFloorTiles(floor);
    //     WallGeneratorr.CreateWalls(floor, tilemapVisualizer);
    // }

    private void CreateRooms()
    {
        roomsList = ProceduralGenerationAlgorithms.BinarySpacePartitioning(
            new BoundsInt((Vector3Int)startPosition, new Vector3Int(dungeon.width, dungeon.height, 0)), dungeon.minRoomWidth, dungeon.minRoomHeight
        );
        
        HashSet<Vector2Int> floor = new();

        if (randomPathGeneration) 
            floor = CreateRoomsRandomly(roomsList);
        else 
            floor = CreateSimpleRooms(roomsList);

        // Generate spawn points within the floor positions
        HashSet<Vector2Int> floorSpawnPoints = GenerateSpawnPoints(floor);

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

        // Visualize spawn points using TilemapVisualizer
        tilemapVisualizer.PaintSpawnPoints(floorSpawnPoints);

        // Spawn enemies around spawn points
        SpawnEnemiesAroundSpawnPoints(floorSpawnPoints, enemySpawnRadius);
    }

    private HashSet<Vector2Int> GenerateSpawnPoints(HashSet<Vector2Int> floor)
    {
        HashSet<Vector2Int> floorSpawnPoints = new();

        // Generate spawn points randomly within the floor positions
        int randomMaxSpawnPoints = Mathf.Min(floor.Count, maxSpawnPoints); // Limit to 10 spawn points

        for (int i = 0; i < randomMaxSpawnPoints; i++)
        {
            // Generate a candidate spawn point
            Vector2Int spawnPoint = floor.ElementAt(Random.Range(0, floor.Count));

            // Check if the candidate spawn point is too close to a wall
            bool tooCloseToWall = IsTooCloseToWall(spawnPoint, floorSpawnPoints);

            // If the candidate spawn point is not too close to a wall, add it to the list of spawn points
            if (!tooCloseToWall)
            {
                floorSpawnPoints.Add(spawnPoint);
            }
        }

        return floorSpawnPoints;
    }

    private bool IsTooCloseToWall(Vector2Int position, HashSet<Vector2Int> floor)
    {
        float minDistanceToWallSquared = 36f; // Minimum squared distance to be considered too close to a wall

        foreach (var wallPosition in floor)
        {
            // Calculate the squared distance between the spawn point and the wall position
            float distanceSquared = (position - wallPosition).sqrMagnitude;

            // If the squared distance is less than the minimum allowed, the spawn point is too close to a wall
            if (distanceSquared < minDistanceToWallSquared)
            {
                return true;
            }
        }

        return false;
    }

    private HashSet<Vector2Int> CreateRoomsRandomly(List<BoundsInt> _roomsList)
    {
        HashSet<Vector2Int> floor = new();
        HashSet<Vector2Int> floorSpawnPoints = new();
        
        for (int i = 0; i < _roomsList.Count; i++)
        {
            var roomBounds = _roomsList[i];
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            var roomFloor = StartRandomPath(dungeon, roomCenter);

            foreach (var _position in roomFloor)
            {
                if ((_position.x >= (roomBounds.xMin + dungeon.offset) && _position.x <= (roomBounds.xMax - dungeon.offset)
                    && _position.y >= (roomBounds.yMin + dungeon.offset) && _position.y <= (roomBounds.yMax - dungeon.offset)))
                    floor.Add(_position);
            }
        }

        // Debug.Log($"Total rooms: {_roomsList.Count}, Total spawn points generated: {floorSpawnPoints.Count}");

        // Visualize spawn points using TilemapVisualizer
        tilemapVisualizer.PaintSpawnPoints(floorSpawnPoints);

        return floor;
    }

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
            newCorridorList = IncreaseBrushSize(newCorridorList);

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
            for (int col = dungeon.offset; col < _room.size.x - dungeon.offset ; col++)
            {
                for (int row = dungeon.offset; row < _room.size.y - dungeon.offset ; row++)
                {
                    Vector2Int position = (Vector2Int)_room.min + new Vector2Int(col, row);
                    floor.Add(position);
                }
            }
        }

        return floor;
    }

    
    private void SpawnEnemiesAroundSpawnPoints(HashSet<Vector2Int> floorSpawnPoints, float radius)
    {
        foreach (var spawnPoint in floorSpawnPoints)
        {
            int numEnemies = Random.Range(1, 4); 

            for (int i = 0; i < numEnemies; i++)
            {
                float angle = Random.Range(0f, Mathf.PI * 2f);

                Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * Random.Range(0f, radius);
                Vector2Int enemySpawnPosition = new((int)(spawnPoint.x + offset.x), (int)(spawnPoint.y + offset.y));

                bool canSpawn = CanSpawnEnemyAt(enemySpawnPosition);
                if (canSpawn)
                {
                    SpawnEnemyAt(enemySpawnPosition);
                }
            }
        }
    }
    
    private void SpawnEnemyAt(Vector2Int position)
    {
        // Get amount of enemies
        float amount = Random.Range(0, enemiesAmount + 1);

        // Get type of enemies
        int typeIndex = Random.Range(0, enemiesType.Count); 

        GameObject enemyPrefab = enemiesType[typeIndex];

        Vector3 spawnPosition = new(position.x, position.y, 0f);


        // Spawn the enemies at the position
        for (int i = 0; i < amount; i++)
        {
            // Instantiate the enemy prefab at the position
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity) as GameObject;
            enemies.Add(newEnemy);
        }

        Debug.Log($"Spawned {amount} of {enemyPrefab.name} at position {position}");
    }

    private bool CanSpawnEnemyAt(Vector2Int position)
    {
        foreach (var enemy in enemies)
        {
            Vector2 enemyPosition = new(enemy.transform.position.x, enemy.transform.position.y);
            float distance = Vector2.Distance(position, enemyPosition);

            if (distance < checkRadius)
            {
                return false;
            }
        }

        return true;
    }

    // private void SpawnEnemiesAroundSpawnPoints(HashSet<Vector2Int> floorSpawnPoints, float radius)
    // {
    //     foreach (var spawnPoint in floorSpawnPoints)
    //     {
    //         // Generate a random number of enemies to spawn around this spawn point
    //         int numEnemies = Random.Range(1, 4); // Change the range as needed

    //         for (int i = 0; i < numEnemies; i++)
    //         {
    //             // Generate a random angle to determine the direction
    //             float angle = Random.Range(0f, Mathf.PI * 2f);

    //             // Calculate random position within the specified radius
    //             Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * Random.Range(0f, radius);
    //             Vector2Int enemySpawnPosition = new((int)(spawnPoint.x + offset.x), (int)(spawnPoint.y + offset.y));

    //             // Spawn your enemy at the calculated position
    //             SpawnEnemyAt(enemySpawnPosition); // You need to implement this method to actually spawn the enemy
    //         }
    //     }
    // }

    
    private List<Vector2Int> IncreaseBrushSize(List<Vector2Int> _corridor)
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

    private void OnDrawGizmos()
    {
        // Draw room bounds for each room in the dungeon
        foreach (var roomBounds in roomsList)
        {
            DrawRoomBounds(roomBounds);
        }
    }

    private void DrawRoomBounds(BoundsInt roomBounds)
    {
        Vector3Int min = roomBounds.min;
        Vector3Int max = roomBounds.max;

        Vector3 topLeft = new(min.x, min.y, 0);
        Vector3 topRight = new(max.x, min.y, 0);
        Vector3 bottomRight = new(max.x, max.y, 0);
        Vector3 bottomLeft = new(min.x, max.y, 0);

        // Draw lines around the room bounds
        Debug.DrawLine(topLeft, topRight, Color.blue);
        Debug.DrawLine(topRight, bottomRight, Color.blue);
        Debug.DrawLine(bottomRight, bottomLeft, Color.blue);
        Debug.DrawLine(bottomLeft, topLeft, Color.blue);
    }
}
