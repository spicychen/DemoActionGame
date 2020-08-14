using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGameManager : MonoBehaviour
{
    public PlayFabHelper playFabHelper;

    public Text score_ui;

    // Start is called before the first frame update
    void Start()
    {
        if (PersistentManagerScript.Instance)
        {

            playFabHelper.CompleteGame(
                PersistentManagerScript.Instance.current_level,
                PersistentManagerScript.Instance.score,
                PersistentManagerScript.Instance.item_used,
                (ExecuteCloudScriptResult result) =>
                {
                    Debug.Log(JsonUtility.ToJson(result, true));
                    Debug.Log(result.FunctionResult);
                    CloudScriptResult custom_result = JsonUtility.FromJson<CloudScriptResult>(result.FunctionResult.ToString());

                    if (custom_result.status == "Success")
                    {
                        FindObjectOfType<MessageWindow>().ShowSuccess(custom_result.message);
                    }
                    else
                    {
                        FindObjectOfType<MessageWindow>().ShowSuccess(custom_result.message, "OH NO!");
                    }
                });

            score_ui.text = PersistentManagerScript.Instance.score.ToString();
        }
    }

}
