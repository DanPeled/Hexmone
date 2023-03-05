using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartyMemberUI : MonoBehaviour
{
    public TextMeshProUGUI nameText,
        lvlText;
    public HPBar hPBar;
    Creature _creature;
    public Color highlightedColor;

    public void SetData(Creature creature)
    {
        this._creature = creature;
        nameText.text = creature._base.creatureName;
        lvlText.text = $"Lvl {creature.level}";

        hPBar.SetHP((float)creature.HP);
        StartCoroutine(hPBar.SetHPSmooth((float)_creature.HP));
    }

    public void SetSelected(bool selected)
    {
        if (selected)
        {
            nameText.color = highlightedColor;
        }
        else
        {
            nameText.color = Color.black;
        }
    }
}
