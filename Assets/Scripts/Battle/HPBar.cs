using UnityEngine;
using System.Collections;

public class HPBar : MonoBehaviour
{
    public GameObject health;

    public void SetHP(float hp)
    {
        this.health = transform.GetChild(1).gameObject;
        health.transform.localScale = new Vector3(hp, 1f);
    }

    public IEnumerator SetHPSmooth(float newHP)
    {
        this.health = transform.GetChild(1).gameObject;
        float curHP = health.transform.localScale.x;
        float maxHP = 220f; // maximum HP value
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
    }
}
