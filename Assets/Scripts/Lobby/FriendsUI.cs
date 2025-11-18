using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Friends;
using Unity.Services.Friends.Models;
using Unity.Services.Friends.Notifications;
using Unity.Services.Authentication;
using TMPro;

public class FriendsUI : MonoBehaviour
{
    [Header("References")]
    public FriendsManager friendsManager;

    [Header("UI Fields")]
    public TMP_InputField addFriendInput;
    public Button sendRequestButton;

    public Transform friendsContainer;
    public Transform incomingContainer;

    public GameObject friendEntryPrefab;
    public GameObject requestEntryPrefab;
    
    private async void OnEnable()
    {
        await friendsManager.InitializeFriends();
        Debug.Log("PLAYER ID = " + AuthenticationService.Instance.PlayerId);
        Debug.Log("PLAYER NAME = " + AuthenticationService.Instance.PlayerName);
        sendRequestButton.onClick.AddListener(OnSendFriendRequest);

        FriendsService.Instance.RelationshipAdded += OnRelationshipsAdded;
        FriendsService.Instance.RelationshipDeleted += OnRelationshipsDeleted;
        FriendsService.Instance.PresenceUpdated += OnPresenceChanged;

        RefreshAll();
    }

    private void OnDisable()
    {
        sendRequestButton.onClick.RemoveListener(OnSendFriendRequest);

        FriendsService.Instance.RelationshipAdded -= OnRelationshipsAdded;
        FriendsService.Instance.RelationshipDeleted -= OnRelationshipsDeleted;
        FriendsService.Instance.PresenceUpdated -= OnPresenceChanged;
    }

    private void OnSendFriendRequest()
    {
        string input = addFriendInput.text.Trim();
        if (string.IsNullOrEmpty(input)) return;

        friendsManager.AddFriendByName(input);
        addFriendInput.text = "";
    }

    private void OnRelationshipsAdded(IRelationshipAddedEvent evt)  => RefreshAll();
    private void OnRelationshipsDeleted(IRelationshipDeletedEvent evt) => RefreshAll();
    private void OnPresenceChanged(IPresenceUpdatedEvent evt) => RefreshAll();

    private void RefreshAll()
    {
        RefreshFriends();
        RefreshIncoming();
    }

    private void RefreshFriends()
    {
        foreach (Transform child in friendsContainer)
            Destroy(child.gameObject);

        foreach (Relationship rel in FriendsService.Instance.Friends)
        {
            GameObject entry = Instantiate(friendEntryPrefab, friendsContainer);

            TMP_Text nameText = entry.transform.Find("FriendName").GetComponent<TMP_Text>();
            TMP_Text statusText = entry.transform.Find("FriendStatus").GetComponent<TMP_Text>();
            Button removeFriendButton = entry.transform.Find("RemoveFriend").GetComponent<Button>();

            string id = rel.Member.Id;
            nameText.text = id;

            string availability = rel.Member.Presence != null ?
                rel.Member.Presence.Availability.ToString() : "Offline";

            statusText.text = availability;
            removeFriendButton.onClick.AddListener(() => {
                friendsManager.DeleteFriend(id);
                RefreshAll();
            });
        }
    }

    private void RefreshIncoming()
    {
        foreach (Transform child in incomingContainer)
            Destroy(child.gameObject);

        foreach (Relationship rel in FriendsService.Instance.IncomingFriendRequests)
        {
            GameObject entry = Instantiate(requestEntryPrefab, incomingContainer);

            TMP_Text nameText = entry.transform.Find("FriendName").GetComponent<TMP_Text>();
            Button acceptBtn = entry.transform.Find("AcceptButton").GetComponent<Button>();
            Button rejectBtn = entry.transform.Find("RejectButton").GetComponent<Button>();

            string id = rel.Member.Id;
            nameText.text = id;

            acceptBtn.onClick.AddListener(() => {
                friendsManager.AcceptFriendRequest(id);
                RefreshAll();
            });
            rejectBtn.onClick.AddListener(() => {
                friendsManager.DeleteIncomingRequest(id);
                RefreshAll();
            });
        }
    }
}
