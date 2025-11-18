using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Buttons")]
    public Button PlayButton;
    public Button SettingsButton;
    public Button QuitButton;

    [Header("Settings")]
    public GameObject SettingsPanel;

    private void Start() {
        PlayButton.onClick.AddListener(PlayGame);
        SettingsButton.onClick.AddListener(OpenOptions);
        QuitButton.onClick.AddListener(QuitGame);
        
        gameObject.SetActive(true);
        SettingsPanel.SetActive(false);
    }

    public void PlayGame() {
        SceneManager.LoadScene("LobbyScene");
    }

    public void OpenOptions() {
        gameObject.SetActive(false);
        SettingsPanel.SetActive(true);
    }

    public void QuitGame() {
        Application.Quit();
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
