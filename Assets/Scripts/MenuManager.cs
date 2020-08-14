using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

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
        playFabHelper.GetPlayerProfile((GetPlayerProfileResult result) => {
            StartCoroutine(SetImage(result.PlayerProfile.AvatarUrl));
            player_name.text = result.PlayerProfile.DisplayName;
            player_profile.player_name = result.PlayerProfile.DisplayName;
            player_profile.player_avatar_url = result.PlayerProfile.AvatarUrl;
        });
    }

    IEnumerator SetImage(string url)
    {
        WWW www = new WWW(url);
        yield return www;

        Texture2D texture = new Texture2D(www.texture.width, www.texture.height, TextureFormat.DXT1, false);
        www.LoadImageIntoTexture(texture);
        player_avatar.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        //player_avatar.material.mainTexture = www.texture;
        //www.LoadImageIntoTexture(player_avatar);
        www.Dispose();
        www = null;
    }

    private void OnExpandClicked()
    {
        playFabHelper.GetPlayerInventory(
            (GetUserInventoryResult result) =>
            {
                player_profile.player_inventory = Item.ConvertToItems(result.Inventory);
                player_profile.SetItems();
                player_profile.player_fighting_points = result.VirtualCurrency["FP"];
                player_profile.player_gold = result.VirtualCurrency["GD"];
                player_profile_animator.SetTrigger("drag");
            }
            );
        playFabHelper.GetPlayerCharacters(
            (ListUsersCharactersResult result) =>
            {
                player_profile.player_characters = Character.ConvertToCharacters(result.Characters);
                player_profile.SetCharacters();
            }
            );
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
