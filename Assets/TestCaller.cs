
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCaller : MonoBehaviour
{
    public PlayFabHelper helper;

    // Start is called before the first frame update
    void Start()
    {
        helper.StartGame(
            5,
            "E059EE26FCE686BD",
            (ExecuteCloudScriptResult result) =>
            {
                Debug.Log(JsonUtility.ToJson(result, true));
                Debug.Log(result.FunctionResult);
            });
        //helper.PurchaseItem(
        //    new PurchaseItemRequest { 
        //        ItemId = "speed_up",
        //        Price = 30,
        //        VirtualCurrency = "GD"
        //    },
        //    (PurchaseItemResult result) =>
        //    {
        //        Debug.Log(JsonUtility.ToJson(result, true));
        //    });
        //helper.UseCharacterItem(
        //    "default_man01",
        //    "my man",
        //    (GrantCharacterToUserResult result) =>
        //    {
        //        Debug.Log(JsonUtility.ToJson(result, true));
        //    });
        //helper.GetPlayerInventory(
        //    (GetUserInventoryResult result) =>
        //    {
        //        Debug.Log(JsonUtility.ToJson(result, true));
        //    });
        //helper.GetPlayerCharacters(
        //    (ListUsersCharactersResult result) =>
        //    {
        //        Debug.Log(JsonUtility.ToJson(result, true));
        //    });
        //helper.GetPlayerProfile(
        //    (GetPlayerProfileResult result) =>
        //    {
        //        Debug.Log(JsonUtility.ToJson(result, true));
        //    });
        //helper.GetCatalogItems(
        //    (GetCatalogItemsResult result) =>
        //    {
        //        Debug.Log(JsonUtility.ToJson(result, true));
        //    });

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
