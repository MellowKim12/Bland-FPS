using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Netcode;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }

    private Lobby joinedLobby;

    private void Awake()
    {
        if (this.gameObject == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeUnityAuthentication();
        }
        else
            Destroy(gameObject);
      
    }

    private async void InitializeUnityAuthentication()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(Random.Range(0, 1000).ToString());

            await UnityServices.InitializeAsync();
            
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        }
    }

    public async void CreateLobby(string name, bool isPrivate)
    {
        try
        {
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(name, 4, new CreateLobbyOptions
            {
                IsPrivate = isPrivate,
            });

            NetworkManager.Singleton.StartHost();
            

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);   
        }

    }



}
