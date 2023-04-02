using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;
using UnityEngine.UI;
public class WorldEditor : MonoBehaviour
{
    public Tilemap tilemap;
    public GameObject playerPrefab;
    public static TileBase selectedTile;
    public float zoomChange, smoothChange, minSize, maxSize;
    public static bool isHoveringButton;
    public static WorldEditor i;
    public float speed = 0.05f;
    public WorldEditorMode mode;

    void Awake()
    {
        i = this;
    }
    void Start()
    {
        
    }
    void Update()
    {
        if (mode == WorldEditorMode.Draw)
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int cellPosition = tilemap.WorldToCell(mouseWorldPosition);
                if (!isHoveringButton)
                {
                    tilemap.SetTile(cellPosition, selectedTile);
                }


            }
            else if (Input.GetMouseButton(1))
            {
                Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int cellPosition = tilemap.WorldToCell(mouseWorldPosition);
                if (!isHoveringButton)
                {
                    tilemap.SetTile(cellPosition, null);
                }
            }
        }
        if (mode == WorldEditorMode.Fill)
        {
            if (!isHoveringButton)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    StartFill(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    EndFill(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                }
                else if (isFilling)
                {
                    Vector3Int currentCell = tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    RectInt fillRegion = new RectInt(
                        Mathf.Min(startCell.x, currentCell.x),
                        Mathf.Min(startCell.y, currentCell.y),
                        Mathf.Abs(currentCell.x - startCell.x) + 1,
                        Mathf.Abs(currentCell.y - startCell.y) + 1
                    );
                    Fill(fillRegion);
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    StartErase(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                }
                else if (Input.GetMouseButtonUp(1))
                {
                    EndErase(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                }
            }
        }
        
        if (Input.mouseScrollDelta.y > 0)
        {
            Camera.main.orthographicSize -= zoomChange * Time.deltaTime * smoothChange;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            Camera.main.orthographicSize += zoomChange * Time.deltaTime * smoothChange;
        }
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minSize, maxSize);

        if (InputSystem.instance.up.isPressed())
        {
            Camera.main.transform.position += new Vector3(0, speed);
        }
        if (InputSystem.instance.down.isPressed())
        {
            Camera.main.transform.position -= new Vector3(0, speed);
        }
        if (InputSystem.instance.left.isPressed())
        {
            Camera.main.transform.position -= new Vector3(speed, 0);
        }
        if (InputSystem.instance.right.isPressed())
        {
            Camera.main.transform.position += new Vector3(speed, 0);
        }
    }
    private bool isFilling = false;
    private bool isErasing = false;
    private Vector3Int startCell;

    // Fills a rectangular region of tiles with the specified tile
    public void Fill(RectInt region)
    {
        for (int x = region.xMin; x <= region.xMax; x++)
        {
            for (int y = region.yMin; y <= region.yMax; y++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), selectedTile);
            }
        }
    }
    
    // Begins the fill operation
    public void StartFill(Vector3 mousePosition)
    {
        startCell = tilemap.WorldToCell(mousePosition);
        isFilling = true;
    }

    // Ends the fill operation
    public void EndFill(Vector3 mousePosition)
    {
        if (!isFilling)
        {
            return;
        }

        isFilling = false;
        Vector3Int endCell = tilemap.WorldToCell(mousePosition);
        RectInt fillRegion = new RectInt(
            Mathf.Min(startCell.x, endCell.x),
            Mathf.Min(startCell.y, endCell.y),
            Mathf.Abs(endCell.x - startCell.x) + 1,
            Mathf.Abs(endCell.y - startCell.y) + 1
        );
        Fill(fillRegion);
    }
    public void SetMode(int modeIndex)
    {
        WorldEditorMode mode = WorldEditorMode.Draw;
        switch (modeIndex)
        {
            case 1:
                mode = WorldEditorMode.Draw;
                break;
            case 2:
                mode = WorldEditorMode.Fill;
                break;
        }
        this.mode = mode;
    }

    // Begins the erase operation
    public void StartErase(Vector3 mousePosition)
    {
        startCell = tilemap.WorldToCell(mousePosition);
        isErasing = true;
    }

    // Ends the erase operation
    public void EndErase(Vector3 mousePosition)
    {
        if (!isErasing)
        {
            return;
        }

        isErasing = false;
        Vector3Int endCell = tilemap.WorldToCell(mousePosition);
        RectInt eraseRegion = new RectInt(
            Mathf.Min(startCell.x, endCell.x),
            Mathf.Min(startCell.y, endCell.y),
            Mathf.Abs(endCell.x - startCell.x) + 1,
            Mathf.Abs(endCell.y - startCell.y) + 1
        );
        Erase(eraseRegion);
    }
    // Erases a rectangular region of tiles
    public void Erase(RectInt region)
    {
        for (int x = region.xMin; x <= region.xMax; x++)
        {
            for (int y = region.yMin; y <= region.yMax; y++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), null);
            }
        }
    }
}
public enum WorldEditorMode
{
    Draw,
    Fill,
    Erase
}