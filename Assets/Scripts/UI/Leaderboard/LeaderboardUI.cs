using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class LeaderboardUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Transform spawnparent;
    [SerializeField] LeaderboardItemUI itemPrefap;
    [SerializeField] Leaderboard leaderboard;


    private List<LeaderboardItemUI> leaderboardItems = new List<LeaderboardItemUI>();



    private void Awake()
    {
        leaderboard.OnLeaderboardValueChange += Leaderboard_OnLeaderVlaueChange;

    }
    private void OnDestroy()
    {
        leaderboard.OnLeaderboardValueChange -= Leaderboard_OnLeaderVlaueChange;

    }

    private void Leaderboard_OnLeaderVlaueChange(NetworkListEvent<LeaderboardItemData> changeEvent)
    {
        switch(changeEvent.Type)
        {
            case NetworkListEvent<LeaderboardItemData>.EventType.Add:
                AddLeaderboardElement(changeEvent.Value);
                break;
            case NetworkListEvent<LeaderboardItemData>.EventType.Remove:
                RemoveBoardElement(changeEvent.Value.ClientID);
                break;
            case NetworkListEvent<LeaderboardItemData>.EventType.Value:
                UpdateBoardElement(changeEvent.Value);
                break;
        }
        UpdateLeaderboardOrder(); 
    }

    private void UpdateLeaderboardOrder()
    {
        //TODO refacot leadeboard ui to work with show hode insted of mask

        leaderboardItems.Sort((x,y) => y.Score.CompareTo(x.Score));

        for(int i = 0; i < leaderboardItems.Count; i++)
        {
            leaderboardItems[i].transform.SetSiblingIndex(i);
            leaderboardItems[i].UpdateOrder();
        }

        LeaderboardItemUI leaderboardItemUI = leaderboardItems.
            FirstOrDefault(i => i.ClientID == NetworkManager.Singleton.LocalClientId);

        if (leaderboardItemUI == null) return;

        if (leaderboardItemUI.transform.GetSiblingIndex() <= 7) return;

        leaderboardItemUI.transform.SetSiblingIndex(7);


    }

    private void UpdateBoardElement(LeaderboardItemData newData)
    {
        LeaderboardItemUI leaderboardItemUI = leaderboardItems.FirstOrDefault(i => i.ClientID == newData.ClientID);
        if(leaderboardItemUI != null)
        {
            leaderboardItemUI.UpdateScore(newData.Score);
        }
    }

    private void RemoveBoardElement(ulong clientID)
    {
        LeaderboardItemUI leaderboardItemToRemove = leaderboardItems.FirstOrDefault(i => i.ClientID == clientID);

        if(leaderboardItemToRemove != null)
        {
            leaderboardItems.Remove(leaderboardItemToRemove);
            leaderboardItemToRemove.transform.SetParent(null, false);
            Destroy(leaderboardItemToRemove.gameObject);
        }
    }

    private void AddLeaderboardElement(LeaderboardItemData data)
    {
        if (leaderboardItems.Any((x) => x.ClientID == data.ClientID)) return;
        
        LeaderboardItemUI leaderboardItem = Instantiate(itemPrefap, spawnparent);
        leaderboardItem.Init(data);
        leaderboardItems.Add(leaderboardItem);
    }
}
