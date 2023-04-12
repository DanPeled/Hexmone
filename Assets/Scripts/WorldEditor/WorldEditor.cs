using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;
using UnityEngine.UI;
public class WorldEditor : MonoBehaviour
{
    [Range(1, 100)]
    public float brushSize = 1f;
    public WorldEditorMode mode;
    [Header("Refrences")]
    public Tilemap tilemap;
    public GameObject playerPrefab;
    public static TileBase selectedTile;
    public static WorldEditor i;
    public Slider slider;
    public TextMeshProUGUI brushSizeText;

    [Header("Vars")]
    public float zoomChange;
    public float smoothChange, minSize, maxSize;
    public static bool isHoveringButton;
    public float movementSpeed = 0.05f;


    void Awake()
    {
        i = this;
    }

    void Start()
    {

    }

    public void Clear()
    {
        tilemap.ClearAllTiles();
    }
    void Update()
    {
        brushSizeText.text = Mathf.FloorToInt(slider.value).ToString();
        if (brushSize != slider.value)
        {
            brushSize = slider.value;
            brushSize = Mathf.FloorToInt(brushSize);
            slider.value = brushSize;
        }
        if (mode == WorldEditorMode.Draw)
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int cellPosition = tilemap.WorldToCell(mouseWorldPosition);
                if (!isHoveringButton)
                {
                    // Set tile on a rectangle of cells based on the brush size
                    for (int x = cellPosition.x - Mathf.FloorToInt(brushSize / 2f); x <= cellPosition.x + Mathf.FloorToInt(brushSize / 2f); x++)
                    {
                        for (int y = cellPosition.y - Mathf.FloorToInt(brushSize / 2f); y <= cellPosition.y + Mathf.FloorToInt(brushSize / 2f); y++)
                        {
                            tilemap.SetTile(new Vector3Int(x, y, 0), selectedTile);
                        }
                    }
                }


            }
            else if (Input.GetMouseButton(1))
            {
                Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int cellPosition = tilemap.WorldToCell(mouseWorldPosition);
                if (!isHoveringButton)
                {
                    // Set tile on a rectangle of cells based on the brush size
                    for (int x = cellPosition.x - Mathf.FloorToInt(brushSize / 2f); x <= cellPosition.x + Mathf.FloorToInt(brushSize / 2f); x++)
                    {
                        for (int y = cellPosition.y - Mathf.FloorToInt(brushSize / 2f); y <= cellPosition.y + Mathf.FloorToInt(brushSize / 2f); y++)
                        {
                            tilemap.SetTile(new Vector3Int(x, y, 0), null);
                        }
                    }
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

        if (InputSystem.up.isPressed())
        {
            Camera.main.transform.position += new Vector3(0, movementSpeed);
        }
        if (InputSystem.down.isPressed())
        {
            Camera.main.transform.position -= new Vector3(0, movementSpeed);
        }
        if (InputSystem.left.isPressed())
        {
            Camera.main.transform.position -= new Vector3(movementSpeed, 0);
        }
        if (InputSystem.right.isPressed())
        {
            Camera.main.transform.position += new Vector3(movementSpeed, 0);
        }
    }
    private bool isFilling = false;
    private bool isErasing = false;
    private Vector3Int startCell;

    // Fills a rectangular region of tiles with the specified tile
    public void Fill(RectInt region)
    {
        int brushSizeInt = Mathf.RoundToInt(brushSize);
        for (int x = region.xMin - brushSizeInt; x <= region.xMax + brushSizeInt; x++)
        {
            for (int y = region.yMin - brushSizeInt; y <= region.yMax + brushSizeInt; y++)
            {
                if (Vector2.Distance(new Vector2(x, y), new Vector2(region.center.x, region.center.y)) <= brushSize)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), selectedTile);
                }
            }
        }
    }

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