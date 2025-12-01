using UnityEngine;
using Unity.Services.Leaderboards;
using System.Threading.Tasks;
using System;
using Unity.Services.Leaderboards.Models;
using System.Collections.Generic;

public class LeaderBoards : MonoBehaviour
{
    public Action<List<LeaderboardEntry>> OnScoresUpdated;

    public async void SendScore(string leaderboardID , double score)
    {
        try
        {
            AddPlayerScoreOptions options = new AddPlayerScoreOptions();
            options.Metadata = "Img_20";

            

            LeaderboardEntry entry = await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardID, score);
            print("id_(" + entry.PlayerId + ") Name: " + entry.PlayerName + " score: " + entry.Score);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
       
    }

    public async void GetScore(string leaderboardID)
    {
        try
        {
            GetPlayerScoreOptions options = new GetPlayerScoreOptions();
            options.IncludeMetadata = true;

            LeaderboardEntry entry = await LeaderboardsService.Instance.GetPlayerScoreAsync(leaderboardID, options);

            print("id_(" + entry.PlayerId + ") Name: " + entry.PlayerName + " score: " + entry.Score +" Metadata:"+ entry.Metadata.ToString());
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public async void GetTopScores(string leaderboardID)
    {
        GetScoresOptions options = new GetScoresOptions();
        options.Limit = 2;

        try
        {
            LeaderboardScoresPage page = await LeaderboardsService.Instance.GetScoresAsync(leaderboardID, options);


            foreach (LeaderboardEntry entry in page.Results)
            {
                print("id_(" + entry.PlayerId + ") Name: " + entry.PlayerName + " score: " + entry.Score + " Metadata:" + entry.Metadata.ToString());
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

    }

    public async void GetRangeScores(string leaderboardID)
    {
        GetPlayerRangeOptions options = new GetPlayerRangeOptions();
        options.RangeLimit = 2;

        try
        {
            LeaderboardScores page = await LeaderboardsService.Instance.GetPlayerRangeAsync(leaderboardID);

            foreach (LeaderboardEntry entry in page.Results)
            {
                print("id_(" + entry.PlayerId + ") Name: " + entry.PlayerName + " score: " + entry.Score + " Metadata:" + entry.Metadata.ToString());
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

    }

}
