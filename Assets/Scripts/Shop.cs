using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    private List<CatalogItem> items;
    public List<Item> converted_items;

    public GameObject shop_panel;
    public ItemSlot[] itemSlots;

    public PlayFabHelper playFabHelper;

    public void SetItems(List<CatalogItem> catalog)
    {
        items = catalog;
        converted_items = new List<Item>();
        ConvertItems();
        shop_panel.SetActive(true);
        ShowItems();
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
                string item_id = converted_items[x].item_id;
                int item_price = converted_items[x].price_gold;
                itemSlots[x].SetSlotItem(converted_items[x].img_path, converted_items[x].name, converted_items[x].price_gold);
                string message = String.Format("{0} will cost {1} gold",converted_items[x].name, converted_items[x].price_gold);
                itemSlots[x].SetClickable(message, 
                    ()=> {
                        Debug.LogFormat("purcharse: {0}", item_id);
                        playFabHelper.PurchaseItem(
                new PurchaseItemRequest
                {
                    ItemId = item_id,
                    Price = item_price,
                    VirtualCurrency = "GD"
                },
                (PurchaseItemResult result) =>
                {
                    FindObjectOfType<MessageWindow>().ShowSuccess("Purchase successful");
                },
                (PlayFabError err) =>
                {
                    FindObjectOfType<MessageWindow>().ShowSuccess(err.GenerateErrorReport());
                }
                );
                    },
                    "Yes");
            }
        }
    }


}
