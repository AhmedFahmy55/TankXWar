using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class LeaderboardItemUI : MonoBehaviour
{


    [SerializeField] TextMeshProUGUI displayText;

    [SerializeField] Color displayColor = Color.green;

    public ulong ClientID { get; private set; }
    public int Score { get; private set; }


    private string _playerName;

    public void Init(LeaderboardItemData data)
    {
        ClientID = data.ClientID;
        _playerName = data.PlayerName.ToString();
        if(ClientID == NetworkManager.Singleton.LocalClientId)
        {
            displayText.color = displayColor;
        }
        UpdateScore(data.Score);
    }

    public void UpdateScore(int newScore)
    {
        Score = newScore;
        displayText.text = $"{ClientID + 1}.{_playerName} ({Score})";
    }

    public void UpdateOrder()
    {
        displayText.text = $"{transform.GetSiblingIndex() + 1}.{_playerName} ({Score})";
    }

}
