using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField] private Tilemap floorTilemap, wallTimeMap, spawnLocationMap;
    [SerializeField] private TileBase floorTile, wallTile, spawnPointTile;

    public void PaintFloorTiles(IEnumerable<Vector2Int> _floorPositions)
    {
        PaintTiles(_floorPositions, floorTilemap, floorTile);
    }

    public void PaintSpawnPoints(IEnumerable<Vector2Int> _spawnPointPositions)
    {
        PaintTiles(_spawnPointPositions, spawnLocationMap, spawnPointTile);
    }

    private void PaintTiles(IEnumerable<Vector2Int> _positions, Tilemap _tilemap, TileBase _tile)
    {
        foreach (var _position in _positions)
        {
            PaintSingleTile(_tilemap, _tile, _position);
        }
    }

    internal void PaintSingleBasicWall(Vector2Int _position)
    {
        PaintSingleTile(wallTimeMap, wallTile, _position);
    }

    private void PaintSingleTile(Tilemap _tilemap, TileBase _tile, Vector2Int _position)
    {
        var tilePosition = _tilemap.WorldToCell((Vector3Int)_position);
        _tilemap.SetTile(tilePosition, _tile);
    }

    public void Clear() 
    {
        floorTilemap.ClearAllTiles();
        wallTimeMap.ClearAllTiles();
        spawnLocationMap.ClearAllTiles();
    }
}


  
    