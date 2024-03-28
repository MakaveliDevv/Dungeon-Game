using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class RoomDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    protected override void RunProceduralGeneration()
    {
        CreateRooms();
    }

    private void CreateRooms()
    {
<<<<<<< HEAD
        roomsList = ProceduralGenerationAlgorithms.BinarySpacePartitioning(
            new BoundsInt((Vector3Int)startPosition, new Vector3Int(dungeon.width, dungeon.height, 0)), dungeon.minRoomWidth, dungeon.minRoomHeight
        );
=======
        var roomsList = ProceduralGenerationAlgorithms.BinarySpacePartitioning(new BoundsInt((Vector3Int)startPosition, new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth, minRoomHeight);

        HashSet<Vector2Int> floor = new();
        if (randomWalkRooms)
        {
            floor = CreateRoomsRandomly(roomsList);

        }
        else
        {
            floor = CreateSimpleRooms(roomsList);
        }
>>>>>>> parent of a8c6dcc (Update 2.6)

        HashSet<Vector2Int> floor = GenerateRooms(roomsList);
        List<Vector2Int> roomCenters = new();
        Vector2Int center;
       
        
        foreach (var _room in roomsList)
        {
            center = (Vector2Int)Vector3Int.RoundToInt(_room.center);
            roomCenters.Add(center);
            roomCount++;
        }

        // Connect the rooms
        HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
        floor.UnionWith(corridors);

        // Visualize the tilemap
        tilemapVisualizer.PaintFloorTiles(floor);

        // Create walls
        HashSet<Vector2Int> walls = WallGeneratorr.CreateWalls(floor, tilemapVisualizer);
         
        HashSet<Vector2Int> floorSpawnPoints = GenerateSpawnPoints(floor, walls, maxSpawnPoints);

                
        // Generate spawn points within the floor positions
        // HashSet<Vector2Int> floorSpawnPoints = GenerateSpawnPoints(floor, walls, maxSpawnPoints);

        // // Determine the number of enemies to spawn at each spawn point
        // int enemiesPerSpawnPoint = maxEnemiesAmount / floorSpawnPoints.Count;

        // foreach (Vector2Int point in floorSpawnPoints)
        // {
        //     for (int i = 0; i < enemiesPerSpawnPoint; i++)
        //     {
        //         if (CanSpawnEnemyAt(point))
        //         {
        //             SpawnEnemyAt(point);
        //         }
        //     }
        //     SpawnSpawnPointAt(point);
        // }

        // SpawnPlayer(playerPrefab);

        // // Visualize spawn points using TilemapVisualizer
        // tilemapVisualizer.PaintSpawnPoints(floorSpawnPoints);
        

        foreach (Vector2Int point in floorSpawnPoints)
        {
            if (CanSpawnEnemyAt(point))
            {
                SpawnEnemyAt(point);
                SpawnSpawnPointAt(point);
            }
        }

        SpawnPlayer(playerPrefab);

        // Visualize spawn points using TilemapVisualizer
        tilemapVisualizer.PaintSpawnPoints(floorSpawnPoints);
    }
  
    private HashSet<Vector2Int> GenerateRooms(List<BoundsInt> _roomsList)
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
<<<<<<< HEAD
                if ((_position.x >= (roomBounds.xMin + dungeon.offset) && _position.x <= (roomBounds.xMax - dungeon.offset)
                    && _position.y >= (roomBounds.yMin + dungeon.offset) && _position.y <= (roomBounds.yMax - dungeon.offset)))
=======
                if(_position.x >= (roomBounds.xMin + offset) && _position.x <= (roomBounds.xMax - offset) 
                && _position.y >= (roomBounds.yMin + offset) && _position.y <= (roomBounds.yMax - offset)) 
                {
>>>>>>> parent of a8c6dcc (Update 2.6)
                    floor.Add(_position);
                }
            }
        }

        // Debug.Log($"Total rooms: {_roomsList.Count}, Total spawn points generated: {floorSpawnPoints.Count}");

        // Visualize spawn points using TilemapVisualizer
        tilemapVisualizer.PaintSpawnPoints(floorSpawnPoints);

        return floor;
    }

    private HashSet<Vector2Int> GenerateSpawnPoints(HashSet<Vector2Int> floor, HashSet<Vector2Int> walls, int maxSpawnPoints)
    {
        HashSet<Vector2Int> floorSpawnPoints = new(); // Floor for spawnpoints

        float minDistanceToWallSquared = 5f; 
        int maxAttemptsPerPoint = 10;

        for (int i = 0; i < maxSpawnPoints; i++)
        {
            int attempts = 0;
            Vector2Int spawnPoint;
            bool validSpawnPoint = false;

            while (!validSpawnPoint && attempts < maxAttemptsPerPoint * floor.Count) 
            {
                spawnPoint = floor.ElementAt(Random.Range(0, floor.Count));

                // Check if the spawn point is too close to any wall
                if (IsTooCloseToWall(spawnPoint, walls, minDistanceToWallSquared))
                {
                    // Find a new spawn point away from the wall within the same floor
                    spawnPoint = FindSpawnPointAwayFromWall(floor, walls, minDistanceToWallSquared);
                }

                // Check if the spawn point is accessible
                if (IsAccessible(spawnPoint, walls)) 
                {
                    floorSpawnPoints.Add(spawnPoint);
                    validSpawnPoint = true;
                }

                attempts++;
            }
        }

        return floorSpawnPoints;
    }

    private Vector2Int FindSpawnPointAwayFromWall(HashSet<Vector2Int> floor, HashSet<Vector2Int> walls, float minDistanceToWallSquared)
    {
        // Iterate over the floor positions and find a position that is not too close to any wall
        foreach (Vector2Int position in floor)
        {
            if (!IsTooCloseToWall(position, walls, minDistanceToWallSquared))
            {
                return position;
            }
        }

        // If no suitable position is found, return a random position from the floor
        return floor.ElementAt(Random.Range(0, floor.Count));
    }

    private bool IsAccessible(Vector2Int position, HashSet<Vector2Int> walls)
    {
        // Check if there's enough space around the spawn position
        foreach (var direction in Directionss2D.cardinalDirectionsList)
        {
            Vector2Int neighborPosition = position + direction;
            if (!walls.Contains(neighborPosition))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsTooCloseToWall(Vector2Int position, HashSet<Vector2Int> walls, float minDistanceSquared)
    {
        foreach (Vector2Int wallPosition in walls)
        {
            float distanceSquared = (position - wallPosition).sqrMagnitude;
            if (distanceSquared < minDistanceSquared)
            {
                return true;
            }
        }

        return false;
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

<<<<<<< HEAD
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
=======
            HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closestPoint);
            currentRoomCenter = closestPoint;
            corridors.UnionWith(newCorridor);
>>>>>>> parent of a8c6dcc (Update 2.6)
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

    private void SpawnPlayer(GameObject _player) 
    {
        // Get the last room index
        int lastRoomIndex = roomsList.Count > 0 ? roomsList.Count - 1 : -1;

        // Get the center from that room
        Vector3 roomCenter = roomsList[lastRoomIndex].center;
        // Vector3 roomCenter = Vector3.zero;
        // if (roomsList.Count > 0)
        // {
        //     roomCenter = roomsList[0].center;
        //     Debug.Log(roomCenter);
        // }

        // Spawn the player at the center
        GameObject newPlayer = Instantiate(_player, roomCenter, Quaternion.identity) as GameObject;
        newPlayer.name = "HeroCharacter";
        playerList.Add(newPlayer);
    }


    private void SpawnSpawnPointAt(Vector2Int position) 
    {
        Vector2 spawnPosition = new(position.x, position.y);

        GameObject spawnPoint = Instantiate(spawnPointPrefab, spawnPosition, Quaternion.identity);
        gameObject_SpawnPoints_List.Add(spawnPoint);

    }
    
    private void SpawnEnemyAt(Vector2Int position)
    {
        // Get amount of enemies
        float amount = Random.Range(0, enemiesAmount + 1);

        // Get type of enemies
        int typeIndex = Random.Range(0, enemiesType.Count); 

        GameObject enemyPrefab = enemiesType[typeIndex];

        Vector2 spawnPosition = new(position.x, position.y);


        // Spawn the enemies at the position
        for (int i = 0; i < amount; i++)
        {
            // Instantiate the enemy prefab at the position
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity) as GameObject;
            enemies.Add(newEnemy);
        }

        // Debug.Log($"Spawned {amount} of {enemyPrefab.name} at position {position}");
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
        for (int i = 0; i < roomsList.Count; i++)
        {
            DrawRoomBounds(roomsList[i]);
            DrawFirstRoomBounds(roomsList[0]);
            DrawLastRoomBounds(roomsList[^1]);
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

    private void DrawFirstRoomBounds(BoundsInt roomBounds) 
    {
        Vector3Int min = roomBounds.min;
        Vector3Int max = roomBounds.max;

        Vector3 topLeft = new(min.x, min.y, 0);
        Vector3 topRight = new(max.x, min.y, 0);
        Vector3 bottomRight = new(max.x, max.y, 0);
        Vector3 bottomLeft = new(min.x, max.y, 0);

        // Draw lines around the room bounds
        Debug.DrawLine(topLeft, topRight, Color.green);
        Debug.DrawLine(topRight, bottomRight, Color.green);
        Debug.DrawLine(bottomRight, bottomLeft, Color.green);
        Debug.DrawLine(bottomLeft, topLeft, Color.green);
    }

    private void DrawLastRoomBounds(BoundsInt roomBounds) 
    {
        Vector3Int min = roomBounds.min;
        Vector3Int max = roomBounds.max;

        Vector3 topLeft = new(min.x, min.y, 0);
        Vector3 topRight = new(max.x, min.y, 0);
        Vector3 bottomRight = new(max.x, max.y, 0);
        Vector3 bottomLeft = new(min.x, max.y, 0);

        // Draw lines around the room bounds
        Debug.DrawLine(topLeft, topRight, Color.red);
        Debug.DrawLine(topRight, bottomRight, Color.red);
        Debug.DrawLine(bottomRight, bottomLeft, Color.red);
        Debug.DrawLine(bottomLeft, topLeft, Color.red);
    }
}
