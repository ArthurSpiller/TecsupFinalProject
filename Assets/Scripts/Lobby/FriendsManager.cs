using UnityEngine;
using Unity.Services.Friends;
// using Sirenix.OdinInspector;
using Unity.Services.Friends.Notifications;
using System;
using Unity.Services.Friends.Models;
using System.Threading.Tasks;
using Unity.Services.Authentication;

public class FriendsManager : MonoBehaviour
{   
    void Start()
    {
    }

    public async Task InitializeFriends()
    {
        await FriendsService.Instance.InitializeAsync();

        FriendsService.Instance.RelationshipAdded += OnRelationshipAdded;
        FriendsService.Instance.RelationshipDeleted += OnRelationshipDeleted;
        FriendsService.Instance.MessageReceived += OnMessageRecived;
        FriendsService.Instance.PresenceUpdated += OnPresenceUpdated;
    }

    public async void SendFriendRequest(string playerId)
    {
        await FriendsService.Instance.AddFriendAsync(playerId);
    }
    
    public async void AddFriendByName(string username)
    {
        await FriendsService.Instance.AddFriendByNameAsync(username);
    }

    public async void AcceptFriendRequest(string playerId)
    {
        await FriendsService.Instance.AddFriendAsync(playerId);
    }
    
    public async void BlockUser(string playerId)
    {
        await FriendsService.Instance.AddBlockAsync(playerId);  
    }
    
    public async void UnblockUser(string playerId)
    {
        await FriendsService.Instance.DeleteBlockAsync(playerId);
    }
 
    public async void DeleteFriend(string playerId)
    {
        await FriendsService.Instance.DeleteFriendAsync(playerId);
    }
    
    public async void DeleteIncomingRequest(string playerId)
    {
        await FriendsService.Instance.DeleteIncomingFriendRequestAsync(playerId);
    }
    
    public async void DeleteOutgoingRequest(string playerId)
    {
        await FriendsService.Instance.DeleteOutgoingFriendRequestAsync(playerId);
    }

    public void ShowFriends()
    {
        foreach (Relationship rel in FriendsService.Instance.Friends)
        {
            Debug.Log(rel.Member.Id + " ||" + rel.Member.Presence.Availability );
        }
    }

    public void ShowIncoming()
    {
        foreach (Relationship rel in FriendsService.Instance.IncomingFriendRequests) {
            Debug.Log(rel.Member.Id + " ||" + rel.Member.Presence.Availability);
        }
    }
    
    public void ShowOutgoing()
    {
        foreach (Relationship rel in FriendsService.Instance.OutgoingFriendRequests) {
            Debug.Log(rel.Member.Id + " ||" + rel.Member.Presence.Availability);
        }
    }
    
    public void ShowBlocks()
    {
        foreach (Relationship rel in FriendsService.Instance.Blocks) {
            Debug.Log(rel.Member.Id + " ||" + rel.Member.Presence.Availability);
        }
    }
    
    public async void SetAvailability(Availability availability)
    {
        await FriendsService.Instance.SetPresenceAvailabilityAsync(availability);
    }
    #region Example
    public class ActivitySample
    {
        public string state;
        public string rounds;

        public ActivitySample() { }
    }

    public void TestActivity()
    {
        ActivitySample activity = new();
        activity.state = "Gibraltar";
        activity.rounds = "8/13";
        SetActivity(activity);
    }
    #endregion

    public async void SetActivity<T>(T activity)where T : new()
    {
        await FriendsService.Instance.SetPresenceActivityAsync(activity);
    }

    public async void SetPrecense<T>(Availability availability , T activity) where T : new()
    {
        await FriendsService.Instance.SetPresenceAsync(availability, activity);
    }

    public class LobbyInviteMessage
    {
        public string lobbyId;
        public string fromPlayerID;

        public LobbyInviteMessage() { } 
    }
    public class SimpleMessage
    {
        public string content;

        public SimpleMessage() { }
    }
    
    public async Task SendMessage(string targetID , string text)
    {
        var msg = new SimpleMessage();
        msg.content = text;

        await FriendsService.Instance.MessageAsync(targetID, msg);

        Debug.Log("Mensaje enviado con exito");
    }

    public async Task SendLobbyInvite(string targetID, string lobbyId)
    {
        var lobbyInvite = new LobbyInviteMessage();
        lobbyInvite.lobbyId = lobbyId;
        lobbyInvite.fromPlayerID = AuthenticationService.Instance.PlayerId;

         await FriendsService.Instance.MessageAsync(targetID, lobbyInvite);

        Debug.Log("Invitacion a lobby enviada");
    }

    private void OnPresenceUpdated(IPresenceUpdatedEvent e)
    {
        Debug.Log("Se a actualizado es estado de" + e.ID + e.Presence.Availability);
    }

    private void OnMessageRecived(IMessageReceivedEvent e)
    {
        LobbyInviteMessage invite = null;
        try {
            invite = e.GetAs<LobbyInviteMessage>();
        } catch { 
            var msg = e.GetAs<SimpleMessage>();
            Debug.Log("he recibido un mensaje de " + e.UserId + "mensaje : " + msg.content);

        }
        if (invite == null)
            return;
    }

    private void OnRelationshipDeleted(IRelationshipDeletedEvent e)
    {
        Debug.Log("Relationship added : " + e.Relationship.Id);
    }

    private void OnRelationshipAdded(IRelationshipAddedEvent e)
    {
        Debug.Log("Relationship added : " + e.Relationship.Id + e.Relationship.Type);
    }
}
