using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.Services.Vivox;

public class VivoxUI : MonoBehaviour
{
    [Header("References")]
    public VivoxManager vivoxManager;
    public GameObject preLobbyPanel;

    [Header("Login")]
    public Button loginButton;

    [Header("Channels")]
    public TMP_InputField textChannelInput;
    public Button joinTextButton;

    public TMP_InputField voiceChannelInput;
    public Button joinVoiceButton;

    [Header("Messaging")]
    public TMP_InputField messageInput;
    public Button sendMessageButton;

    [Header("Chat History")]
    public Transform messageContainer;
    public GameObject messagePrefab;

    [Header("Audio Settings")]
    public Slider micVolumeSlider;
    public Slider outputVolumeSlider;

    [Header("Devices")]
    public TMP_Dropdown inputDeviceDropdown;
    public TMP_Dropdown outputDeviceDropdown;

    [Header("Nav")]
    public Button backButton;

    void Start()
    {
        loginButton.onClick.AddListener(OnLogin);
        joinTextButton.onClick.AddListener(JoinText);
        joinVoiceButton.onClick.AddListener(JoinVoice);
        sendMessageButton.onClick.AddListener(SendMessage);
        backButton.onClick.AddListener(Back);

        micVolumeSlider.onValueChanged.AddListener(v => vivoxManager.SetMicVolume((int)v));
        outputVolumeSlider.onValueChanged.AddListener(v => vivoxManager.SetOutputVolume((int)v));

        inputDeviceDropdown.onValueChanged.AddListener(OnInputDeviceSelected);
        outputDeviceDropdown.onValueChanged.AddListener(OnOutputDeviceSelected);
    }
    
    // -------------------------------
    // LOGIN
    // -------------------------------
    private async void OnLogin()
    {
        await vivoxManager.LoginVivox();
    }

    // -------------------------------
    // CHANNEL BUTTONS
    // -------------------------------
    private async void JoinText()
    {
        string channel = textChannelInput.text;
        if (string.IsNullOrEmpty(channel)) channel = "CH1";

        await vivoxManager.JoinTextChannel(channel);
    }

    private async void JoinVoice()
    {
        string channel = voiceChannelInput.text;
        if (string.IsNullOrEmpty(channel)) channel = "CH1";

        await vivoxManager.JoinVoiceChannel(channel);
    }

    // -------------------------------
    // MESSAGING
    // -------------------------------
    private async void SendMessage()
    {
        string msg = messageInput.text;
        if (string.IsNullOrEmpty(msg)) return;

        string channel = textChannelInput.text;
        if (string.IsNullOrEmpty(channel)) channel = "CH1";

        await vivoxManager.SendMessageToChannel(msg, channel);
        AddMessageToUI("Me", msg);

        messageInput.text = "";
    }

    public void AddMessageToUI(string sender, string message)
    {
        GameObject item = Instantiate(messagePrefab, messageContainer);
        TMP_Text label = item.GetComponentInChildren<TMP_Text>();
        label.text = $"{sender}: {message}";
    }

    // -------------------------------
    // DEVICE LISTS
    // -------------------------------
    public void RefreshDeviceLists()
    {
        if (VivoxService.Instance == null)
            return;

        inputDeviceDropdown.ClearOptions();
        outputDeviceDropdown.ClearOptions();

        List<string> inputNames = new();
        foreach (var d in VivoxService.Instance.AvailableInputDevices)
            inputNames.Add(d.DeviceName);

        List<string> outputNames = new();
        foreach (var d in VivoxService.Instance.AvailableOutputDevices)
            outputNames.Add(d.DeviceName);

        inputDeviceDropdown.AddOptions(inputNames);
        outputDeviceDropdown.AddOptions(outputNames);
    }

    private void OnInputDeviceSelected(int index)
    {
        string id = VivoxService.Instance.AvailableInputDevices[index].DeviceID;
        vivoxManager.SelectInputDevice(id);
    }

    private void OnOutputDeviceSelected(int index)
    {
        string id = VivoxService.Instance.AvailableOutputDevices[index].DeviceID;
        vivoxManager.SelectOutputDevice(id);
    }

    // -------------------------------
    // NAVIGATION
    // -------------------------------
    private void Back()
    {
        gameObject.SetActive(false);
        preLobbyPanel.SetActive(true);
    }
}
