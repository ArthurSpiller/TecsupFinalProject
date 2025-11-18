using UnityEngine;
using Unity.Services.Lobbies.Models;
using System;

public class LobbyState : MonoBehaviour
{
    public static LobbyState Instance { get; private set; }

    public Lobby CurrentLobby { get; private set; }
    public string LocalPlayerId { get; private set; }

    public event Action<Lobby> OnLobbyUpdated;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetLocalPlayer(string playerId) {
        LocalPlayerId = playerId;
    }

    public void SetLobby(Lobby lobby) {
        CurrentLobby = lobby;
        OnLobbyUpdated?.Invoke(lobby);
    }

    public void ClearLobby() {
        CurrentLobby = null;
        OnLobbyUpdated?.Invoke(null);
    }
}
