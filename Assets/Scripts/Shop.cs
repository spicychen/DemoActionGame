using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class Shop : MonoBehaviour
{
    private List<CatalogItem> items;

    public void SetItems(List<CatalogItem> catalog)
    {
        items = catalog;
    }

    public void ShowItems()
    {
        Debug.Log(items[0].DisplayName);
        Debug.Log(items[0].VirtualCurrencyPrices);
    }
}
