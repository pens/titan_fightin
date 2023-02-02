using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ShipCombat : NetworkBehaviour {

    // Constant
    private const float RESPAWN_TIME = 5.0f; // in seconds
    private const float OUTOFBOUND_TIME = 15f;
    private const int NUM_LIVES = 98;

    // Ship's private state
    [SyncVar]
    private bool isAlive = true;
    [SyncVar]
    private int livesRemaining = NUM_LIVES;

    private float outOfBoundsTimer;
    private bool outOfBounds = false;
    private GameObject[] spawnPoints;

    // Controlled state
    [SerializeField]
    private GameObject explosionPrefab;

    [SyncVar(hook = "OnChangeName")]
    string playerName;

    private bool isSpectator;

    public string PlayerName
    {
        get
        {
            return playerName;
        }
    }

    public void Start()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag("Spawn Point");
        if (!isLocalPlayer)
        {
            OnChangeName(playerName);
        }
    }

    public override void OnStartLocalPlayer()
    {
        CmdSetName();
    }

    [Command]
    public void CmdSetName()
    {
        GameManager.instance.AddPlayer();
        playerName = "Player " + GameManager.instance.NumPlayers;
        transform.name = playerName;
    }

    public void OnChangeName(string playerName)
    {
        transform.name = playerName;
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        if (outOfBounds)
        {
            outOfBoundsTimer += Time.deltaTime;
            if (outOfBoundsTimer > OUTOFBOUND_TIME)
            {
                OutOfBounds = false;
                CmdKillShip();
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        bool collidingWithTerrain = collider.gameObject.name.Equals("Terrain");
        bool collidingWithBuilding = collider.gameObject.CompareTag("Building");
        if (collidingWithTerrain || collidingWithBuilding)
        {
            if (isServer)
                KillShip();
            else if (isLocalPlayer)
                CmdKillShip();

            // TODO: This could be called multiple times, once for each computer that sees the ship hit the building.
            if(collidingWithBuilding)
                collider.gameObject.GetComponent<ThrowableObject>().Die();
        }
    }

    public void KillShip()
    {
        if (isAlive)
        {
            isAlive = false;
            livesRemaining--;
            StartCoroutine(Explode(transform.position, transform.rotation));
            GameManager.instance.PlayerDeath();
            if (livesRemaining <= 0)
            {
                GameManager.instance.PlayerOutOfLives();
            }
            RpcDie(transform.position, transform.rotation);
        }
    }

    // Tells the server to kill this ship and respawn it.
    [Command]
    public void CmdKillShip()
    {
        KillShip();
    }

    void resetCameraTransformForMonitor(GameObject shipCamera)
    {
        shipCamera.transform.localPosition = Vector3.zero;
        shipCamera.transform.rotation = shipCamera.transform.parent.rotation;
    }

    IEnumerator WaitForRespawn()
    {
        BecomeSpectator();

        yield return new WaitForSeconds(RESPAWN_TIME);

        // Respawn the ship.
        if (livesRemaining > 0)
        {
            GameObject overviewCameraPosition = GameObject.Find("Overview Camera Position");
            overviewCameraPosition.transform.Find("Canvas").gameObject.SetActive(false);

            Respawn();
        }
    }

    void BecomeSpectator()
    {
        transform.position = new Vector3(-1000.0f, -1000.0f, -1000.0f);
        GameObject overviewCameraPosition = GameObject.Find("Overview Camera Position");
        overviewCameraPosition.transform.Find("Canvas").gameObject.SetActive(true);
        GameObject shipCamera = GameObject.Find("Camera (head)");
        shipCamera.transform.SetParent(overviewCameraPosition.transform);
        resetCameraTransformForMonitor(shipCamera);
    }

    void Respawn()
    {
        GameObject spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GetComponent<ShipController>().FreshlySpawned();
        transform.position = spawnPoint.transform.position;
        transform.rotation = spawnPoint.transform.rotation;
        CmdSetAlive();
        GameObject shipCamera = GameObject.Find("Camera (head)");
        shipCamera.transform.SetParent(transform.Find("CameraRig"));
        resetCameraTransformForMonitor(shipCamera);
        
    }

    [Command]
    void CmdSetAlive()
    {
        isAlive = true;
    }

    public bool IsAlive()
    {
        return isAlive;
    }

    [ClientRpc]
    void RpcDie(Vector3 oldPos, Quaternion oldRot)
    {
        if (isLocalPlayer)
        {
            StartCoroutine("WaitForRespawn");
        } 

        // Play the explosion where the ship died.
        StartCoroutine(Explode(oldPos, oldRot));
    }

    IEnumerator Explode(Vector3 pos, Quaternion rot)
    {
        yield return new WaitForSeconds(0.1f);
        Object exp = Instantiate(explosionPrefab, pos, rot);
        Destroy(exp, 5f);
    }

    /******************** Out of bounds *************************/
    /************************************************************/
    void OutOfBoundsEffect()
    {
        if (outOfBounds)
        {
            transform.Find("Shockwave").GetComponent<ParticleSystem>().Play();
            transform.Find("OutofBound Canvas").gameObject.SetActive(true);
        }
        else
        {
            transform.Find("Shockwave").GetComponent<ParticleSystem>().Stop();
            transform.Find("OutofBound Canvas").gameObject.SetActive(false);
        }
    }

    // setter for out of bound
    public bool OutOfBounds
    {
        set
        {
            outOfBounds = value;
            outOfBoundsTimer = 0f;
            OutOfBoundsEffect();
        }
    }

    // getter for out of bound timer
    public string OutOfBoundsTimer
    {
        get
        {
            return string.Format("{0:d}", (int)(OUTOFBOUND_TIME - outOfBoundsTimer));
        }
    }

    public int LivesRemaining
    {
        get
        {
            return livesRemaining;
        }
    }
}
