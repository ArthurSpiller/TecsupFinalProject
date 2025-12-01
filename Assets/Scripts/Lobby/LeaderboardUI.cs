using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Unity.Services.Leaderboards.Models;

public class LeaderboardsUI : MonoBehaviour
{
    [Header("References")]
    public LeaderBoards leaderboardsManager;
    public GameObject preLobbyPanel;

    [Header("Inputs")]
    public TMP_InputField leaderboardIdInput;
    public TMP_InputField scoreInput;

    [Header("Buttons")]
    public Button sendScoreButton;
    public Button getScoreButton;
    public Button topScoresButton;
    public Button rangeScoresButton;
    public Button backButton;

    [Header("List Display")]
    public Transform scoresContainer;
    public GameObject scoreItemPrefab;

    private void Start()
    {
        sendScoreButton.onClick.AddListener(OnSendScore);
        getScoreButton.onClick.AddListener(OnGetScore);
        topScoresButton.onClick.AddListener(OnGetTopScores);
        rangeScoresButton.onClick.AddListener(OnGetRangeScores);
        backButton.onClick.AddListener(OnBackClicked);

        leaderboardsManager.OnScoresUpdated += PopulateScores;
    }

    private string LeaderboardID => leaderboardIdInput.text;

    private void OnBackClicked() {
        gameObject.SetActive(false);
        preLobbyPanel.SetActive(true);
    }
    
    private void OnSendScore()
    {
        double.TryParse(scoreInput.text, out double score);
        leaderboardsManager.SendScore(LeaderboardID, score);
    }

    private void OnGetScore()
    {
        leaderboardsManager.GetScore(LeaderboardID);
    }

    private void OnGetTopScores()
    {
        leaderboardsManager.GetTopScores(LeaderboardID);
    }

    private void OnGetRangeScores()
    {
        leaderboardsManager.GetRangeScores(LeaderboardID);
    }

    public void PopulateScores(List<LeaderboardEntry> entries)
    {
        foreach (Transform child in scoresContainer)
            Destroy(child.gameObject);

        foreach (LeaderboardEntry entry in entries)
        {
            GameObject item = Instantiate(scoreItemPrefab, scoresContainer);

            TMP_Text[] labels = item.GetComponentsInChildren<TMP_Text>();

            labels[0].text = entry.Rank.ToString();
            labels[1].text = string.IsNullOrEmpty(entry.PlayerName) ? entry.PlayerId : entry.PlayerName;
            labels[2].text = entry.Score.ToString();
        }
    }
}
