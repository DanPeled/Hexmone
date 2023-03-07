using System.Collections;
using UnityEngine;
using TMPro;

public class BattleHud : MonoBehaviour
{
    public TextMeshProUGUI nameText,
        lvlText;
    public HPBar hPBar;
    Creature _creature;

    public void SetData(Creature creature)
    {

        this._creature = creature;
        nameText.text = creature._base.creatureName;
        lvlText.text = $"Lvl {creature.level}";

        hPBar.SetHP((float)creature.HP);
        StartCoroutine(UpdateHP());
    }

    public IEnumerator UpdateHP()
    {
        yield return hPBar.SetHPSmooth((float)_creature.HP);
    }
}
