using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class SimpleLobbyManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject connexionPanel;
    public GameObject lobbyPanel;
    public GameObject friendsPanel;
    public Button backButton;
    public Button quickPlayButton;
    public Button createLobbyButton;
    public Button friendsButton;
    public TMP_InputField lobbyNameInput;
    public TMP_InputField lobbyNbPlayerInput;
    public Button joinLobbyButton;
    public TMP_InputField lobbyCodeInput;

    [SerializeField] private Button showInfoButton;
    [SerializeField] private TMP_Text lobbyInfoText;

    private Lobby currentLobby;

    private async void Start() {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn) {
            gameObject.SetActive(false);
            connexionPanel.SetActive(true);
            lobbyPanel.SetActive(false);
            friendsPanel.SetActive(false);
        } else  {
            gameObject.SetActive(true);
            connexionPanel.SetActive(false);
            lobbyPanel.SetActive(false);
            friendsPanel.SetActive(false);
        }
    }

    private void OnEnable() {
        backButton.onClick.AddListener(BackToMainMenu);
        quickPlayButton.onClick.AddListener(QuickPlay);
        createLobbyButton.onClick.AddListener(CreateLobby);
        friendsButton.onClick.AddListener(ToFriends);
        joinLobbyButton.onClick.AddListener(() => JoinLobbyByCode(lobbyCodeInput.text));
        showInfoButton.onClick.AddListener(PrintLobbyInfo);
    }
    
    private void QuickPlay() {
        SceneManager.LoadScene("MainScene");
    }

    private void BackToMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }

    private void ToFriends() {
        gameObject.SetActive(false);
        friendsPanel.SetActive(true);
    }
    
    private async Task<bool> EnsureSignedInAsync() {
        if (AuthenticationService.Instance.IsSignedIn)
            return true;
        try {
            await PlayerAccountService.Instance.StartSignInAsync();

            while (!PlayerAccountService.Instance.IsSignedIn) {
                await Task.Yield();
            }
            string accessToken = PlayerAccountService.Instance.AccessToken;

            if (string.IsNullOrEmpty(accessToken)) {
                Debug.LogError("Access token missing after Player Account sign-in.");
                lobbyInfoText.text = "Login failed. Try again.";
                return false;
            }
            await AuthenticationService.Instance.SignInWithUnityAsync(accessToken);
            Debug.Log($"Signed in as PlayerID: {AuthenticationService.Instance.PlayerId}");
            return true;
        } catch (Exception ex) {
            Debug.LogError($"Sign-in failed: {ex.Message}");
            lobbyInfoText.text = "Sign-in failed. Try again.";
            return false;
        }
    }

    private async void CreateLobby() {
        if (!await EnsureSignedInAsync())
            return;
        try {
            if (string.IsNullOrEmpty(lobbyNameInput.text)) {
                lobbyInfoText.text = "Please enter a lobby name.";
                return;
            }

            if (!int.TryParse(lobbyNbPlayerInput.text, out int maxPlayers) || maxPlayers <= 0) {
                lobbyInfoText.text = "Please enter a valid number of players.";
                return;
            }

            currentLobby = await LobbyService.Instance.CreateLobbyAsync(
                lobbyNameInput.text,
                maxPlayers,
                new CreateLobbyOptions {
                    Player = new Player(AuthenticationService.Instance.PlayerId)
                });

            LobbyState.Instance.SetLocalPlayer(AuthenticationService.Instance.PlayerId);
            LobbyState.Instance.SetLobby(currentLobby);

            gameObject.SetActive(false);
            lobbyPanel.SetActive(true);
            
        } catch (Exception ex) {
            Debug.LogError($"Failed to create lobby: {ex.Message}");
            lobbyInfoText.text = "Failed to create lobby.";
        }
    }

    private async void JoinLobbyByCode(string code) {
        if (string.IsNullOrEmpty(code)) {
            lobbyInfoText.text = "Enter a lobby code first!";
            return;
        }
        if (!await EnsureSignedInAsync())
            return;
        try {
            currentLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code);

            LobbyState.Instance.SetLocalPlayer(AuthenticationService.Instance.PlayerId);
            LobbyState.Instance.SetLobby(currentLobby);

            gameObject.SetActive(false);
            lobbyPanel.SetActive(true);
        } catch (Exception ex) {
            Debug.LogError($"Failed to join lobby: {ex.Message}");
            lobbyInfoText.text = "Failed to join lobby.";
        }
    }

    private async void PrintLobbyInfo() {
        try {
            if (currentLobby == null) {
                lobbyInfoText.text = "No lobby joined yet.";
                return;
            }

            currentLobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);

            string info = $"Lobby: {currentLobby.Name}\nPlayers: {currentLobby.Players.Count}";
            foreach (var player in currentLobby.Players)
                info += $"\n - {player.Id}";

            Debug.Log(info);
            lobbyInfoText.text = info;
        } catch (Exception ex) {
            Debug.LogError($"Failed to refresh lobby info: {ex.Message}");
        }
    }
}
