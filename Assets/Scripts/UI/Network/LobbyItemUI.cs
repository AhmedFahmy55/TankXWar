using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyItemUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI lobbyNameText;
    [SerializeField] TextMeshProUGUI lobbyNumbText;
    [SerializeField] Button joinButton;

    private Lobby _lobby;



    private void Start()
    {

        joinButton.onClick.AddListener(async () =>
        {
            if (_lobby != null) await ClientSingelton.Instance.ClientManager.JoinLobbyByID(_lobby.Id);

        });
    }
    public void Init(Lobby lobby)
    {
        _lobby = lobby;
        lobbyNameText.text = _lobby.Name;
        lobbyNumbText.text = $"{_lobby.Players.Count}/10";
    }
}
