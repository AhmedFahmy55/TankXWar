using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConnectingUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI messagetext;
    [SerializeField] Button retryButton, playOfflineButton;
    [SerializeField] GameObject spinner;




    private void Awake()
    {
        retryButton.onClick.AddListener(async () =>
        {
            messagetext.text = "Connecting...";
            spinner.SetActive(true);
            await ClientSingelton.Instance.ClientManager.AuthenticateClient();
        });

        playOfflineButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
    }

    private void Start()
    {
        if (ClientSingelton.Instance.ClientManager == null) return;

        ClientSingelton.Instance.ClientManager.OnAuthenticating += ClientSingelton_OnAuthenticating;

        if(ClientSingelton.Instance.ClientManager.IsAuthenticated)
        {
            gameObject.SetActive(false);

        }
        else
        {
            spinner.SetActive(false);
            messagetext.text = "Failed to connect!";
            retryButton.gameObject.SetActive(true);
            playOfflineButton.gameObject.SetActive(true);
        }
    }
    private void OnDestroy()
    {
        ClientSingelton.Instance.ClientManager.OnAuthenticating -= ClientSingelton_OnAuthenticating;
    }

    private void ClientSingelton_OnAuthenticating(bool isAuthanticated)
    {
        if(!isAuthanticated)
        {
            spinner.SetActive(false);
            messagetext.text = "Failed to connect!";
            retryButton.gameObject.SetActive(true);
            playOfflineButton.gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }



}
