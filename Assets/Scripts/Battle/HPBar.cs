using UnityEngine;
using System.Collections;

public class HPBar : MonoBehaviour
{
    public GameObject health;
    public float maxHP;
    public bool isUpdating;
    public void SetHP(float hp, float maxHP)
    {
        this.maxHP = maxHP;
        hp = CalculateHPRatio(hp);
        this.health = transform.GetChild(1).gameObject;
        health.transform.localScale = new Vector3(hp, 1f);
    }

    public IEnumerator SetHPSmooth(float newHP)
    {
        this.isUpdating = true;
        newHP = CalculateHPRatio(newHP);
        this.health = transform.GetChild(1).gameObject;
        float curHP = health.transform.localScale.x;
        newHP = Mathf.Clamp(newHP, 0f, maxHP); // clamp newHP to valid range
        float changeAmt = curHP - newHP;
        float minHP = 0f; // minimum HP value
        while (curHP - newHP > 0.01f) // check against a small value
        {
            curHP -= changeAmt * Time.deltaTime;
            if (curHP < minHP) // check if curHP goes below minHP
            {
                curHP = minHP;
            }
            health.transform.localScale = new Vector3(curHP, 1f);
            yield return null;
        }
        health.transform.localScale = new Vector3(newHP, 1f);
        this.isUpdating = false;
    }

    public float CalculateHPRatio(float hp)
    {
        return Mathf.Clamp(hp / maxHP, 0, 1);
    }
}
