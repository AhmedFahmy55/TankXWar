using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class ClientManager : IDisposable
{
    //events
    public event Action OnStartToJoinLobby;
    public event Action OnFailedToJoinLobby;
    public event Action<bool> OnAuthenticating;


    //Proprties
    public NetworkClient NetworkClient { get; private set; }

    public bool IsAuthenticated { get; private set; }

    //NetworkData

    private JoinAllocation JoinAllocation;
    private Lobby joinedLobby;

    //localData
    private bool _isJoiningLobby;
    private bool _isRefreshingLobbies;



    public async Task Init()
    {
        NetworkClient = new NetworkClient(NetworkManager.Singleton);
        await AuthenticateClient();
    }

    public async Task AuthenticateClient()
    {
        InitializationOptions options = new InitializationOptions();
        options.SetProfile(UnityEngine.Random.Range(0, 100).ToString());
        await UnityServices.InitializeAsync(options);

        AuthenticationWrapper.AuthState authState = await AuthenticationWrapper.DoAuth();
        IsAuthenticated = authState == AuthenticationWrapper.AuthState.Authenticated;

        OnAuthenticating?.Invoke(IsAuthenticated);
    }

    public async Task JoinLobbyByID(string LobbyID)
    {
        if (_isJoiningLobby) return;
        try
        {

            _isJoiningLobby = true;
            OnStartToJoinLobby?.Invoke();
            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(LobbyID);
            string joinCode = joinedLobby.Data["JoinCode"].Value;

            JoinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>()
                .SetRelayServerData(new RelayServerData(JoinAllocation, "udp"));

            SetPlayerconnectiondata();
            NetworkManager.Singleton.StartClient();
            _isJoiningLobby = false;

        }
        catch (LobbyServiceException e)  
        {
            Debug.LogError(e);
            OnFailedToJoinLobby?.Invoke();
            _isJoiningLobby = false;
            return;
        }
    }


    public async Task JoinLobbyByCode(string joinCode)
    {
        if(_isJoiningLobby) return;
        try
        {
            OnStartToJoinLobby?.Invoke();
            _isJoiningLobby = true;

            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(joinCode);
            string allocationCode = joinedLobby.Data["JoinCode"].Value;

            JoinAllocation = await RelayService.Instance.JoinAllocationAsync(allocationCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(JoinAllocation, "udp"));
            SetPlayerconnectiondata();

            NetworkManager.Singleton.StartClient();
            _isJoiningLobby = false;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            OnFailedToJoinLobby?.Invoke();
            _isJoiningLobby = false;

            return;
        }
    }

    public async Task QuickJoinLobby()
    {
        if (_isJoiningLobby) return;
        try
        {
            _isJoiningLobby = true;

            OnStartToJoinLobby?.Invoke();

            QuickJoinLobbyOptions options = new QuickJoinLobbyOptions();
            options.Filter = new List<QueryFilter>()
            {
                new QueryFilter
                (
                   field: QueryFilter.FieldOptions.AvailableSlots,
                   op:QueryFilter.OpOptions.GT,
                   value:"0"
                ) ,
            };

            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync(options);
            string allocationCode = joinedLobby.Data["JoinCode"].Value;

            JoinAllocation = await RelayService.Instance.JoinAllocationAsync(allocationCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(JoinAllocation, "udp"));
            SetPlayerconnectiondata();

            NetworkManager.Singleton.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            OnFailedToJoinLobby?.Invoke();
            _isJoiningLobby = false;

            return;
        }
    }

    public async Task<List<Lobby>> ListLobbies()
    {
        if (_isRefreshingLobbies) return null;
        try
        {
            _isRefreshingLobbies = true;

            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots,"0",QueryFilter.OpOptions.GT),

                },
                Order = new List<QueryOrder>
                {
                    new QueryOrder(true,QueryOrder.FieldOptions.MaxPlayers),

                }

            };

            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);
            _isRefreshingLobbies = false;

            if(queryResponse.Results.Any()) return queryResponse.Results;
            return null;   
        }

        catch (LobbyServiceException ex)
        {
            Debug.Log(ex);
            _isRefreshingLobbies = false;
            return null;
        }
    }

    private static void SetPlayerconnectiondata()
    {
        string playerName = PlayerPrefs.GetString(LobbyCreationUI.Player_Name_Key,
                        "Player" + UnityEngine.Random.Range(1, 100));

        PlayerData playerData = new PlayerData()
        {
            playerName = playerName,
            playerAuthID = AuthenticationService.Instance.PlayerId,
            PlayerScore = 0,
        };
        byte[] payLoadByts = Encoding.UTF8.GetBytes(JsonUtility.ToJson(playerData));
        NetworkManager.Singleton.NetworkConfig.ConnectionData = payLoadByts;
    }

    public async void Dispose()
    {
        if(joinedLobby != null)
        {
            string playerId = AuthenticationService.Instance.PlayerId;
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
        }
        NetworkClient?.Dispose();
    }

    public void LeaveGame()
    {
        NetworkClient.LeaveGame();
    }
}
