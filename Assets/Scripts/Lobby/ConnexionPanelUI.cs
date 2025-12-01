using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ConnexionPanelUI : MonoBehaviour
{
    [Header("References")]
    public UnityPlayerAuth auth;
    public Button connectButton;
    public Button backButton;

    public GameObject preLobbyPanel;

    private void Awake()
    {
        connectButton.onClick.AddListener(OnConnectClicked);
        backButton.onClick.AddListener(OnBackClicked);
    }

    private void Start()
    {
        auth.OnSignedIn += OnSignedIn;
        if (Unity.Services.Authentication.AuthenticationService.Instance.IsSignedIn) {
            gameObject.SetActive(false);
            preLobbyPanel.SetActive(true);
        }
    }

    private async void OnConnectClicked()
    {
        await auth.InitSignIn();
    }

    private void OnSignedIn(Unity.Services.Authentication.PlayerInfo info, string name)
    {
        gameObject.SetActive(false);
        preLobbyPanel.SetActive(true);
    }

    private void OnBackClicked() {
        SceneManager.LoadScene("MainMenu");
    }
}
