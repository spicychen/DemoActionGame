using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public int time_limit_in_seconds;
    public Text timerText;

    public int score;
    public Text score_ui;
    public Text level_ui;
    public Text player_health;

    public Text player_health_ui;

    public EnemyController enemy01;

    public GameObject character01;
    public GameObject character02;

    private GameObject active_chatacter;
    private CustomCC.CharaterController active_character_controller;

    private void Start()
    {
        StartCoroutine(Timer(time_limit_in_seconds));
        character01.SetActive(false);
        character02.SetActive(false);
        score = 0;

        active_chatacter = character01;

        if (PersistentManagerScript.Instance)
        {

            level_ui.text = PersistentManagerScript.Instance.current_level.ToString();

            PersistentManagerScript.Instance.score = score;
            PersistentManagerScript.Instance.item_used = new List<string>();

            switch (PersistentManagerScript.Instance.selected_character.character_type)
            {
                case "default_man01":
                    active_chatacter = character01;
                    break;
                case "powerful_man01":
                    active_chatacter = character02;
                    break;
                default:
                    active_chatacter = character01;
                    break;
            }
        }
        active_chatacter.SetActive(true);
        active_character_controller = active_chatacter.GetComponent<CustomCC.CharaterController>();
        enemy01.player = active_chatacter.transform;
        enemy01.player_animator = active_chatacter.GetComponent<Animator>();
    }

    IEnumerator Timer(int time_limit)
    {
        while (time_limit > 0)
        {
            time_limit -= 1;
            timerText.text = time_limit.ToString();
            yield return new WaitForSeconds(1f);
        }

        GameOver();

    }
    void Update()
    {
        score_ui.text = score.ToString();
        if (active_character_controller)
            player_health.text = active_character_controller.health.ToString();
    }

    public void AddScore(int s)
    {
        score += s;
    }

    public void GameOver()
    {
        if (PersistentManagerScript.Instance)
        {

            PersistentManagerScript.Instance.score = score;
        }
        LoadSceneManager l = FindObjectOfType<LoadSceneManager>();
        if (l != null)
        {
            l.GoToGameOverScene();
        }
    }
}
