﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class PlayFabHelper : MonoBehaviour
{
    public LoadSceneManager LoadSceneManager;

    public GetPlayerCombinedInfoRequestParams InfoRequestParams;

    private const string _PlayFabRememberMeIdKey = "PlayFabIdPassGuid";
    private string RememberMeId
    {
        get
        {
            return PlayerPrefs.GetString(_PlayFabRememberMeIdKey, "");
        }
    }

    public void AuthenticateRememberMeId(System.Action<LoginResult> callback = null)
    {

        PlayFabClientAPI.LoginWithCustomID(
            new LoginWithCustomIDRequest()
            {
                TitleId = PlayFabSettings.TitleId,
                CustomId = RememberMeId,
                CreateAccount = false,
                InfoRequestParameters = InfoRequestParams
            },

            (LoginResult result) =>
            {
                Debug.LogFormat("CustomId:{0}", RememberMeId);
                Debug.LogFormat("Logged In as: {0}", result.PlayFabId);
                if(callback != null)
                {
                    callback.Invoke(result);
                }
            },

            (PlayFabError error) =>
            {
                LoadSceneManager.GoToLoginScene();
            }
            );
    }

    public void ExecuteAfterAuthenticate(System.Action callback = null)
    {
        if (!PlayFabClientAPI.IsClientLoggedIn())
        {
            AuthenticateRememberMeId(
                (LoginResult result) =>
                {
                    if (callback != null)
                    {
                        callback.Invoke();
                    }
                });
        }
        else
        {
            callback.Invoke();
        }
    }

    public void GetCatalogItems(System.Action<GetCatalogItemsResult> callback = null)
    {
        ExecuteAfterAuthenticate(
            () =>
            {
                    PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest
                    {
                        CatalogVersion = "main"
                    },
                    (GetCatalogItemsResult result) =>
                    {
                        if(callback != null)
                        {
                            callback.Invoke(result);
                        }
                    },
                    LogError
                    );
            }
            );
        
    }

    public void GetDailyAndWeeklyLeaderboard(System.Action<GetLeaderboardResult, GetLeaderboardResult> callback = null)
    {
        ExecuteAfterAuthenticate(() => { 
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
                            if (callback != null)
                            {
                                callback.Invoke(result,w_result);
                            }
                        },
                        LogError

                        );
                    },
                    LogError

                    );
        });
    }

    public void GetPlayerCombinedInfo(System.Action<GetPlayerCombinedInfoResult> callback = null, System.Action<PlayFabError> error_callback = null)
    {
        ExecuteAfterAuthenticate(() =>
        {
            PlayFabClientAPI.GetPlayerCombinedInfo(new GetPlayerCombinedInfoRequest
            {
                InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
                {
                    //GetUserAccountInfo = true,
                    GetPlayerProfile = true,
                    GetUserInventory = true,
                    GetUserVirtualCurrency = true,
                    GetCharacterList = true,
                    ProfileConstraints = new PlayerProfileViewConstraints
                    {
                        ShowAvatarUrl = true,
                        ShowDisplayName = true
                    }
                }
            },
            (GetPlayerCombinedInfoResult result) =>
            {
                if(callback!=null)
                callback.Invoke(result);
            },
            (PlayFabError err) =>
            {
                if (error_callback != null)
                {
                    error_callback.Invoke(err);
                }
                LogError(err);
            }
            );
        });
    }
    

    public void ChangeAvatar(string url, System.Action callback = null, System.Action<PlayFabError> error_callback = null)
    {
        ExecuteAfterAuthenticate(() =>
        {
            PlayFabClientAPI.UpdateAvatarUrl(new UpdateAvatarUrlRequest
            {
                ImageUrl = url
            },
            (EmptyResponse result) =>
            {
                if (callback != null)
                    callback.Invoke();
            },
            (PlayFabError err) =>
            {
                if (error_callback != null)
                {
                    error_callback.Invoke(err);
                }
                LogError(err);
            }
            );
        });
    }
    

    public void ChangeName(string name, System.Action<UpdateUserTitleDisplayNameResult> callback = null, System.Action<PlayFabError> error_callback = null)
    {
        ExecuteAfterAuthenticate(() =>
        {
            PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
            {
                DisplayName = name
            },
            (UpdateUserTitleDisplayNameResult result) =>
            {
                if (callback != null)
                    callback.Invoke(result);
            },
            (PlayFabError err) =>
            {
                if (error_callback != null)
                {
                    error_callback.Invoke(err);
                }
                LogError(err);
            }
            );
        });
    }

    public void OpenGift(System.Action<UnlockContainerItemResult> callback = null, System.Action<PlayFabError> error_callback = null)
    {
        ExecuteAfterAuthenticate(() =>
        {
            PlayFabClientAPI.UnlockContainerItem(new UnlockContainerItemRequest
            {
                ContainerItemId = "newbee_gift"
            },
            (UnlockContainerItemResult result) =>
            {
                if (callback != null)
                    callback.Invoke(result);
            },
            (PlayFabError err) =>
            {
                if (error_callback != null)
                {
                    error_callback.Invoke(err);
                }
                LogError(err);
            }
            );
        });
    }

    public void GetPlayerProfile(System.Action<GetPlayerProfileResult> callback = null)
    {
        ExecuteAfterAuthenticate(() =>
        {
            //Debug.Log("test");
            PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest
            {
                ProfileConstraints = new PlayerProfileViewConstraints
                {
                    ShowAvatarUrl = true,
                    ShowDisplayName = true
                }
            },
            (GetPlayerProfileResult result) =>
            {
                if (callback != null)
                {
                    callback.Invoke(result);
                }
            },
            LogError
            );
        });
    }

    public void GetPlayerInventory(System.Action<GetUserInventoryResult> callback = null)
    {
        ExecuteAfterAuthenticate(() =>
        {
            PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest { },
                (GetUserInventoryResult result) =>
                {
                    if (callback != null)
                    {
                        callback.Invoke(result);
                    }
                },
                LogError
                );
        });
    }
    public void GetPlayerCharacters(System.Action<ListUsersCharactersResult> callback = null)
    {
        ExecuteAfterAuthenticate(() =>
        {
            PlayFabClientAPI.GetAllUsersCharacters(new ListUsersCharactersRequest { },
                (ListUsersCharactersResult result) =>
                {
                    if (callback != null)
                    {
                        callback.Invoke(result);
                    }
                },
                LogError
                );
        });
    }


    public void PurchaseItem(PurchaseItemRequest request, System.Action<PurchaseItemResult> callback = null, System.Action<PlayFabError> error_callback = null)
    {
        ExecuteAfterAuthenticate(() =>
        {
            PlayFabClientAPI.PurchaseItem(request,
            (PurchaseItemResult result)=>
            {
                if (callback != null)
                {
                    callback.Invoke(result);
                }
            },

            (PlayFabError err) =>
            {
                if (error_callback != null)
                {
                    error_callback.Invoke(err);
                }
                LogError(err);
            }
            );
        });
    }

    public void UseCharacterItem(string item_id, string character_name, System.Action<GrantCharacterToUserResult> callback, System.Action<PlayFabError> error_callback=null)
    {
        ExecuteAfterAuthenticate(() =>
        {
            PlayFabClientAPI.GrantCharacterToUser(
                new GrantCharacterToUserRequest
                {
                    CharacterName = character_name,
                    ItemId = item_id
                },
                (GrantCharacterToUserResult result) =>
                {
                    if (callback != null)
                    {
                        callback.Invoke(result);
                    }

                },
            (PlayFabError err) =>
            {
                if (error_callback != null)
                {
                    error_callback.Invoke(err);
                }
                    LogError(err);
            }
                );
        });
    }

    public void StartGame(int level, string character_id, System.Action<ExecuteCloudScriptResult> callback = null)
    {
        ExecuteAfterAuthenticate(() =>
        {
            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
            {
                FunctionName = "startActionGame",
                FunctionParameter =
                new
                {
                    currentPlayLevel = level,
                    currentCharacter = character_id,
                }
            },
            (ExecuteCloudScriptResult result) =>
            {
                if(callback != null)
                {
                    callback.Invoke(result);
                }
            },
                LogError
            );
        });
    }

    public void CompleteGame(int level, int finalScore, Dictionary<string,int> item_used, System.Action<ExecuteCloudScriptResult> callback = null)
    {

        ExecuteAfterAuthenticate(() =>
        {
            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
            {

                FunctionName = "completeActionGame",
                FunctionParameter =
                new
                {
                    currentPlayLevel = level,
                    score = finalScore,
                    usedItems = item_used,
                }
            },
            (ExecuteCloudScriptResult result) =>
            {
                if (callback != null)
                {
                    callback.Invoke(result);
                }
            },
            LogError
            );
        });
    }

    public void ConvertFPToGD(int fightingPoint, System.Action<ExecuteCloudScriptResult> callback = null)
    {

        ExecuteAfterAuthenticate(() =>
        {
            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
            {

                FunctionName = "convertFPToGD",
                FunctionParameter =
                new
                {
                    FightingPoint = fightingPoint
                }
            },
            (ExecuteCloudScriptResult result) =>
            {
                if (callback != null)
                {
                    callback.Invoke(result);
                }
            },
            LogError
            );
        });
    }

    private void LogError(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }
}
