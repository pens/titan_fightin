using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;

    // constant for game settings
    public const float GAMEPLAY_TIME = 240f; // 4 minutes
    public const float RESPAWN_TIME = 5f;
    public const float NUM_JET_FIGHTERS = 4;
    public const int NUM_LIVES = 1000;

    [SyncVar]
    private int livesRemaining; // a shared pool of lives between all ships

    private int playersRemaining;

    // Scenes name
    private string playerWinScene = "titanloses";
    private string playerLoseScene = "titanwins";

    private NetworkManager networkManager;

    // Game state
    [SyncVar]
    private float gametime;
    public string GameTime
    { // return the game time in time formatted string
        get
        {
            return string.Format("{0:0}:{1:00}", (int)gametime / 60, (int)gametime % 60);
        }
    }

    public bool IsGameRunning
    {
        get
        {
            return isGameRunning;
        }
    }

    [SyncVar]
    private int numPlayer = 0;
    [SyncVar]
    private bool isGameRunning;

    // UI

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("There are more than one Game manager in the scene");
        } else
        {
            instance = this;
        }
    }

    void Start()
    {
        gametime = GAMEPLAY_TIME + RESPAWN_TIME;
        //numPlayer = 0;
        isGameRunning = false;
        livesRemaining = NUM_LIVES;

        networkManager = FindObjectOfType<TitanNetworkManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (isServer)
            {
                EndServer();
            }
            else
            {
                EndClient();
            }
        }

        if (!isGameRunning)
            return;

        Debug.Assert(numPlayer > 0);

        gametime -= Time.deltaTime;

        if (gametime < 0)
        {
            EndGame(false);
        }

        // TODO: replace with the real UI
        //GameObject.Find("Game Timer").GetComponent<Text>().text = GameTime;
    }

    public void StartGame()
    {
        if (isGameRunning)
            return;

        Debug.Assert(numPlayer > 0);

        gametime = GAMEPLAY_TIME + RESPAWN_TIME;
        isGameRunning = true;
    }

    public void EndGame(bool isPlayerWin)
    {
        isGameRunning = false;
        //GameObject.Find("SimpleTown Stuff").SetActive(false);
        if (isPlayerWin)
        {
            //networkManager.ServerChangeScene(playerWinScene);
            GameObject.Find("Game Over").transform.Find("Result").gameObject.GetComponent<Text>().text = "Ships Win!";
        }
        else
        {
            //networkManager.ServerChangeScene(playerLoseScene);
            GameObject.Find("Game Over").transform.Find("Result").gameObject.GetComponent<Text>().text = "Titan Wins!";
        }
        GameObject.Find("Game Over").SetActive(true);
        if (isServer)
        {
            RpcEndGame(isPlayerWin);
        }
    }

    [ClientRpc]
    public void RpcEndGame(bool isPlayerWin)
    {
        EndGame(isPlayerWin);
    }

    public void EndServer()
    {
        networkManager.StopServer();
    }

    public void EndClient()
    {
        networkManager.StopClient();
    }

    public bool AddPlayer()
    {
        if (numPlayer <= NUM_JET_FIGHTERS)
        {
            numPlayer++;
            playersRemaining++;
            StartGame();
            return true;
        }

        return false;
    }

    public void RemovePlayer()
    {
        numPlayer--;
        if (numPlayer <= 0)
        {
            gametime = 0;
        }
    }

    public void PlayerDeath()
    {
        livesRemaining--;
    }

    public void PlayerOutOfLives()
    {
        playersRemaining--;
        if (playersRemaining <= 0 && numPlayer > 0)
        {
            EndGame(false);
        }
    }

    public int LivesRemaining
    {
        get
        {
            return livesRemaining;
        }
    }

    public int NumPlayers
    {
        get
        {
            return numPlayer;
        }
    }
}