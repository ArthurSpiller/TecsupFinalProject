using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System.Threading.Tasks;
using System;
using Unity.Services.Authentication.PlayerAccounts;

public class UnityPlayerAuth : MonoBehaviour
{
    public event Action<PlayerInfo, string> OnSignIn;
    private PlayerInfo playerInfo;
    
    private async void Start() {
        try {
            await UnityServices.InitializeAsync();
        } catch (Exception e) {
            Debug.LogError("Unity Services failed to initialize: " + e);
            return;
        }

        SetupEvents();

        PlayerAccountService.Instance.SignedIn += async () => {
            try {
                string accessToken = PlayerAccountService.Instance.AccessToken;
                await AuthenticationService.Instance.SignInWithUnityAsync(accessToken);
            }
            catch (Exception e) {
                Debug.LogError("Authentication failed: " + e);
            }
        };
    }

    private void SetupEvents() {
        AuthenticationService.Instance.SignedIn += async () => {
            await SignInWithUnityAuth();
        };
        AuthenticationService.Instance.SignInFailed += (err) => {
            Debug.Log(err);
                };
        AuthenticationService.Instance.SignedOut += () => {
            Debug.Log("Player Logged Out");
        };
        AuthenticationService.Instance.Expired += () => {
            Debug.Log("Player Session Expired");
        };
    }

    public async Task InitSignIn() {
        await PlayerAccountService.Instance.StartSignInAsync();
    }
    
    private async void SignIn() {
        try {
            await SignInWithUnityAuth();
        } catch (Exception ex) {
            Debug.Log(ex);
        }
    }

    private async Task SignInWithUnityAuth() {
        try {
            playerInfo = AuthenticationService.Instance.PlayerInfo;
            string name = await AuthenticationService.Instance.GetPlayerNameAsync();

            OnSignIn?.Invoke(playerInfo, name);
            Debug.Log("Sign in Success !!!");
        }
        catch (AuthenticationException ex) {
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex) {
            Debug.LogException(ex);
        }
    }

    public async Task DeleteAccountUnityData() {
        try {
            await AuthenticationService.Instance.DeleteAccountAsync();
        } catch (Exception) {
            throw;
        }
    }
}
