using UnityEngine;

public class Item : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int ID;

    //Shop fields
    public int buyPrice = 10; //Buy from shop
    [Range(0, 1)]
    public float sellPriceMultiplier = 0.5f; //Sell for 50% buy price

    public int GetSellPrice()
    {
        return Mathf.RoundToInt(buyPrice * sellPriceMultiplier);
    }
}
