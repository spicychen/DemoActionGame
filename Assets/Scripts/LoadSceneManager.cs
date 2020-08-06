using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : MonoBehaviour
{
    private int current_scene_index;
    // Start is called before the first frame update
    void Start()
    {
        current_scene_index = SceneManager.GetActiveScene().buildIndex;

    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(current_scene_index + 1);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
