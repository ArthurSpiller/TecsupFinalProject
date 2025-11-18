using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LobbyPanelUI : MonoBehaviour
{
    [Header("UIReference")]
    public GameObject preLobbyPanel;
    public TMP_Text lobbyNameText;
    public TMP_Text lobbyCode;
    public Button leaveButton;
    public Button readyButton;
    public Button startButton;

    [Header("PlayerList")]
    public Transform playersContainer;
    public GameObject playerEntryPrefab;

    private void OnEnable()
    {
        LobbyState.Instance.OnLobbyUpdated += RefreshLobbyUI;

        if (LobbyState.Instance.CurrentLobby != null)
            RefreshLobbyUI(LobbyState.Instance.CurrentLobby);

        leaveButton.onClick.AddListener(LeaveLobby);
        readyButton.onClick.AddListener(ToggleReady);
        startButton.onClick.AddListener(StartPlay);
    }

    private void OnDisable()
    {
        LobbyState.Instance.OnLobbyUpdated -= RefreshLobbyUI;
        leaveButton.onClick.RemoveListener(LeaveLobby);
        readyButton.onClick.RemoveListener(ToggleReady);
        startButton.onClick.RemoveListener(StartPlay);
    }

    private void RefreshLobbyUI(Lobby lobby)
    {
        if (lobby == null) return;

        lobbyNameText.text = lobby.Name;
        lobbyCode.text = $"Code: {lobby.LobbyCode}";

        // Check if local player is host
        bool isHost = lobby.HostId == LobbyState.Instance.LocalPlayerId;
        startButton.gameObject.SetActive(isHost);

        // Clear previous list
        foreach (Transform child in playersContainer)
            Destroy(child.gameObject);

        // Populate player entries
        foreach (var player in lobby.Players)
        {
            GameObject entry = Instantiate(playerEntryPrefab, playersContainer);

            TMP_Text nameText = entry.transform.Find("PlayerName").GetComponent<TMP_Text>();
            TMP_Text readyText = entry.transform.Find("PlayerReadiness").GetComponent<TMP_Text>();

            nameText.text = player.Id;

            bool isReady =
                player.Data != null &&
                player.Data.ContainsKey("ready") &&
                player.Data["ready"].Value == "1";

            readyText.text = isReady ? "Ready" : "Not Ready";
        }
    }

    // ---- LEAVE LOBBY ----

    private async void LeaveLobby()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(
                LobbyState.Instance.CurrentLobby.Id,
                LobbyState.Instance.LocalPlayerId
            );

            LobbyState.Instance.ClearLobby();

            gameObject.SetActive(false);
            preLobbyPanel.SetActive(true);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to leave lobby: " + ex);
        }
    }

    // ---- READY TOGGLE ----

    private async void ToggleReady()
    {
        try
        {
            Lobby lobby = LobbyState.Instance.CurrentLobby;
            if (lobby == null) return;

            // Get local player
            Player localPlayer = lobby.Players.Find(p => p.Id == LobbyState.Instance.LocalPlayerId);

            string current = "0";
            if (localPlayer.Data != null &&
                localPlayer.Data.ContainsKey("ready"))
            {
                current = localPlayer.Data["ready"].Value;
            }

            string newValue = current == "1" ? "0" : "1";

            var updateOptions = new UpdatePlayerOptions
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    { "ready",
                        new PlayerDataObject(
                            PlayerDataObject.VisibilityOptions.Member,
                            newValue)
                    }
                }
            };

            Lobby updated = await LobbyService.Instance.UpdatePlayerAsync(
                lobby.Id,
                localPlayer.Id,
                updateOptions
            );

            // Update the global lobby state
            LobbyState.Instance.SetLobby(updated);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to update ready state: " + ex);
        }
    }

    // ---- START THE GAME ----

    private void StartPlay()
    {
        // In the future: verify all players are ready
        SceneManager.LoadScene("MainScene");
    }
}
