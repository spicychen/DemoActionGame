﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{

    public Image item_img;
    public Text item_name;
    public Text item_price;

    public bool is_sell;

    // Start is called before the first frame update
    void Awake()
    {
        HideSlotItem();
    }

    public void HideSlotItem()
    {
        //is_sell = false;
        //item_img.gameObject.SetActive(false);
        //item_name.gameObject.SetActive(false);
        //item_price.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void ShowSlotItem()
    {
        //is_sell = true;
        //item_img.gameObject.SetActive(true);
        //item_name.gameObject.SetActive(true);
        //item_price.gameObject.SetActive(true);
        gameObject.SetActive(true);
    }

    public void SetSlotItem(string image_path, string item_name, int item_price)
    {
        this.item_img.sprite = Resources.Load<Sprite>(image_path);
        this.item_name.text = item_name;
        this.item_price.text = item_price.ToString();
        ShowSlotItem();
    }

}