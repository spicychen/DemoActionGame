using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using System.Text;
using UnityEngine.Networking;
using UnityEditor.PackageManager.Requests;

public class MenuManager : MonoBehaviour
{

    public Button start_game;
    public Button[] level_selection;

    public Button shop_button;
    public Button leaderboard_button;
    public Button expand_button;
    public Button shrink_button;
    public InputField convert_amount;
    public Button convert_button;

    public Button open_gift;

    public GameObject levels_panel;
    public GameObject characters_panel;
    public GameObject shop_panel;
    public GameObject leaderboard_panel;

    public Animator player_profile_animator;

    public Image player_avatar;
    public Text player_name;

    public PlayerProfile player_profile;
    public LoadSceneManager load_scene_manager;
    public PlayFabHelper playFabHelper;
    public CharacterSelection characterSelection;

    public Shop shop;
    public Leaderboard leaderboard;

    // Start is called before the first frame update
    void Start()
    {
        convert_button.onClick.AddListener(ConvertFPToGD);
        open_gift.onClick.AddListener(OnOpenGiftClicked);

        open_gift.gameObject.SetActive(false);
        levels_panel.SetActive(false);
        characters_panel.SetActive(false);
        shop_panel.SetActive(false);
        leaderboard_panel.SetActive(false);

        start_game.onClick.AddListener(OnStartGameClicked);
        for(int i=0; i<level_selection.Length; i++)
        {
            int x = i;
            level_selection[x].onClick.AddListener(()=> { OnLevelButtonClicked(x+1);Debug.Log(x); });
            
        }

        shop_button.onClick.AddListener(OnShopButtonClicked);
        leaderboard_button.onClick.AddListener(OnLeaderboardButtonClicked);
        expand_button.onClick.AddListener(OnExpandClicked);
        shrink_button.onClick.AddListener(OnShrinkClicked);

        OnMainMenuLoad();
    }

    private void OnMainMenuLoad()
    {
        //playFabHelper.GetPlayerProfile((GetPlayerProfileResult result) => {
        //    StartCoroutine(SetImage(result.PlayerProfile.AvatarUrl));
        //    player_name.text = result.PlayerProfile.DisplayName;
        //    player_profile.player_name = result.PlayerProfile.DisplayName;
        //    player_profile.player_avatar_url = result.PlayerProfile.AvatarUrl;
        //});

        playFabHelper.GetPlayerCombinedInfo((GetPlayerCombinedInfoResult result) =>
        {
            //update avatar and name
            StartCoroutine(SetImage(result.InfoResultPayload.PlayerProfile.AvatarUrl));
            player_name.text = result.InfoResultPayload.PlayerProfile.DisplayName;
            player_profile.player_name = result.InfoResultPayload.PlayerProfile.DisplayName;
            player_profile.player_avatar_url = result.InfoResultPayload.PlayerProfile.AvatarUrl;

            foreach( ItemInstance i_i in result.InfoResultPayload.UserInventory)
            {
                if(i_i.ItemId == "newbee_gift")
                {
                    open_gift.gameObject.SetActive(true);
                    break;
                }
            }
        },
        (PlayFabError err) =>
        {
            FindObjectOfType<MessageWindow>().ShowSuccess(err.GenerateErrorReport());
        }
        );
    }

