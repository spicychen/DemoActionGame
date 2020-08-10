using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using System;

public class Shop : MonoBehaviour
{
    private List<CatalogItem> items;
    public List<Item> converted_items;

    public GameObject shop_panel;
    public ItemSlot[] itemSlots;

    public void SetItems(List<CatalogItem> catalog)
    {
        items = catalog;
        converted_items = new List<Item>();
        ConvertItems();
        ShowItems();
        shop_panel.SetActive(true);
    }

    public void ConvertItems()
    {
        if (items.Count>0)
        {
            foreach (CatalogItem item in items)
            {
                var img_path = String.Format("ItemSprites/{0}", item.ItemId);
                int price_gold = 0;
                try
                {
                    price_gold = (int) item.VirtualCurrencyPrices["GD"];
                }
                catch (KeyNotFoundException)
                {
                    price_gold = 99999999;
                }

                converted_items.Add(new Item(img_path, item.ItemId, item.DisplayName, price_gold));
            }
        }
    }

    public void ShowItems()
    {
        if(converted_items.Count <= itemSlots.Length)
        {
            for(int x=0; x < converted_items.Count; x++)
            {
                itemSlots[x].SetSlotItem(converted_items[x].img_path, converted_items[x].name, converted_items[x].price_gold);
            }
        }
    }
}
