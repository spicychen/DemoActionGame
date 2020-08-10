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

    public void LoadNextScene(int next_i)
    {
        SceneManager.LoadScene(current_scene_index + next_i);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void GoToLoginScene()
    {
        SceneManager.LoadScene("LoginScene");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
