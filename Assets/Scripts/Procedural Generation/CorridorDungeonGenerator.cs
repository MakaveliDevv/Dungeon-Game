using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CorridorDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    // [SerializeField] private int corridorLength = 14, corridorCount = 5;
    // [SerializeField] [Range(.1f, 1f)] private float roomPercent = .8f;

    // protected override void RunProceduralGeneration()
    // {
    //     CorridorGenerator();
    // }

    // private void CorridorGenerator()
    // {
    //     HashSet<Vector2Int> floorPositions = new();
    //     HashSet<Vector2Int> potentialRoomPositions = new();

    //     List<List<Vector2Int>> corridors = CreateCorridors(floorPositions, potentialRoomPositions);

    //     HashSet<Vector2Int> roomPositions = CreateRooms(potentialRoomPositions);
    //     List<Vector2Int> deadEnds = FindAllDeadEnds(floorPositions);

    //     CreateRoomsAtDeadEnd(deadEnds, roomPositions);
    //     floorPositions.UnionWith(roomPositions);

    //     for (int i = 0; i < corridors.Count; i++)
    //     {
    //         // corridors[i] = IncreaseCorridorSizeByOne(corridors[i]);
    //         corridors[i] = IncreaseCorridorBrush3By3(corridors[i]);
    //         floorPositions.UnionWith(corridors[i]);    
    //     }

    //     tilemapVisualizer.PaintFloorTiles(floorPositions);
    //     WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
    // }

    // private List<Vector2Int> IncreaseCorridorBrush3By3(List<Vector2Int> _corridor)
    // {
    //     List<Vector2Int> newCorridor = new();

    //     for (int i = 1; i < _corridor.Count; i++)
    //     {
    //         for (int x = -1; x < 2; x++)
    //         {
    //             for (int y = -1; y < 2; y++)
    //             {
    //                 newCorridor.Add(_corridor[i - 1] + new Vector2Int(x, y));
    //             }
    //         }            
    //     }

    //     return newCorridor;
    // }

    // private void CreateRoomsAtDeadEnd(List<Vector2Int> _deadEnds, HashSet<Vector2Int> _roomFloors)
    // {
    //     foreach (var _position in _deadEnds)
    //     {
    //         if(_roomFloors.Contains(_position) == false) 
    //         {
    //             var room = StartRandomWalk(randomWalkParameters, _position);
    //             _roomFloors.UnionWith(room);
    //         }
    //     }
    // }

    // private List<Vector2Int> FindAllDeadEnds(HashSet<Vector2Int> _floorPositions)
    // {
    //     List<Vector2Int> deadEnds = new();
    //     foreach (var _position in _floorPositions)
    //     {
    //         int neighboursCount = 0;
    //         foreach (var _direction in Direction2D.cardinalDirectionsList)
    //         {
    //             if(_floorPositions.Contains(_position + _direction)) 
    //                 neighboursCount ++;
    //         }

    //         if(neighboursCount == 1) 
    //         {
    //             deadEnds.Add(_position);
    //         }
    //     }

    //     return deadEnds;
    // }

    // private HashSet<Vector2Int> CreateRooms(HashSet<Vector2Int> _potentialRoomPositions)
    // {
    //     HashSet<Vector2Int> roomPositions = new();
    //     int roomToCreateCount = Mathf.RoundToInt(_potentialRoomPositions.Count * roomPercent);

    //     List<Vector2Int> roomToCreate = _potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomToCreateCount).ToList();

    //     foreach (var _roomPosition in roomToCreate)
    //     {
    //         var roomFloor = StartRandomWalk(randomWalkParameters, _roomPosition);
    //         roomPositions.UnionWith(roomFloor);
    //     }

    //     return roomPositions;
    // }

    // private List<List<Vector2Int>> CreateCorridors(HashSet<Vector2Int> _floorPositions, HashSet<Vector2Int> _potentialRoomPositions)
    // {
    //     var currentPosition = startPosition;
    //     _potentialRoomPositions.Add(currentPosition);

    //     List<List<Vector2Int>> corridors = new();

    //     for (int i = 0; i < corridorCount; i++)
    //     {
    //         var corridor = ProceduralGenerationAlgorithms.RandomWalkCorridor(currentPosition, corridorLength);
    //         corridors.Add(corridor);
            
    //         currentPosition = corridor[corridor.Count - 1];
    //         _potentialRoomPositions.Add(currentPosition);
    //         _floorPositions.UnionWith(corridor);
    //     }

    //     return corridors;
    // }

    // public List<Vector2Int> IncreaseCorridorSizeByOne(List<Vector2Int> _corridor)
    // {
    //     List<Vector2Int> newCorridor = new();
    //     Vector2Int previousDirection = Vector2Int.zero;

    //     for (int i = 1; i < _corridor.Count; i++)
    //     {
    //         Vector2Int directionFromCell = _corridor[i] - _corridor[i - 1];
    //         if(previousDirection != Vector2Int.zero && directionFromCell != previousDirection) 
    //         {
    //             // HANDLE CORNER
    //             for (int x = -1; x < 2; x++)
    //             {
    //                 for (int y = -1; y < 2; y++)
    //                 {
    //                     newCorridor.Add(_corridor[i - 1] + new Vector2Int(x, y));
    //                 }
    //             }

    //             previousDirection = directionFromCell;

    //         } else 
    //         {
    //             // Add a single cell in the direction + 90 degrees
    //             Vector2Int newCorridorTileOffset = GetDirection90From(directionFromCell);
    //             newCorridor.Add(_corridor[i - 1]);
    //             newCorridor.Add(_corridor[i - 1] + newCorridorTileOffset);
    //         }
    //     }

    //     return newCorridor;
    // }

    // private Vector2Int GetDirection90From(Vector2Int _directionFromCell)
    // {
    //     if(_directionFromCell == Vector2Int.up) return Vector2Int.right;
    //     if(_directionFromCell == Vector2Int.right) return Vector2Int.down;
    //     if(_directionFromCell == Vector2Int.down) return Vector2Int.left;
    //     if(_directionFromCell == Vector2Int.left) return Vector2Int.up;

    //     return Vector2Int.zero;
    // }
}
