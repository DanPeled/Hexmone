using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ImageReplacementSystem : MonoBehaviour
{
    public Sprite missingTexture;
    void Start()
    {
        ReplaceImages();
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
                    if ((image.GetComponent<BattleUnit>() != null || image.tag.Equals("CharacterImage")))
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
            if (spriteRenderer.GetComponent<SpriteRenderer>() != null &&
             spriteRenderer.gameObject.activeInHierarchy && spriteRenderer.GetComponentInParent<Player>() == null && spriteRenderer.GetComponentInParent<TrainerController>() == null && !spriteRenderer.gameObject.name.StartsWith("Hexoball"))
            {
                spriteRenderer.GetComponent<SpriteRenderer>().sprite = missingTexture;
            }
        }
    }
}