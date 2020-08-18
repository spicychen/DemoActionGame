using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProfile : MonoBehaviour
{
    public string player_name;
    public string player_avatar_url;
    public int player_level;
    public List<Item> player_inventory;
    public List<Character> player_characters;
    public int player_fighting_points
    {
        get
        {
            return _player_fighting_points;
        }
        set
        {
            _player_fighting_points = value;
            player_fighting_points_ui.text = _player_fighting_points.ToString();
        }
    }
    private int _player_fighting_points;

    public int player_gold { get {
            return _player_gold;
        } 
    set
        {
            _player_gold = value;
            player_gold_ui.text = _player_gold.ToString();
        }
    }
    private int _player_gold;

    public ItemSlot[] itemSlots;
    public ItemSlot[] characterSlots;

    public GameObject item_slot_group;
    public GameObject character_slot_group;

    public Button show_item_button;
    public Button show_character_button;

    public Text player_fighting_points_ui;
    public Text player_gold_ui;

    public PlayFabHelper playFabHelper;

    private void Start()
    {
        ShowItems();
        show_item_button.onClick.AddListener(ShowItems);
        show_character_button.onClick.AddListener(ShowCharacters);
    }

    public void SetItems()
    {
        bool current_active = item_slot_group.activeSelf;
        item_slot_group.SetActive(true);
        if (player_inventory.Count <= itemSlots.Length)
        {
            for (int x = 0; x < itemSlots.Length; x++)
            {
                if(x< player_inventory.Count)
                {

                itemSlots[x].SetSlotItem(player_inventory[x].img_path, player_inventory[x].name, player_inventory[x].remaining_uses);
                }
                else
                {
                    itemSlots[x].HideSlotItem();
                }

                //switch (player_inventory[x].item_id)
                //{
                //    case "default_man01":
                //    case "powerful_man01":

                //        string item_id = player_inventory[x].item_id;
                //        string message = string.Format("Are you sure you want to use {0}", player_inventory[x].name);
                //        ItemSlot current_slot = itemSlots[x];
                //        current_slot.SetClickable(message,()=>
                //        {
                //            playFabHelper.UseCharacterItem(
                //                item_id,
                //                item_id,
                //                (GrantCharacterToUserResult result) =>
                //                {
                //                    FindObjectOfType<MessageWindow>().ShowSuccess("Redeem successful");
                //                    current_slot.gameObject.SetActive(false);
                //                }
                //                );
                //        });
                //        break;
                //}
            }
        }
        item_slot_group.SetActive(current_active);
    }

    public void RedeemCharacters(string character_id)
    {
        playFabHelper.UseCharacterItem(
                                character_id,
                                character_id,
                                (GrantCharacterToUserResult g_result) =>
                                {
                                    FindObjectOfType<MessageWindow>().ShowSuccess("Redeem successful");

                                    playFabHelper.GetPlayerCombinedInfo((GetPlayerCombinedInfoResult result) =>
                                    {
                                        //update player inventory
                                        this.player_inventory = Item.ConvertToItems(result.InfoResultPayload.UserInventory);
                                        this.SetItems();
                                        this.player_fighting_points = result.InfoResultPayload.UserVirtualCurrency["FP"];
                                        this.player_gold = result.InfoResultPayload.UserVirtualCurrency["GD"];

                                        //update player characters

                                        this.player_characters = Character.ConvertToCharacters(result.InfoResultPayload.CharacterList);
                                        this.SetCharacters();
                                    },
                                    (PlayFabError err) =>
                                    {
                                        FindObjectOfType<MessageWindow>().ShowSuccess(err.GenerateErrorReport());
                                    }
                                    );
                                },
                                (PlayFabError err) =>
                                {
                                    FindObjectOfType<MessageWindow>().ShowSuccess("You do not Have This character!","Fine");
                                }
                                );

    }

    public void SetCharacters()
    {
        bool current_active = character_slot_group.activeSelf;
        character_slot_group.SetActive(true);
        if (player_characters.Count <= characterSlots.Length)
        {
            for (int x = 0; x < characterSlots.Length; x++)
            {
                if(x< player_characters.Count)
                {

                characterSlots[x].SetSlotItem(player_characters[x].img_path, player_characters[x].character_type, 1);
                }
                else
                {
                    characterSlots[x].HideSlotItem();
                }
            }
        }
        character_slot_group.SetActive(current_active);
    }

    private void ShowItems()
    {
        character_slot_group.SetActive(false);
        item_slot_group.SetActive(true);
    }

    private void ShowCharacters()
    {
        item_slot_group.SetActive(false);
        character_slot_group.SetActive(true);
    }
}
