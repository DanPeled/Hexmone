using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapOcclusionCulling : MonoBehaviour
{
    public Camera mainCamera;
    public Tilemap[] tilemaps;
    public float cullingDistance = 10f;

    private List<Vector3Int> visibleTiles = new List<Vector3Int>();

    private void Awake()
    {
        if (!mainCamera)
        {
            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        // Calculate the camera frustum
        var cameraBounds = OrthographicBounds(mainCamera);

        // Determine the visible tiles
        visibleTiles.Clear();
        foreach (var tilemap in tilemaps)
        {
            if (tilemap == null) continue; // null check
            foreach (var tilePos in tilemap.cellBounds.allPositionsWithin)
            {
                var tileWorldPos = tilemap.CellToWorld(tilePos) + tilemap.cellSize / 2f;
                var tileBounds = new Bounds(tileWorldPos, tilemap.cellSize);
                if (tilemap.HasTile(tilePos) && cameraBounds.Intersects(tileBounds))
                {
                    visibleTiles.Add(tilePos);
                }
            }
        }

        // Set the visibility of the tiles
        foreach (var tilemap in tilemaps)
        {
            if (tilemap == null) continue; // null check
            foreach (var tilePos in tilemap.cellBounds.allPositionsWithin)
            {
                if (tilemap.HasTile(tilePos))
                {
                    var tileBase = tilemap.GetTile(tilePos);
                    var tile = tileBase as Tile;
                    if (tile == null) continue; // null check
                    var sprite = tile.sprite;
                    var color = tilemap.GetColor(tilePos);
                    if (visibleTiles.Contains(tilePos))
                    {
                        color.a = 1f;
                    }
                    else
                    {
                        color.a = 0.5f;
                    }
                    tilemap.SetColor(tilePos, color);
                }
            }
        }
    }



    private Bounds OrthographicBounds(Camera camera)
    {
        var screenAspect = (float)Screen.width / (float)Screen.height;
        var cameraHeight = camera.orthographicSize * 2;
        var cameraWidth = cameraHeight * screenAspect;
        var cameraPos = camera.transform.position;
        var cameraBounds = new Bounds(
            new Vector3(cameraPos.x, cameraPos.y, 0),
            new Vector3(cameraWidth, cameraHeight, 0));
        return cameraBounds;
    }
}
