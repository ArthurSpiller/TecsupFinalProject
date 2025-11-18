using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Authentication;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class ConnexionUI : MonoBehaviour
{
    [Header("UI References")]
    public Button connectButton;
    public Button backButton;
    public GameObject preLobbyPanel;

    [Header("UnityAuthHolder")]
    public UnityPlayerAuth unityPlayerAuth;

    void Start() {
        if (AuthenticationService.Instance.IsSignedIn) {
            gameObject.SetActive(false);
            preLobbyPanel.SetActive(true);
            return;
        }
    }

    private void OnEnable() {
        connectButton.onClick.AddListener(ConnectPlayer);
        unityPlayerAuth.OnSignIn += UnityPlayerOnSignIn;
        backButton.onClick.AddListener(BackToMainMenu);
    }

    private void UnityPlayerOnSignIn(PlayerInfo info, string name) {
        gameObject.SetActive(false);
        preLobbyPanel.gameObject.SetActive(true);
    }
    
    private async void ConnectPlayer() {
        await unityPlayerAuth.InitSignIn();
    }

    private void BackToMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }

    private void OnDisable() {
        connectButton?.onClick.RemoveListener(ConnectPlayer);
        unityPlayerAuth.OnSignIn -= UnityPlayerOnSignIn;
    }
}
