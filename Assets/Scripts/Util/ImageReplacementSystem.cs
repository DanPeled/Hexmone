using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ImageReplacementSystem : MonoBehaviour
{
    public Sprite missingCreatureTexture;
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
                if ((image.GetComponent<BattleUnit>() != null || image.tag.Equals("CharacterImage")) && image.gameObject.activeInHierarchy)
                {
                    image.sprite = missingCreatureTexture;
                }
                else
                {
                    continue;
                }
            }
        }
    }
}