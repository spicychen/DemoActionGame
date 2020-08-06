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
    public Button leaderboard;

    public GameObject levels_panel;
    public GameObject characters_panel;
    public GameObject shop_panel;

    public PlayerProfile player_profile;
    public LoadSceneManager load_scene_manager;

    public Shop shop;

    // Start is called before the first frame update
    void Start()
    {
        levels_panel.SetActive(false);
        characters_panel.SetActive(false);
        shop_panel.SetActive(false);

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
        PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest
        {
            CatalogVersion = "main"
        },
        (GetCatalogItemsResult result) =>
        {
            shop.SetItems(result.Catalog);
            shop.ShowItems();
        },
        (PlayFabError error) =>
        {
            Debug.Log(error.GenerateErrorReport());
        }
        ) ;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
