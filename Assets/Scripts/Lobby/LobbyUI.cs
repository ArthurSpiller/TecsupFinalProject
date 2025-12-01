using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InLobbyUI : MonoBehaviour
{
    public LobbyManager lobbyManager;
    public GameObject preLobbyPanel;

    public TMP_Text lobbyInfoText;
    public TMP_Text lobbyCodeText;
    public Transform playersContainer;
    public GameObject playerEntryPrefab;

    public Button readyButton;
    public Button startGameButton;
    public Button leaveButton;

    private bool isReady = false;

    private void Start()
    {
        readyButton.onClick.AddListener(OnReadyClicked);
        startGameButton.onClick.AddListener(() => lobbyManager.StartGame());
        leaveButton.onClick.AddListener(() => lobbyManager.LeaveLobby());
    }

    private void Update()
    {
        // Show Lobby Info
        lobbyInfoText.text = $"{lobbyManager.JoinedLobby.Name}";
        lobbyCodeText.text = $"{lobbyManager.JoinedLobby.LobbyCode}";

        UpdatePlayerList();

        // Host controls Start button
        if (lobbyManager.IsLobbyHost())
        {
            startGameButton.gameObject.SetActive(true);
            startGameButton.interactable = lobbyManager.AreAllPlayersReady();
        } else {
            startGameButton.gameObject.SetActive(false);
        }
    }

    private void UpdatePlayerList()
    {
        foreach (Transform t in playersContainer)
            Destroy(t.gameObject);

        foreach (var player in lobbyManager.JoinedLobby.Players)
        {
            GameObject entry = Instantiate(playerEntryPrefab, playersContainer);

            string name = player.Data["PlayerName"].Value;
            string ready = player.Data["Ready"].Value == "1" ? "✔ READY" : "✖ NOT READY";

            entry.GetComponentInChildren<TMP_Text>().text = $"{name} — {ready}";
        }
    }

    private void OnReadyClicked()
    {
        isReady = !isReady;

        readyButton.GetComponentInChildren<TMP_Text>().text =
            isReady ? "Unready" : "Ready";

        lobbyManager.SetReady(isReady);
    }
}
