using System;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.RemoteConfig;
using UnityEngine.UI;
using Unity.Services.Core;
using Unity.Services.Authentication;

public class RemoteConfigMain : MonoBehaviour
{
    struct UserAttributes { }
    struct AppAttributes { }

    [Header("References")]
    public Transform playerCube;
    public Camera mainCamera;
    public Button fetchButton;

    private void Awake()
    {

        RemoteConfigService.Instance.FetchCompleted += OnRemoteConfigFetched;
    }

    private async void Start()
    {
        Debug.Log("Ready. Press button to Fetch.");
        await UnityServices.InitializeAsync();
        fetchButton.onClick.AddListener(StartFetching);
    }

    public async void StartFetching()
    {
        await FetchConfig();
    }

    public async Task FetchConfig()
    {
        Debug.Log("Trying to fetch info...");

        try
        {
            await RemoteConfigService.Instance.FetchConfigsAsync(new UserAttributes(), new AppAttributes());
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }
    [Serializable]
    public class LegendaryItem
    {
        public string name;
        public int power;
        public float dropRate;
    }

    private void OnRemoteConfigFetched(ConfigResponse response)
    {
        Debug.Log("Config fetched from: " + response.requestOrigin);

        int playerSpeed = RemoteConfigService.Instance.appConfig.GetInt("PlayerSpeed", 5);
        Debug.Log("playerSpeed: " + playerSpeed);

        float playerScale = RemoteConfigService.Instance.appConfig.GetFloat("PlayerScale", 1f);
        playerCube.localScale = Vector3.one * playerScale;
        Debug.Log("playerScale applied: " + playerScale);

        string backgroundColor = RemoteConfigService.Instance.appConfig.GetString("PlayerColor", "black");
        mainCamera.backgroundColor = ColorFromName(backgroundColor);
        Debug.Log("backgroundColor applied: " + backgroundColor);

        string json = RemoteConfigService.Instance.appConfig.GetJson("PlayerInfo");
        LegendaryItem item = JsonUtility.FromJson<LegendaryItem>(json);
        Debug.Log($"Legendary Item:\nName: {item.name}\nPower: {item.power}\nDrop Rate: {item.dropRate}");
    }

    Color ColorFromName(string colorName)
    {
        switch (colorName.ToLower())
        {
            case "red": return Color.red;
            case "blue": return Color.blue;
            case "green": return Color.green;
            case "white": return Color.white;
            case "black": return Color.black;
            case "yellow": return Color.yellow;
            case "cyan": return Color.cyan;
            case "magenta": return Color.magenta;

            default: return Color.gray;
        }
    }
}
