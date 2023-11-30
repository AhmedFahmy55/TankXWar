using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class HostManager : IDisposable
{
    //Events
    public event Action OnLobbyCreation;
    public event Action OnLobbyCreationFailed;

    //Settings
    private const int Max_Players = 10;
    private string Game_Scene = "Game";
    

    //Network Data
    private Allocation allocation;
    private string joinCode;

    private Lobby joinedLobby;

    public NetworkServer NetworkServer { get; private set; }


    //localData
    private bool _isCreatingLobby;




    public async Task CreateLobby(string lobbyName,bool isPrivate)
    {
        if(_isCreatingLobby) { return; }

        try
        {
            _isCreatingLobby = true;
            OnLobbyCreation?.Invoke();
            allocation = await RelayService.Instance.CreateAllocationAsync(Max_Players);
            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        }
        catch (RelayServiceException ex)
        {
            Debug.Log(ex);
            OnLobbyCreationFailed?.Invoke();
            _isCreatingLobby = false;
            return;
        }

        try
        {
            joinedLobby = await SetupLobby(lobbyName, isPrivate, joinCode);
            
            HostSingelton.Instance.StartCoroutine(LobbyHeartBeat(15));
            
        }
        catch(LobbyServiceException lobbyEx)
        {
            Debug.Log(lobbyEx);
            OnLobbyCreationFailed?.Invoke();
            _isCreatingLobby = false;
            return;
        }

        NetworkManager networkManager = NetworkManager.Singleton;

        networkManager.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "udp"));

        NetworkServer = new NetworkServer(networkManager);
        NetworkServer.OnClientDisconnect += OnClientDisconnect;


        string playerName = PlayerPrefs.GetString(LobbyCreationUI.Player_Name_Key,
                "Player" + UnityEngine.Random.Range(1, 100));

        PlayerData playerData = new PlayerData
        {
            playerName = playerName,
            playerAuthID = AuthenticationService.Instance.PlayerId,
            PlayerScore = 0,
        };

        byte[] payLoadByts = Encoding.UTF8.GetBytes(JsonUtility.ToJson(playerData));
        NetworkManager.Singleton.NetworkConfig.ConnectionData = payLoadByts;

        networkManager.StartHost();
        networkManager.SceneManager.LoadScene(Game_Scene, UnityEngine.SceneManagement.LoadSceneMode.Single);
        _isCreatingLobby = false;

    }



    IEnumerator LobbyHeartBeat(float freshreate)
    {
        WaitForSeconds delay = new WaitForSeconds(freshreate);
        while (joinedLobby != null)
        {
            LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            yield return delay;
        }
    }

    private async Task<Lobby> SetupLobby(string lobbyName, bool isPrivate,string joinCode)
    {

        CreateLobbyOptions lobbyOptions = new CreateLobbyOptions()
        {
            IsPrivate = isPrivate,
            Data = new Dictionary<string, DataObject>()
                {
                    {
                        "JoinCode", new DataObject(DataObject.VisibilityOptions.Member,joinCode)
                    },
                }
        };

        if(string.IsNullOrEmpty(lobbyName)) 
        {
            lobbyName = $"{PlayerPrefs.GetString(LobbyCreationUI.Player_Name_Key,"Missing")} Lobby";
        }

        return await LobbyService.Instance.CreateLobbyAsync(lobbyName, Max_Players, lobbyOptions);
    }




    private async Task LeaveLobby(string AuthID)
    {
        if (joinedLobby == null) return;
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthID);

        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }

    }
    private async void OnClientDisconnect(ulong clientID, string AuthID)
    {
        await LeaveLobby(AuthID);
    }

    public async void ShutDown()
    {
        HostSingelton.Instance.StopCoroutine(nameof(LobbyHeartBeat));

        try
        {
            if(joinedLobby != null) await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
        }
        catch(Exception ex) { Debug.Log(ex);}

        if(NetworkServer != null) NetworkServer.OnClientDisconnect -= OnClientDisconnect;

    }
    public  void Dispose()
    {
        ShutDown();
        NetworkServer?.Dispose();
        
    }

}
