using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
using System;
[System.Serializable]
public class Item
{
    public string img_path;
    public string item_id;
    public string item_instance_id;
    public string name;
    public int price_gold;
    public int remaining_uses;

    public Item(string img_path, string item_id, string name, int price_gold)
    {
        this.img_path = img_path;
        this.item_id = item_id;
        this.name = name;
        this.price_gold = price_gold;
    }
    public Item(string img_path, string item_id, string name, string item_instance_id, int? remaining_uses)
    {
        this.img_path = img_path;
        this.item_id = item_id;
        this.name = name;
        this.item_instance_id = item_instance_id;
        this.remaining_uses = remaining_uses.HasValue?(int) remaining_uses:1;
    }

    public static List<Item> ConvertToItems(List<CatalogItem> items)
    {
        List<Item> converted_items = new List<Item>();
        if (items.Count > 0)
        {
            foreach (CatalogItem item in items)
            {
                var img_path = String.Format("ItemSprites/{0}", item.ItemId);
                int price_gold = 0;
                try
                {
                    price_gold = (int)item.VirtualCurrencyPrices["GD"];
                }
                catch (KeyNotFoundException)
                {
                    price_gold = 99999999;
                }

                converted_items.Add(new Item(img_path, item.ItemId, item.DisplayName, price_gold));
            }
        }

        return converted_items;
    }
    public static List<Item> ConvertToItems(List<ItemInstance> items)
    {
        List<Item> converted_items = new List<Item>();
        if (items.Count > 0)
        {
            foreach (ItemInstance item in items)
            {
                var img_path = String.Format("ItemSprites/{0}", item.ItemId);
                
                converted_items.Add(new Item(img_path, item.ItemId, item.DisplayName, item.ItemInstanceId, item.RemainingUses));
            }
        }

        return converted_items;
    }
}
