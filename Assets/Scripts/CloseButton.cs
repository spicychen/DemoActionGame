using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloseButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Button close_button = GetComponent<Button>();
        if (close_button)
        {
            close_button.onClick.AddListener(CloseParent);
        }

    }

    private void CloseParent()
    {
        transform.parent.gameObject.SetActive(false);
    }

}
