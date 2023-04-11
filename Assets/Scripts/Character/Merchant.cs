using System.Collections;
using UnityEngine;

public class Merchant : MonoBehaviour
{
    public IEnumerator Trade(){
        yield return ShopController.i.StartTrade(this);
    }
}