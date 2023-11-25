using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class HostSingelton : MonoBehaviour
{

    public static HostSingelton Instance { get; private set; }

    public HostManager HostManager { get; private set; }


    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnDestroy()
    {
        HostManager?.Dispose();
    }

    public void CreatHost()
    {
        HostManager = new HostManager();
    }

}
