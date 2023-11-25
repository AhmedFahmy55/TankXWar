using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCreationUI : MonoBehaviour
{
    [SerializeField] InputField lobbyNameINputField;
    [SerializeField] Toggle isPrivateToggle;
    [SerializeField] Button createLobbyButton;
    [SerializeField] MessagePopupUI messagePopupUI;
    [SerializeField] TMP_InputField playerNameImputField;



    public const string Player_Name_Key = "PlayerName";

    private void Awake()
    {
        lobbyNameINputField.onValueChanged.AddListener((x) =>
        {
            createLobbyButton.interactable = !string.IsNullOrEmpty(x) 
                        && !string.IsNullOrEmpty(playerNameImputField.text) ? true : false;
        });

        playerNameImputField.onValueChanged.AddListener((x) =>
        {
            UpdatePlayerName(x);
            createLobbyButton.interactable = !string.IsNullOrEmpty(x)
                        && !string.IsNullOrEmpty(lobbyNameINputField.text) ? true : false;
        });

        createLobbyButton.onClick.AddListener(async () =>
        {
            await HostSingelton.Instance.HostManager.CreateLobby(lobbyNameINputField.text, isPrivateToggle.isOn);

        });
    }
    private void Start()
    {
        HostSingelton.Instance.HostManager.OnLobbyCreation += OnlobbyCreation;
        HostSingelton.Instance.HostManager.OnLobbyCreationFailed += OnlobbyCreationFailed;

        UpdatePlayerName(PlayerPrefs.GetString(Player_Name_Key,string.Empty));

        playerNameImputField.text = PlayerPrefs.GetString(Player_Name_Key, string.Empty);

        createLobbyButton.interactable = !string.IsNullOrEmpty(lobbyNameINputField.text)
            && !string.IsNullOrEmpty(playerNameImputField.text) ? true : false;
    }

    private void UpdatePlayerName(string newName)
    {
        PlayerPrefs.SetString(Player_Name_Key, newName);
    }

    private void OnDestroy()
    {
        if(HostSingelton.Instance) HostSingelton.Instance.HostManager.OnLobbyCreation -= OnlobbyCreation;
        if (HostSingelton.Instance) HostSingelton.Instance.HostManager.OnLobbyCreationFailed -= OnlobbyCreationFailed;
    }

    private void OnlobbyCreation()
    {

        messagePopupUI.PopUpWithTryingMessage("Creating Lobby...");
    }

    private void OnlobbyCreationFailed()
    {
        messagePopupUI.PopUpWithEndMessage("Failed to create Lobby please try again");
    }



}
