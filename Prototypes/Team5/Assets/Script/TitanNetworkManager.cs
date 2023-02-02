using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


public class TitanNetworkManager : NetworkManager
{
    public GameObject viveCamera;

    public override void OnStartServer()
    {
        Debug.Log("Hello!");
        viveCamera.SetActive(true);
    }

    
}