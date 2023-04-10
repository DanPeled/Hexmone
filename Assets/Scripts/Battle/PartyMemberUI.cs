using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartyMemberUI : MonoBehaviour
{
    public TextMeshProUGUI nameText,
        lvlText, messageText;
    public HPBar hpBar;
    Creature _creature;

    public void Init(Creature creature)
    {
        this._creature = creature;
        UpdateData();
        SetMessage("");

        _creature.OnHPChanged += UpdateData;
    }
    void UpdateData(){

        nameText.text = _creature._base.name;
        lvlText.text = $"Lvl {_creature.level}";

        hpBar.SetHP((float)_creature.HP, _creature.maxHealth);
        StartCoroutine(hpBar.SetHPSmooth((float)_creature.HP));
    }
    public void SetMessage(string msg){
        messageText.text = msg;
    }
    public void SetSelected(bool selected)
    {
        if (selected)
        {
            nameText.color = GlobalSettings.i.highlightedColor;
        }
        else
        {
            nameText.color = Color.black;
        }
    }
}
