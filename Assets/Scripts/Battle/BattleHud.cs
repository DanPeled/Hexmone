using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;
public class BattleHud : MonoBehaviour
{
    public TextMeshProUGUI nameText,
        lvlText, statusText;
    public HPBar hPBar;
    Creature _creature;
    public Color psnColor, brnColor, slpColor, parColor, frzColor;
    Dictionary<ConditionID, Color> statusColors;
    public GameObject expBar;

    public void SetData(Creature creature)
    {
        if(_creature != null ){
            _creature.OnStatusChanged -= SetStatusText;
            _creature.OnHPChanged -= UpdateHP;
        }

        this._creature = creature;
        nameText.text = creature._base.creatureName;
        SetLevel();

        hPBar.SetHP((float)creature.HP, _creature.maxHealth);
        SetExp();
        StartCoroutine(UpdateHPAsync());
        statusColors = new Dictionary<ConditionID, Color>(){
            {ConditionID.psn, psnColor},
            {ConditionID.brn, brnColor},
            {ConditionID.slp , slpColor},
            {ConditionID.par, parColor},
            {ConditionID.frz, frzColor}
        };
        SetStatusText();

        _creature.OnStatusChanged += SetStatusText;
        _creature.OnHPChanged += UpdateHP;
    }
    public void UpdateHP()
    {
        StartCoroutine(UpdateHPAsync());
    }
    public IEnumerator UpdateHPAsync()
    {
        yield return hPBar.SetHPSmooth((float)_creature.HP);
    }
    public IEnumerator WaitForHPUpdate(){
        yield return new WaitUntil(() => !hPBar.isUpdating);
    }
    public void SetStatusText()
    {
        if (_creature.status == null)
        {
            statusText.text = "";
        }
        else
        {
            statusText.text = _creature.status.iD.ToString().ToUpper();
            statusText.color = statusColors[_creature.status.iD];
        }
    }
    public void SetExp()
    {
        if (expBar == null) return;
        float normalizedExp = GetNormalizedExp();
        expBar.transform.localScale = new Vector3(normalizedExp, 1, 1);
    }
    float GetNormalizedExp()
    {
        int currentLevelExp = _creature._base.GetExpForLevel(_creature.level);
        int nextLevelExp = _creature._base.GetExpForLevel(_creature.level + 1);

        float normalizedExp = ((float)_creature.exp - (float)currentLevelExp) / ((float)nextLevelExp - (float)currentLevelExp);

        return Mathf.Clamp01(normalizedExp);

    }
    public IEnumerator SetExpSmooth(bool reset = false)
    {
        if (expBar == null) yield break;
        if (reset)
        {
            expBar.transform.localScale = new Vector3(0, 1, 1);
        }
        float normalizedExp = GetNormalizedExp();
        yield return expBar.transform.DOScaleX(normalizedExp, 1.5f);
    }
    public void SetLevel()
    {
        lvlText.text = $"L: {_creature.level}";
    }
}
