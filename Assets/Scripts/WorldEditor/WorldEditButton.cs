using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
public class WorldEditButton : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    Image image;
    TileBase tile;
    void Start()
    {
        this.image = GetComponent<Image>();
    }
    public void SelectTile(TileBase tile)
    {
        WorldEditor.selectedTile = tile;
        this.tile = tile;
    }
    void Update()
    {
        if (this.tile != WorldEditor.selectedTile || WorldEditor.selectedTile == null)
        {
            image.color = Color.white;
        }
        else
        {
            //This is chosen
            image.color = Color.gray;
            WorldEditMouseController.sprite = image.sprite;
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        WorldEditor.isHoveringButton = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        WorldEditor.isHoveringButton = false;
    }
}