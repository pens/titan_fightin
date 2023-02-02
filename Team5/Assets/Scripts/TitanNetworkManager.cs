using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


public class TitanNetworkManager : NetworkManager
{
    public GameObject viveRig;
    public bool isDummyServer;
    public bool isDummyClient;

    private GameObject[] spawnPoints;
    GameObject menuUI;
    GameObject debugUI;

    void Start()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag("Spawn Point");
        menuUI = transform.Find("Network UI").gameObject;
        debugUI = transform.Find("Debug UI").gameObject;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        //if (isDummyServer)
        //{
        //    GameObject.Find("ViveRig").SetActive(false);
        //} else
        //{
        //    GameObject.Find("ViveRig").SetActive(true);
        //}

        //viveRig.SetActive(true);
    }

    public override void OnStartClient(NetworkClient client)
    {
        base.OnStartClient(client);
        //if (SteamVR.enabled && !isDummyClient)
        //{
        //    GameObject.Find("ViveRig").SetActive(true);
        //} else
        //{
        //    GameObject.Find("ViveRig").SetActive(false);
        //}
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        Transform startpos = spawnPoints[Random.Range(0, spawnPoints.Length)].transform;
        Debug.Log(startpos);
        var player = (GameObject)Instantiate(playerPrefab, startpos.position, startpos.rotation);
        Debug.Log(player);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        EnableMenuUI();
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        Debug.Log("Called here");
        GameManager.instance.RemovePlayer();
        base.OnServerDisconnect(conn);
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        EnableMenuUI();
    }

    private void EnableMenuUI ()
    {
        PlayerSelect.playing = false;
        Debug.Log("menuui" + menuUI);
        menuUI.SetActive(true);
        menuUI.GetComponent<Canvas>().enabled = true;
        debugUI.SetActive(true);
    }

}