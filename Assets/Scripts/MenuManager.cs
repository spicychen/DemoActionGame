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
    public Button[] character_selection;

    public Button shop_button;
    public Button leaderboard_button;

    public GameObject levels_panel;
    public GameObject characters_panel;
    public GameObject shop_panel;
    public GameObject leaderboard_panel;

    public PlayerProfile player_profile;
    public LoadSceneManager load_scene_manager;
    public AuthenticateHelper AuthenticateHelper;
    public PlayFabHelper playFabHelper;

    public Shop shop;
    public Leaderboard leaderboard;

    // Start is called before the first frame update
    void Start()
    {
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
        for(int i=0; i < character_selection.Length; i++)
        {
            int x = i;
            character_selection[x].onClick.AddListener(()=> { OnCharacterButtonClicked(x);Debug.LogFormat("charx: {0}", x); }) ;

        }

        shop_button.onClick.AddListener(OnShopButtonClicked);
        leaderboard_button.onClick.AddListener(OnLeaderboardButtonClicked);


    }

    private void OnMainMenuLoad()
    {

    }

    private void OnStartGameClicked()
    {
        levels_panel.SetActive(true);
    }

    private void OnLevelButtonClicked(int level)
    {
        PersistentManagerScript.Instance.current_level = level;
        characters_panel.SetActive(true);
    }

    private void OnCharacterButtonClicked(int i_character)
    {
        //Debug.Log(i_character);
        //Debug.Log(PersistentManagerScript.Instance.current_level);
        PersistentManagerScript.Instance.selected_character = player_profile.player_characters[i_character];
        load_scene_manager.LoadNextScene(PersistentManagerScript.Instance.current_level);

    }

    private void OnShopButtonClicked()
    {
        if (!PlayFabClientAPI.IsClientLoggedIn())
        {
            AuthenticateHelper.AuthenticateRememberMeId();
            return;
        }
        PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest
        {
            CatalogVersion = "main"
        },
        (GetCatalogItemsResult result) =>
        {
            shop.SetItems(result.Catalog);
            //shop.ShowItems();
        },
        (PlayFabError error) =>
        {
            Debug.Log(error.GenerateErrorReport());
        }
        ) ;
    }

    private void OnLeaderboardButtonClicked()
    {

        if (!PlayFabClientAPI.IsClientLoggedIn())
        {
            AuthenticateHelper.AuthenticateRememberMeId();
            return;
        }
        PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest
        {
            StatisticName = "DailyScore",
            MaxResultsCount = 7
        },
        (GetLeaderboardResult result) =>
        {
            PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest
            {
                StatisticName = "WeeklyScore",
                MaxResultsCount = 7
            },
            (GetLeaderboardResult w_result) =>
            {
                leaderboard.SetRows(result.Leaderboard, w_result.Leaderboard);
            },
            (PlayFabError w_error) =>
            {
                Debug.Log(w_error.GenerateErrorReport());

            }

            );
        },
        (PlayFabError error) =>
        {
            Debug.Log(error.GenerateErrorReport());
        }
        
        );
    }

}