    IEnumerator SetImage(string url)
    {
        //WWW www = new WWW(url);
        //yield return www;


        //www.LoadImageIntoTexture(texture);
        //player_avatar.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        ////player_avatar.material.mainTexture = www.texture;
        ////www.LoadImageIntoTexture(player_avatar);
        //www.Dispose();
        //www = null;
        if (!string.IsNullOrEmpty(url))
        {

            UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(url);
            yield return unityWebRequest.SendWebRequest();
            if(unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
            {
                Debug.LogError(unityWebRequest.error);
            }
            else
            {
                Texture2D texture = ((DownloadHandlerTexture)unityWebRequest.downloadHandler).texture;
                player_avatar.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
        }
    }

    private void OnOpenGiftClicked()
    {

        playFabHelper.OpenGift((UnlockContainerItemResult result) =>
        {
            StringBuilder sb = new StringBuilder("You Received: ",200);
            foreach(ItemInstance i_i in result.GrantedItems)
            {
                sb.AppendFormat("\n{0} ",i_i.DisplayName);
            }
            open_gift.gameObject.SetActive(false);
            FindObjectOfType<MessageWindow>().ShowSuccess(sb.ToString());
        },
        (PlayFabError err) =>
        {
            FindObjectOfType<MessageWindow>().ShowSuccess(err.GenerateErrorReport());
        }
        );
    }

    private void OnExpandClicked()
    {
        playFabHelper.GetPlayerCombinedInfo((GetPlayerCombinedInfoResult result) =>
        {
            //update avatar and name
            StartCoroutine(SetImage(result.InfoResultPayload.PlayerProfile.AvatarUrl));
            player_name.text = result.InfoResultPayload.PlayerProfile.DisplayName;
            player_profile.player_name = result.InfoResultPayload.PlayerProfile.DisplayName;
            player_profile.player_avatar_url = result.InfoResultPayload.PlayerProfile.AvatarUrl;

            //update player inventory
            player_profile.player_inventory = Item.ConvertToItems(result.InfoResultPayload.UserInventory);
            player_profile.SetItems();
            player_profile.player_fighting_points = result.InfoResultPayload.UserVirtualCurrency["FP"];
            player_profile.player_gold = result.InfoResultPayload.UserVirtualCurrency["GD"];
            player_profile_animator.SetTrigger("drag");

            //update player characters

            player_profile.player_characters = Character.ConvertToCharacters(result.InfoResultPayload.CharacterList);
            player_profile.SetCharacters();
        },
        (PlayFabError err) =>
        {
            FindObjectOfType<MessageWindow>().ShowSuccess(err.GenerateErrorReport());
        }
        );
        //playFabHelper.GetPlayerInventory(
        //    (GetUserInventoryResult result) =>
        //    {
        //        player_profile.player_inventory = Item.ConvertToItems(result.Inventory);
        //        player_profile.SetItems();
        //        player_profile.player_fighting_points = result.VirtualCurrency["FP"];
        //        player_profile.player_gold = result.VirtualCurrency["GD"];
        //        player_profile_animator.SetTrigger("drag");
        //    }
        //    );
        //playFabHelper.GetPlayerCharacters(
        //    (ListUsersCharactersResult result) =>
        //    {
        //        player_profile.player_characters = Character.ConvertToCharacters(result.Characters);
        //        player_profile.SetCharacters();
        //    }
        //    );
    }

    private void OnShrinkClicked()
    {
        player_profile_animator.SetTrigger("drag");
    }

    private void OnStartGameClicked()
    {
        levels_panel.SetActive(true);
    }

    private void OnLevelButtonClicked(int level)
    {
        PersistentManagerScript.Instance.current_level = level;

        characters_panel.SetActive(true);
        playFabHelper.GetPlayerCharacters(
            (ListUsersCharactersResult result) =>
            {
                player_profile.player_characters = Character.ConvertToCharacters(result.Characters);
                player_profile.SetCharacters();
                characterSelection.SetItems();
            }
            );


        playFabHelper.GetPlayerInventory(
            (GetUserInventoryResult result) =>
            {
                PersistentManagerScript.Instance.player_inventory = Item.ConvertToItems(result.Inventory);

            });
    }

    //private void OnCharacterButtonClicked(int i_character)
    //{
    //    PersistentManagerScript.Instance.selected_character = player_profile.player_characters[i_character];
    //    load_scene_manager.LoadNextScene(PersistentManagerScript.Instance.current_level);

    //}

    private void OnShopButtonClicked()
    {
        
        playFabHelper.GetCatalogItems(
        (GetCatalogItemsResult result) =>
        {
            shop.SetItems(result.Catalog);
        }
        ) ;
    }

    private void OnLeaderboardButtonClicked()
    {
        playFabHelper.GetDailyAndWeeklyLeaderboard(
            (GetLeaderboardResult result,GetLeaderboardResult w_result) =>
            {
                leaderboard.SetRows(result.Leaderboard, w_result.Leaderboard);
            }
            );
    }

    public void ConvertFPToGD()
    {
        playFabHelper.ConvertFPToGD(int.Parse(convert_amount.text), (ExecuteCloudScriptResult result) =>
        {
            CloudScriptResult custom_result = JsonUtility.FromJson<CloudScriptResult>(result.FunctionResult.ToString());
            //Debug.Log(custom_result.status);
            //Debug.Log(custom_result.message);
            //Debug.Log(result.FunctionResult.GetType());
            //Debug.Log(result.FunctionResult.ToString());
            //Debug.Log(JsonUtility.FromJson<CloudScriptResult>(result.FunctionResult.ToString()));
            if (custom_result.status == "Success")
            {
                FindObjectOfType<MessageWindow>().ShowSuccess(custom_result.message);
            }else if(custom_result.status == "NotEnoughFP")
            {
                FindObjectOfType<MessageWindow>().ShowSuccess(custom_result.message);
            }
        });
    }
}
