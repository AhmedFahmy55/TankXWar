using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] ClientSingelton clientSingeltonPrefap;
    [SerializeField] HostSingelton hostSingeltonPrefap;





    private async void Awake()
    {
        DontDestroyOnLoad(gameObject);

        await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }

    private async Task LaunchInMode(bool isDedicatedServer)
    {
        if(isDedicatedServer)
        {

        }
        else
        {
            HostSingelton hostSingelton = Instantiate(hostSingeltonPrefap);
            hostSingelton.CreatHost();

            ClientSingelton clientSingelton = Instantiate(clientSingeltonPrefap);
            await clientSingelton.CreatCLient();

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
