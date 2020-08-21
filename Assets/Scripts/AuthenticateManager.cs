using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using System;
using Facebook.Unity;
using LoginResult = PlayFab.ClientModels.LoginResult;

public enum Authtypes
{
    None,
    Silent,
    UsernameAndPassword,
    RegisterPlayFabAccount,
    Facebook
}

public class AuthenticateManager : MonoBehaviour
{
    public bool ClearPlayerPrefs;

    public InputField UsernameInput;
    public InputField PasswordInput;
    public InputField ConfirmPasswordInput;
    public InputField EmailInput;

    public Button LoginButton;
    public Button PlayAsGuestButton;
    public Button RegisterButton;
    public Button OpenLoginButton;
    public Button OpenFacebookLoginButton;

    public Text StatusText;

    public GameObject LoadingPanel;
    public GameObject StatusPanel;
    public GameObject LoginPanel;
    public GameObject RegisterPanel;

    private string Username;
    private string Password;
    private string Email;

    public bool ForceLink = false;

    public GetPlayerCombinedInfoRequestParams InfoRequestParams;

    public LoadSceneManager load_scene_manager;

    public static string PlayFabId { get { return _playFabId; } }
    private static string _playFabId;

    public static string SessionTicket { get { return _sessionTicket; } }
    private static string _sessionTicket;

    private const string _PlayFabRememberMeIdKey = "PlayFabIdPassGuid";
    private const string _PlayFabAuthTypeKey = "PlayFabAuthType";

    public Authtypes AuthType
    {
        get
        {
            return (Authtypes)PlayerPrefs.GetInt(_PlayFabAuthTypeKey, 0);
        }
        set
        {
            PlayerPrefs.SetInt(_PlayFabAuthTypeKey, (int)value);
        }
    }

    private string RememberMeId
    {
        get
        {
            return PlayerPrefs.GetString(_PlayFabRememberMeIdKey, "");
        }
        set
        {
            var guid = value ?? Guid.NewGuid().ToString();
            PlayerPrefs.SetString(_PlayFabRememberMeIdKey, guid);
        }
    }

    public void Awake()
    {
        if(ClearPlayerPrefs)
        {
            UnlinkSilentAuth();
            ClearRememberMe();
            AuthType = Authtypes.None;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadingPanel.SetActive(false);
        StatusPanel.SetActive(false);
        LoginPanel.SetActive(false);
        RegisterPanel.SetActive(false);

        PlayAsGuestButton.onClick.AddListener(OnPlayAsGuestClicked);
        OpenLoginButton.onClick.AddListener(OnOpenLoginClicked);
        LoginButton.onClick.AddListener(OnLoginClicked);
        RegisterButton.onClick.AddListener(OnRegisterButtonClicked);
        OpenFacebookLoginButton.onClick.AddListener(OnFacebookLoginButtonClicked);

        Authenticate();
    }

    private void OnPlayAsGuestClicked()
    {
        Authenticate(Authtypes.Silent);
    }

    private void OnOpenLoginClicked()
    {
        LoginPanel.SetActive(true);
    }

    private void OnLoginClicked()
    {
        Username = UsernameInput.text;
        Password = PasswordInput.text;
        Authenticate(Authtypes.UsernameAndPassword);
    }

    private void OnRegisterButtonClicked()
    {
        if(PasswordInput.text != ConfirmPasswordInput.text)
        {
            ShowStatus("Password do not Match.");
            return;
        }

        Username = UsernameInput.text;
        Password = PasswordInput.text;
        Email = EmailInput.text;
        Authenticate(Authtypes.RegisterPlayFabAccount);
    }

    private void OnFacebookLoginButtonClicked()
    {
        Authenticate(Authtypes.Facebook);
    }

    private void OnFacebookInitialized()
    {
        FB.LogInWithReadPermissions(null, AuthenticateFacebook);
    }


    public void Authenticate(Authtypes authType)
    {
        AuthType = authType;
        Authenticate();
    }

    public void Authenticate()
    {
        switch(AuthType)
        {
            case Authtypes.None:
                break;
            case Authtypes.Silent:
                SilentlyAuthenticate();
                break;
            case Authtypes.UsernameAndPassword:
                AuthenticateUsernameAndPassword();
                break;
            case Authtypes.RegisterPlayFabAccount:
                AddAccountAndPassword();
                break;
            case Authtypes.Facebook:

                if (!string.IsNullOrEmpty(RememberMeId))
                {
                    AuthenticateRememberMeId();
                }
                else if (!FB.IsInitialized)
                {
                    FB.Init(OnFacebookInitialized);
                }
                else
                {
                    OnFacebookInitialized();
                }
                break;
        }
    }

    private void SilentlyAuthenticate(System.Action<LoginResult> callback = null)
    {
        LoadingPanel.SetActive(true);

#if UNITY_ANDROID && !UNITY_EDITOR

    AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    //... not currently used
#elif UNITY_IPHONE || UNITY_IOS && !UNITY_EDITOR
    // code for iphone/ios

#else
        RememberMeId = SystemInfo.deviceUniqueIdentifier;

        PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest()
        {
            TitleId = PlayFabSettings.TitleId,
            CustomId = RememberMeId,
            CreateAccount = true,
            InfoRequestParameters = InfoRequestParams
        }, (result) => {
            _playFabId = result.PlayFabId;
            _sessionTicket = result.SessionTicket;

            if(callback == null)
            {
                OnLoginSuccess(result);
                LoadingPanel.SetActive(false);
            }
            else
            {
                LoadingPanel.SetActive(false);
                callback.Invoke(result);
            }

        }, (error)=>
        {
            if(callback == null)
            {

                LoadingPanel.SetActive(false);
            }
            else
            {
                LoadingPanel.SetActive(false);
                callback.Invoke(null);
                Debug.LogError(error.GenerateErrorReport());
            }

        });

#endif
    }

