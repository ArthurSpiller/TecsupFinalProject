using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine.SceneManagement;

public class PreLobbyUI : MonoBehaviour
{
    [Header("References")]
    public LobbyManager lobbyManager;
    public GameObject lobbyPanel;
    public GameObject friendsPanel;
    public GameObject vivoxPanel;
    public GameObject leaderboardPanel;

    [Header("Create Lobby")]
    public TMP_InputField lobbyNameInput;
    public TMP_InputField maxPlayersInput;
    public Button createLobbyButton;

    [Header("Join Lobby")]
    public TMP_InputField joinCodeInput;
    public Button joinLobbyButton;

    [Header("List Lobbies")]
    public Button refreshButton;
    public Transform lobbyListContainer;
    public GameObject lobbyListItemPrefab;

    [Header("Nav Buttons")]
    public Button backButton;
    public Button friendsButton;
    public Button vivoxButton;
    public Button leaderboardButton;
    public Button quickplayButton;

    private void Start()
    {
        // Hook UI interactions
        createLobbyButton.onClick.AddListener(OnCreateLobbyClicked);
        joinLobbyButton.onClick.AddListener(OnJoinLobbyClicked);
        refreshButton.onClick.AddListener(OnRefreshClicked);
        backButton.onClick.AddListener(OnBackClicked);
        friendsButton.onClick.AddListener(OnFriendsClicked);
        vivoxButton.onClick.AddListener(OnVivoxClicked);
        leaderboardButton.onClick.AddListener(OnLeaderboardClicked);
        quickplayButton.onClick.AddListener(OnQuickplayCLicked);

        // Subscribe to lobby refresh callback
        lobbyManager.OnLobbyRefresh += PopulateLobbyList;
    }

    private void OnBackClicked() {
        SceneManager.LoadScene("MainMenu");
    }

    private void OnQuickplayCLicked() {
        SceneManager.LoadScene("MainScene");        
    }
        
    private void OnFriendsClicked() {
        gameObject.SetActive(false);
        friendsPanel.SetActive(true);
    }

    private void OnVivoxClicked() {
        gameObject.SetActive(false);
        vivoxPanel.SetActive(true);
    }

    private void OnLeaderboardClicked() {
        gameObject.SetActive(false);
        leaderboardPanel.SetActive(true);
    }

    private void OnCreateLobbyClicked()
    {
        string name = lobbyNameInput.text;
        int.TryParse(maxPlayersInput.text, out int maxPlayers);

        if (maxPlayers <= 0) maxPlayers = 4;

        lobbyManager.CreateLobby(name, maxPlayers);
        gameObject.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    private void OnJoinLobbyClicked()
    {
        string code = joinCodeInput.text;
        lobbyManager.JoinLobbyByCode(code);
        gameObject.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    private void OnRefreshClicked()
    {
        lobbyManager.ListLobbies();
    }

    private void PopulateLobbyList(List<Lobby> lobbies)
    {
        foreach (Transform child in lobbyListContainer)
            Destroy(child.gameObject);

        foreach (Lobby lobby in lobbies)
        {
            GameObject item = Instantiate(lobbyListItemPrefab, lobbyListContainer);

            TMP_Text label = item.GetComponentInChildren<TMP_Text>();
            label.text = $"{lobby.Name} ({lobby.Players.Count}/{lobby.MaxPlayers})";

            Button btn = item.GetComponentInChildren<Button>();
            btn.onClick.AddListener(() =>
            {
                lobbyManager.JoinLobbyByID(lobby.Id);
            });
        }
    }
}
