using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ShipCombat))]
public class ShipUIManager : MonoBehaviour {

    [SerializeField]
    private bool showHealth;

    [SerializeField]
    private Text gameTimer;

    [SerializeField]
    private Text outofboundTimer;

    [SerializeField]
    private Text monsterHealth;

    void Update()
    {
        displayGameTimer();
        displayOutOfBoundTimer();
        displayLives();
        displayMonsterHP();
    }

    void displayGameTimer()
    {
        gameTimer.text = GameManager.instance.GameTime;
    }

    void displayOutOfBoundTimer()
    {
        outofboundTimer.text = GetComponent<ShipCombat>().OutOfBoundsTimer;
    }

    void displayMonsterHP()
    {
        //transform.Find("Monster HP Canvas")
        //    .Find("Monster HP")
        //    .GetComponent<Text>().text = GameObject.Find("Monster").GetComponent<MonsterCombat>().Health.ToString();
        monsterHealth.text = GameObject.Find("Monster").GetComponent<MonsterCombat>().Health.ToString();
    }

    void displayLives()
    {
        int numPlayers = GameManager.instance.NumPlayers;
        GameObject livesUI = transform.Find("Lives Canvas").gameObject;
        string text = "You: " + GetComponent<ShipCombat>().LivesRemaining;
        livesUI.transform.Find("Lives").GetComponent<Text>().text = text;


        //for (int i = 1; i <= GameManager.NUM_JET_FIGHTERS; i++)
        //{
        //    string playerName = "Player " + i;
        //    string text = "";
        //    if (i <= numPlayers)
        //    {
        //        if (playerName.Equals(GetComponent<ShipCombat>().PlayerName))
        //        {
        //            text = "You: " + GameObject.Find(playerName).GetComponent<ShipCombat>().LivesRemaining;
        //        }
        //        else
        //        {
        //            text = playerName + ": " + GameObject.Find(playerName).GetComponent<ShipCombat>().LivesRemaining;
        //        }
        //    }
        //    livesUI.transform.FindChild(playerName).GetComponent<Text>().text = text;
        //}
    }
}
