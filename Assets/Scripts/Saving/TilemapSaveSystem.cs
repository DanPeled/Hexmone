using UnityEngine;
using UnityEngine.Tilemaps;
using System.Text;
using System.Collections.Generic;

public class TilemapSaveSystem : MonoBehaviour
{
    public Tilemap tilemap;

    [System.Serializable]
    public struct TileData
    {
        public string name;
        public Vector3Int position;
    }

    [System.Serializable]
    public struct TilemapData
    {
        public List<TileData> tiles;
    }

    public void SaveTilemapToJson()
    {
        TilemapData data = new TilemapData();
        data.tiles = new List<TileData>();

        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);

            if (tile != null)
            {
                TileData tileData = new TileData();
                tileData.name = tile.name;
                tileData.position = pos;

                data.tiles.Add(tileData);
            }
        }

        string jsonData = JsonUtility.ToJson(data);
        StringBuilder stringBuilder = new StringBuilder(jsonData);
        System.IO.File.WriteAllText(Application.dataPath + "/tilemapData.json", stringBuilder.ToString());
    }

    public void LoadTilemapFromJson()
    {
        string jsonData = System.IO.File.ReadAllText(Application.dataPath + "/tilemapData.json");
        TilemapData data = JsonUtility.FromJson<TilemapData>(jsonData);

        foreach (TileData tileData in data.tiles)
        {
            TileBase tile = Resources.Load<TileBase>(tileData.name);
            tilemap.SetTile(tileData.position, tile);
        }
    }
}