    private void AuthenticateRememberMeId()
    {

            PlayFabClientAPI.LoginWithCustomID(
                new LoginWithCustomIDRequest()
                {
                    TitleId = PlayFabSettings.TitleId,
                    CustomId = RememberMeId,
                    CreateAccount = true,
                    InfoRequestParameters = InfoRequestParams
                },

                (LoginResult result) =>
                {
                    LoadingPanel.SetActive(false);
                    _playFabId = result.PlayFabId;
                    _sessionTicket = result.SessionTicket;

                    OnLoginSuccess(result);
                },

                (PlayFabError error) =>
                {
                    LoadingPanel.SetActive(false);
                    OnPlayFabError(error);
                }
                );
    }

    private void AuthenticateUsernameAndPassword()
    {
        LoadingPanel.SetActive(true);
        if (!string.IsNullOrEmpty(RememberMeId))
        {
            AuthenticateRememberMeId();
            return;
        }

        if(string.IsNullOrEmpty(Username) && string.IsNullOrEmpty(Password))
        {
            ShowStatus("Please Fill in the Username and password.");
            LoadingPanel.SetActive(false);
            return;
        }

        PlayFabClientAPI.LoginWithPlayFab(
            new LoginWithPlayFabRequest()
            {
                TitleId = PlayFabSettings.TitleId,
                Username = Username,
                Password = Password,
                InfoRequestParameters = InfoRequestParams
            },

            (LoginResult result) =>
            {
                _playFabId = result.PlayFabId;
                _sessionTicket = result.SessionTicket;

                RememberMeId = Guid.NewGuid().ToString();
                AuthType = Authtypes.UsernameAndPassword;

                PlayFabClientAPI.LinkCustomID(
                    new LinkCustomIDRequest
                    {
                        CustomId = RememberMeId,
                        ForceLink = ForceLink,
                    },
                    null,
                    null
                    );

                OnLoginSuccess(result);
                LoadingPanel.SetActive(false);
            },

            (PlayFabError error) =>
            {

                LoadingPanel.SetActive(false);
                OnPlayFabError(error);
            }
            );
    }

