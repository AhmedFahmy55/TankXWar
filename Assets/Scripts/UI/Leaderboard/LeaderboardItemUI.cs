using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardItemUI : MonoBehaviour
{


    [SerializeField] TextMeshProUGUI displayText;

    public ulong ClientID { get; private set; }


    private string playerName;

    public void Init(LeaderboardItemData data)
    {
        ClientID = data.ClientID;
        playerName = data.PlayerName.ToString();
        UpdateScore(data.Score);
    }

    public void UpdateScore(int newScore)
    {
        displayText.text = $"{ClientID + 1}.{playerName} ({newScore})";
    }

}
