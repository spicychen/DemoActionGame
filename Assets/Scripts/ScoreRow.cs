using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreRow : MonoBehaviour
{

    public Text player_name;
    public Text player_score;

    // Start is called before the first frame update
    void Awake()
    {
        HideScoreRow();
    }

    public void HideScoreRow()
    {
        gameObject.SetActive(false);
    }

    public void ShowScoreRow()
    {
        gameObject.SetActive(true);
    }

    public void SetScoreRow(string player_name, int player_score)
    {
        this.player_name.text = player_name;
        this.player_score.text = player_score.ToString();
        ShowScoreRow();
    }
}
