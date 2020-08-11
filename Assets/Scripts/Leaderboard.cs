using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;

public class Leaderboard : MonoBehaviour
{

    private List<PlayerLeaderboardEntry> daily_entries;
    private List<PlayerLeaderboardEntry> weekly_entries;

    public GameObject leaderboard_panel;
    public ScoreRow[] daily_score;
    public ScoreRow[] weekly_score;
    
    public void SetRows(List<PlayerLeaderboardEntry> daily_entries, List<PlayerLeaderboardEntry> weekly_entries)
    {
        this.daily_entries = daily_entries;
        this.weekly_entries = weekly_entries;
        leaderboard_panel.SetActive(true);
        ShowScores();
    }

    public void ShowScores()
    {
        if (daily_entries.Count <= daily_score.Length)
        {
            for(int x = 0; x < daily_entries.Count; x++)
            {
                daily_score[x].SetScoreRow(daily_entries[x].DisplayName, daily_entries[x].StatValue);
            }
        }
        if (weekly_entries.Count <= weekly_score.Length)
        {
            for (int x = 0; x < weekly_entries.Count; x++)
            {
                weekly_score[x].SetScoreRow(weekly_entries[x].DisplayName, weekly_entries[x].StatValue);
            }
        }
    }

}
