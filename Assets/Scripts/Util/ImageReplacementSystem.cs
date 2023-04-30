using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ImageReplacementSystem : MonoBehaviour
{
    public Sprite missingTexture;
    void Start()
    {
    }
    void Update()
    {
        ReplaceImages();
    }
    void ReplaceImages()
    {
        foreach (var image in FindObjectsOfType<Image>())
        {
            if (image.sprite == null)
            {
                if (image.gameObject.activeInHierarchy)
                {
                    if ((image.GetComponent<BattleUnit>() != null || image.tag.Equals("CharacterImage") || image.tag.Equals("NPCImage")))
                    {
                        image.sprite = missingTexture;
                    }
                }
                else
                {
                    continue;
                }
            }
        }
        foreach (var spriteRenderer in FindObjectsOfType<SpriteRenderer>())
        {
            if (spriteRenderer.GetComponent<SpriteRenderer>() != null && spriteRenderer.sprite == null &&
             spriteRenderer.gameObject.activeInHierarchy)
            {
                spriteRenderer.GetComponent<SpriteRenderer>().sprite = missingTexture;
            }
        }
    }
}