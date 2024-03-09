using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField] private Tilemap floorTilemap, wallTimeMap;
    [SerializeField] private TileBase floorTile, wallTile;

    public void PaintFloorTiles(IEnumerable<Vector2Int> _floorPositions)
    {
        PaintTiles(_floorPositions, floorTilemap, floorTile);
    }

    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile)
    {
        foreach (var _position in positions)
        {
            PaintSingleTile(tilemap, tile, _position);
        }
    }

    internal void PaintSingleBasicWall(Vector2Int position)
    {
        PaintSingleTile(wallTimeMap, wallTile, position);
    }

    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    public void Clear() 
    {
        floorTilemap.ClearAllTiles();
        wallTimeMap.ClearAllTiles();
    }
}
