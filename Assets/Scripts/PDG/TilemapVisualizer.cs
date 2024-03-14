using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField] private Tilemap floorTilemap, wallTilemap;
    [SerializeField] private TileBase floorTileBase, wallTileBase;

    public void PaintFloorTiles(IEnumerable<Vector2Int> _floorPos) 
    {
        PaintTiles(_floorPos, floorTilemap, floorTileBase);
    }
    
    private void PaintTiles(IEnumerable<Vector2Int> _tilePos, Tilemap _tilemap, TileBase _tileBase) 
    {
        foreach (var position in _tilePos)
        {
            // Paint tile
            SetSingleTile(position, _tilemap, _tileBase);    
        }
    }

    private void SetSingleTile(Vector2Int _tilePos, Tilemap _tilemap, TileBase _tileBase) 
    {
        var tilePosition = _tilemap.WorldToCell((Vector3Int)_tilePos);
        _tilemap.SetTile(tilePosition, _tileBase);
    }
    
    internal void PaintWallTiles(Vector2Int _pos) 
    {
        // Paint tile
        SetSingleTile(_pos, wallTilemap, wallTileBase);
    }

    public void Clear() 
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
    }
}
