using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WorldEditMouseController : MonoBehaviour
{
    public static Sprite sprite;
    SpriteRenderer image;
    // Start is called before the first frame update
    void Start()
    {
        image = this.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = WorldEditor.i.tilemap.WorldToCell(mouseWorldPosition);
        this.image.sprite = sprite;
        this.transform.position = cellPosition + WorldEditor.i.tilemap.tileAnchor;
    }
}
