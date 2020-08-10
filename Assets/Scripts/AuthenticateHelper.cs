
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class AuthenticateHelper : MonoBehaviour
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

    public void AuthenticateRememberMeId()
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
                Debug.LogFormat("Logged In as: {0}", result.PlayFabId);
            },

            (PlayFabError error) =>
            {
                LoadSceneManager.GoToLoginScene();
            }
            );
    }
}
