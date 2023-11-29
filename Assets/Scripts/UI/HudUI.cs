using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HudUI : MonoBehaviour
{
    [SerializeField] private Button leaveGameButton;



    private void Awake()
    {
        leaveGameButton.onClick.AddListener(() =>
        {
            if(NetworkManager.Singleton.IsHost)
            {
                HostSingelton.Instance.HostManager.ShutDown();
            }

            ClientSingelton.Instance.ClientManager.LeaveGame();
        });
    }
}
