using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MessageWindow: MonoBehaviour
{
    public GameObject message_window;
    public Text message;
    public Button ok_button;

    public GameObject success_window;
    public Text success_msg;
    public Button success_button;

    private void Start()
    {
        message_window.SetActive(false);
    }

    public void ShowMessage(string msg, System.Action button_callback=null, string button_text = "OK" )
    {
        message.text = msg;
        ok_button.gameObject.GetComponentInChildren<Text>().text = button_text;
        UnityAction on_click_function = () => { if(button_callback!=null)button_callback.Invoke(); message_window.SetActive(false); };
        ok_button.onClick.RemoveAllListeners();
        ok_button.onClick.AddListener(on_click_function);
        message_window.SetActive(true);
    }

    public void ShowSuccess(string msg, string button_text = "OK")
    {
        success_msg.text = msg;
        success_button.gameObject.GetComponentInChildren<Text>().text = button_text;
        success_button.onClick.RemoveAllListeners();
        success_button.onClick.AddListener(() => { success_window.SetActive(false); });
        success_window.SetActive(true);
    }

}
