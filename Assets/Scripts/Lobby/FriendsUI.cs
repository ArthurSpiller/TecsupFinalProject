using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.Services.Friends.Models;
using Unity.Services.Friends;
using Unity.Services.Friends.Notifications;
using Unity.Services.Authentication;

public class FriendsUI : MonoBehaviour
{
    [Header("References")]
    public FriendsManager friendsManager;
    public GameObject preLobbyPanel;
    public Button backButton;

    [Header("Add Friend")]
    public TMP_InputField usernameInput;
    public Button addFriendButton;

    [Header("Sections")]
    public Transform friendsListContainer;
    public Transform incomingListContainer;
    public Transform outgoingListContainer;
    public Transform blocksListContainer;

    public GameObject friendEntryPrefab;

    [Header("Refresh")]
    public Button refreshFriendsButton;

    [Header("Presence/Activity")]
    public TMP_Dropdown availabilityDropdown;
    public Button setAvailabilityButton;

    private async void Start()
    {
        await friendsManager.InitializeFriends();

        addFriendButton.onClick.AddListener(OnAddFriendClicked);
        refreshFriendsButton.onClick.AddListener(PopulateAllLists);
        backButton.onClick.AddListener(OnBackButtonClicked);

        setAvailabilityButton.onClick.AddListener(OnSetAvailability);

        availabilityDropdown.ClearOptions();
        availabilityDropdown.AddOptions(new List<string> { "Online", "Busy", "Away", "Offline" });

        PopulateAllLists();
    }

    private void OnBackButtonClicked() {
        gameObject.SetActive(false);
        preLobbyPanel.SetActive(true);
    }

    private void OnAddFriendClicked()
    {
        string username = usernameInput.text.Trim();
        if (username.Length > 0)
            friendsManager.AddFriendByName(username);
    }

    private void OnSetAvailability()
    {
        Availability selected = (Availability)availabilityDropdown.value;
        friendsManager.SetAvailability(selected);
    }

    private void PopulateAllLists()
    {
        Clear(friendsListContainer);
        Clear(incomingListContainer);
        Clear(outgoingListContainer);
        Clear(blocksListContainer);

        PopulateFriendsList();
        PopulateIncomingList();
        PopulateOutgoingList();
        PopulateBlocksList();
    }

    private void PopulateFriendsList()
    {
        foreach (Relationship rel in FriendsService.Instance.Friends)
        {
            GameObject entry = Instantiate(friendEntryPrefab, friendsListContainer);
            
            string name = rel.Member.Id;
            string availability = rel.Member.Presence.Availability.ToString();

            entry.transform.Find("PlayerNameText").GetComponent<TMP_Text>().text = name;
            entry.transform.Find("AvailabilityText").GetComponent<TMP_Text>().text = availability;

            // Buttons
            Button removeBtn = entry.transform.Find("ActionButton2").GetComponent<Button>();


            removeBtn.GetComponentInChildren<TMP_Text>().text = "Remove";
            removeBtn.onClick.AddListener(() =>
            {
                friendsManager.DeleteFriend(rel.Member.Id);
                PopulateAllLists();
            });
        }
    }

    private void PopulateIncomingList()
    {
        foreach (Relationship rel in FriendsService.Instance.IncomingFriendRequests)
        {
            GameObject entry = Instantiate(friendEntryPrefab, incomingListContainer);
            
            entry.transform.Find("PlayerNameText").GetComponent<TMP_Text>().text = rel.Member.Id;
            entry.transform.Find("AvailabilityText").GetComponent<TMP_Text>().text = "Incoming Request";

            Button acceptBtn = entry.transform.Find("ActionButton1").GetComponent<Button>();
            Button rejectBtn = entry.transform.Find("ActionButton2").GetComponent<Button>();

            acceptBtn.GetComponentInChildren<TMP_Text>().text = "Accept";
            acceptBtn.onClick.AddListener(() =>
            {
                friendsManager.SendFriendRequest(rel.Member.Id); 
                PopulateAllLists();
            });

            rejectBtn.GetComponentInChildren<TMP_Text>().text = "Reject";
            rejectBtn.onClick.AddListener(() =>
            {
                friendsManager.DeleteIncomingRequest(rel.Member.Id);
                PopulateAllLists();
            });
        }
    }

    private void PopulateOutgoingList()
    {
        foreach (Relationship rel in FriendsService.Instance.OutgoingFriendRequests)
        {
            GameObject entry = Instantiate(friendEntryPrefab, outgoingListContainer);

            entry.transform.Find("PlayerNameText").GetComponent<TMP_Text>().text = rel.Member.Id;
            entry.transform.Find("AvailabilityText").GetComponent<TMP_Text>().text = "Outgoing Request";

            Button cancelBtn = entry.transform.Find("ActionButton1").GetComponent<Button>();
            cancelBtn.GetComponentInChildren<TMP_Text>().text = "Cancel";
            cancelBtn.onClick.AddListener(() =>
            {
                friendsManager.DeleteOutgoingRequest(rel.Member.Id);
                PopulateAllLists();
            });

            entry.transform.Find("ActionButton2").gameObject.SetActive(false);
        }
    }

    private void PopulateBlocksList()
    {
        foreach (Relationship rel in FriendsService.Instance.Blocks)
        {
            GameObject entry = Instantiate(friendEntryPrefab, blocksListContainer);

            entry.transform.Find("PlayerNameText").GetComponent<TMP_Text>().text = rel.Member.Id;
            entry.transform.Find("AvailabilityText").GetComponent<TMP_Text>().text = "Blocked";

            Button unblockBtn = entry.transform.Find("ActionButton1").GetComponent<Button>();
            unblockBtn.GetComponentInChildren<TMP_Text>().text = "Unblock";
            unblockBtn.onClick.AddListener(() =>
            {
                friendsManager.UnblockUser(rel.Member.Id);
                PopulateAllLists();
            });

            entry.transform.Find("ActionButton2").gameObject.SetActive(false);
        }
    }

    private void Clear(Transform container)
    {
        foreach (Transform c in container)
            Destroy(c.gameObject);
    }
}
