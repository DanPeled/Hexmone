using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    public BattleHud hud;
    public Vector3 originalPos;
    public Color originalColor;
    public bool isPlayerUnit;
    public Creature creature { get; set; }
    public Image image;

    public void Awake()
    {
        this.image = GetComponent<Image>();
        originalPos = image.transform.localPosition;
        this.originalColor = image.color;
    }

    public void Setup(Creature creature)
    {
        if (isPlayerUnit)
        {
            image.sprite = creature._base.backSprite;
        }
        else
        {
            image.sprite = creature._base.frontSprite;
        }
        hud.SetData(creature);
        image.color = originalColor;
        PlayEnterAnimation();
        this.creature = creature;
    }

    public void PlayEnterAnimation()
    {
        image.color = originalColor;
        if (isPlayerUnit)
        {
            image.transform.localPosition = new Vector3(-500f, originalPos.y);
        }
        else
        {
            image.transform.localPosition = new Vector3(500f, originalPos.y);
        }
        image.transform.DOLocalMoveX(originalPos.x, 1f);
    }

    public void PlayAttackAnimation()
    {
        var seq = DOTween.Sequence();
        if (isPlayerUnit)
        {
            seq.Append(image.transform.DOLocalMoveX(originalPos.x + 50f, 0.25f));
        }
        else
        {
            seq.Append(image.transform.DOLocalMoveX(originalPos.x - 50f, 0.25f));
        }
        seq.Append(image.transform.DOLocalMoveX(originalPos.x, 0.25f));
    }

    public void PlayHitAnimation()
    {
        var seq = DOTween.Sequence();
        seq.Append(image.DOColor(Color.gray, 0.1f));
        seq.Append(image.DOColor(this.originalColor, 0.1f));
    }

    public void PlayFaintAnimation()
    {
        var seq = DOTween.Sequence();
        seq.Append(image.transform.DOLocalMoveY(originalPos.y - 150f, 0.5f));
        seq.Join(image.DOFade(0f, 0.5f));
    }
}