    private void AddAccountAndPassword()
    {
        SilentlyAuthenticate(
            (LoginResult result) =>
            {
                if(result == null)
                {
                    OnPlayFabError(new PlayFabError() { Error = PlayFabErrorCode.UnknownError, ErrorMessage = "Silent authentication by device failed" });
                    return;
                }

                PlayFabClientAPI.AddUsernamePassword(
                    new AddUsernamePasswordRequest()
                    {
                        Username = Username,
                        Email = Email,
                        Password = Password,
                    },
                    (AddUsernamePasswordResult addResult) =>
                    {
                        _playFabId = result.PlayFabId;
                        _sessionTicket = result.SessionTicket;

                        //RememberMeId = Guid.NewGuid().ToString();

                        PlayFabClientAPI.LinkCustomID(
                            new LinkCustomIDRequest()
                            {
                                CustomId = RememberMeId,
                                ForceLink = ForceLink
                            },
                            null,
                            null
                            );

                        AuthType = Authtypes.UsernameAndPassword;

                        OnLoginSuccess(result);
                    },

                    (PlayFabError error) =>
                    {
                        OnPlayFabError(error);
                    }

                    );


            }
            );
    }

    private void AuthenticateFacebook(ILoginResult i_result)
    {

        if (i_result == null || string.IsNullOrEmpty(i_result.Error))
        {
            SilentlyAuthenticate(
            (LoginResult result) =>
            {

                if (result == null)
                {
                    OnPlayFabError(new PlayFabError() { Error = PlayFabErrorCode.UnknownError, ErrorMessage = "Silent authentication by device failed" });
                    return;
                }

                PlayFabClientAPI.LoginWithFacebook(new LoginWithFacebookRequest
                {
                    CreateAccount = true,
                    AccessToken = AccessToken.CurrentAccessToken.TokenString
                },
                (LoginResult loginResult) =>
                {
                    _playFabId = result.PlayFabId;
                    _sessionTicket = result.SessionTicket;

                    //RememberMeId = Guid.NewGuid().ToString();

                    PlayFabClientAPI.LinkCustomID(
                        new LinkCustomIDRequest()
                        {
                            CustomId = RememberMeId,
                            ForceLink = ForceLink
                        },
                        null,
                        null
                        );

                    AuthType = Authtypes.Facebook;

                    OnLoginSuccess(result);
                },
                OnPlayFabError
                );
            }
            );
        }
        else
        {
            ShowStatus(i_result.Error);
        }
    }

    private void OnLoginSuccess(PlayFab.ClientModels.LoginResult result)
    {
        Debug.LogFormat("Logged In as: {0}", result.PlayFabId);
        if(AuthType == Authtypes.Silent)
        AuthType = Authtypes.None;
        load_scene_manager.GoToMainMenu();
    }

    private void OnPlayFabError(PlayFabError error)
    {
        switch (error.Error)
        {
            //case PlayFabErrorCode.InvalidEmailOrPassword:
            //case PlayFabErrorCode.InvalidPassword:
            //case PlayFabErrorCode.InvalidEmailAddress:
            //    ShowStatus("Invalid Email or Password");
            //    break;
            case PlayFabErrorCode.InvalidUsername:
            case PlayFabErrorCode.InvalidUsernameOrPassword:
            case PlayFabErrorCode.InvalidPassword:
                ShowStatus("Invalid Username or Password");
                break;
            case PlayFabErrorCode.AccountNotFound:
                RegisterPanel.SetActive(true);
                return;
            default:
                ShowStatus(error.GenerateErrorReport());
                break;

        }
    }

    private void ShowStatus(string status)
    {
        StatusText.text = status;
        StatusPanel.SetActive(true);
    }

    public void UnlinkSilentAuth()
    {
        SilentlyAuthenticate(
            (result) =>
            {
                PlayFabClientAPI.UnlinkCustomID(new UnlinkCustomIDRequest()
                {
                    CustomId = SystemInfo.deviceUniqueIdentifier
                }, null, null);
            }
            );
    }

    public void ClearRememberMe()
    {
        PlayerPrefs.DeleteKey(_PlayFabAuthTypeKey);
        PlayerPrefs.DeleteKey(_PlayFabRememberMeIdKey);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
