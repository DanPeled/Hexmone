using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Merchant : MonoBehaviour
{
    public List<ItemBase> items;

    public IEnumerator Trade()
    {
        yield return ShopController.i.StartTrade(this);
    }
}
