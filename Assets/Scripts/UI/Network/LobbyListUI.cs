using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyListUI : MonoBehaviour
{
    [SerializeField] LobbyItemUI lobbyItemUIPrefap;
    [SerializeField] Transform lobbyItemParent;
    [SerializeField] MessagePopupUI messagePopupUI;
    [SerializeField] InputField joinCodeInputField;
    [SerializeField] Button refreshButton , quickJoinButton,joinByCodeButton;
    [SerializeField] float refreshRate = 2.0f;



    private float _timeSinceLastrefresh;



    private void OnEnable()
    {
        RefreshLobbisList();
    }

    private void Start()
    {
        ClientSingelton.Instance.ClientManager.OnStartToJoinLobby += ClientManager_OnStartToJoinLobby;
        ClientSingelton.Instance.ClientManager.OnFailedToJoinLobby += ClientManager_OnFailedToJoinLobby;

        joinCodeInputField.onValueChanged.AddListener((x) =>
        {
            joinByCodeButton.interactable = string.IsNullOrEmpty(x) ? false : true;
        });

        refreshButton.onClick.AddListener(RefreshLobbisList);

        quickJoinButton.onClick.AddListener(async () =>
        {
            await ClientSingelton.Instance.ClientManager.QuickJoinLobby();
        });

        joinByCodeButton.onClick.AddListener(async () =>
        {
            await ClientSingelton.Instance.ClientManager.JoinLobbyByCode(joinCodeInputField.text);
        });
    }
    private void OnDestroy()
    {
        if (ClientSingelton.Instance) ClientSingelton.Instance.ClientManager.OnStartToJoinLobby += ClientManager_OnStartToJoinLobby;
        if (ClientSingelton.Instance) ClientSingelton.Instance.ClientManager.OnFailedToJoinLobby += ClientManager_OnFailedToJoinLobby;
    }

    private void Update()
    {
        _timeSinceLastrefresh += Time.deltaTime;

        if (_timeSinceLastrefresh > refreshRate)
        {
            RefreshLobbisList();
            _timeSinceLastrefresh = 0;
        }
    }

    private void ClientManager_OnFailedToJoinLobby()
    {
        messagePopupUI.PopUpWithEndMessage("Failed to join lobby ps try again");
    }

    private void ClientManager_OnStartToJoinLobby()
    {
        messagePopupUI.PopUpWithTryingMessage("Joining Lobby...");
    }

    public async void RefreshLobbisList()
    {
        List<Lobby> lobbies = await ClientSingelton.Instance.ClientManager.ListLobbies();
        if (lobbies == null) return;

        foreach (Transform chield in  lobbyItemParent)
        {
            Destroy(chield.gameObject);
        }
        foreach (Lobby lob in lobbies)
        {
            LobbyItemUI lobbyItem = Instantiate(lobbyItemUIPrefap, lobbyItemParent);
            lobbyItem.Init(lob);
        }
    }
}
