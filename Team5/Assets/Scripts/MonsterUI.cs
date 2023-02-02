using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

public class MonsterUI : NetworkBehaviour {

    private GameObject monster = null;

    void Update()
    {
        if (!isServer)
        {
            gameObject.SetActive(false);
            return;
        }
        displayGameTimer();
        displayMonsterHealth();
        //displayShipLifePool();
        //displayShipLives();
        displayNumOfPlayer();
    }

    void displayGameTimer()
    {
        getTextByName("Game Timer").text = GameManager.instance.GameTime;
    }

    void displayMonsterHealth()
    {
        if (monster == null)
        {
            monster = GameObject.Find("Monster");
            return;
        }
        
        getTextByName("Monster Health").text = "Monster HP: " + monster.GetComponent<MonsterCombat>().Health;
    }

    void displayShipLifePool()
    {
        getTextByName("Ship Life Pool").text = "Ship Life Pool: " + GameManager.instance.LivesRemaining;
    }

    void displayShipLives()
    {
        int numPlayers = GameManager.instance.NumPlayers;
        for(int i = 1; i <= GameManager.NUM_JET_FIGHTERS; i++)
        {
            string playerName = "Player " + i;
            if (getTextByName(playerName) == null) continue;
            if (i > numPlayers)
            {
                getTextByName(playerName).text = "";    // player not in game yet
            } else
            {
                getTextByName(playerName).text = playerName + " Lives: " + GameObject.Find(playerName).GetComponent<ShipCombat>().LivesRemaining;
                //TODO: when player exit, numplayer is not updated
            }
        }
    }

    void displayNumOfPlayer()
    {
        getTextByName("Num Ships").text = "Ships: " + GameManager.instance.NumPlayers;
    }

    Text getTextByName(string name)
    {
        return transform.FindChild(name).GetComponent<Text>();
    }
}
