using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MonsterCombat : NetworkBehaviour
{

    [SerializeField]
    GameObject leftHand;

    [SerializeField]
    GameObject rightHand;

    [SerializeField]
    GameObject head;

    public AudioClip shriek;
    public AudioClip subtleShriek;
    public AudioClip deathCries;

    private const int maxHealth = 10000;

    [SyncVar]
    private int health = maxHealth;

    void FixedUpdate()
    {
        if (health <= maxHealth / 4)
        {
            leftHand.transform.Find("Arteries").gameObject.SetActive(false);
            rightHand.transform.Find("Arteries").gameObject.SetActive(false);
            GetComponent<AudioSource>().clip = shriek;
            GetComponent<AudioSource>().Play();
        }
        else if (health <= maxHealth / 2)
        {
            leftHand.transform.Find("Veins").gameObject.SetActive(false);
            rightHand.transform.Find("Veins").gameObject.SetActive(false);
            GetComponent<AudioSource>().clip = shriek;
            GetComponent<AudioSource>().Play();
        }
        else if (health <= 3 * maxHealth / 4)
        {
            leftHand.transform.Find("Muscles").gameObject.SetActive(false);
            rightHand.transform.Find("Muscles").gameObject.SetActive(false);
            GetComponent<AudioSource>().clip = subtleShriek;
            GetComponent<AudioSource>().Play();
        }
    }

    public void TakeDamage(int amount, bool flicker = false)
    {
        if (!isServer)
        {
            return;
        }
        Flash(flicker);
        RpcFlash(flicker);
        health -= amount;
        if (health <= 0)
        {
            health = 0;
            // game over
            GameManager.instance.EndGame(true);
        }
    }

    void Flash(bool flicker)
    {
        // recursively flash all the things
        if (flicker)
        {
            head.GetComponent<Flashable>().Flicker();
        }
        else
        {
            head.GetComponent<Flashable>().Flash();
        }

        Flashable[] leftHandParts = leftHand.GetComponentsInChildren<Flashable>();
        Flashable[] rightHandParts = rightHand.GetComponentsInChildren<Flashable>();
        Debug.Assert(leftHandParts.Length == rightHandParts.Length);
        for (int i = 0; i < leftHandParts.Length; i++)
        {
            if (flicker)
            {
                leftHandParts[i].Flicker();
                rightHandParts[i].Flicker();
            }
            else
            {
                leftHandParts[i].Flash();
                rightHandParts[i].Flash();
            }

        }
        //death screams
        if (health <= 0)
        {
            GetComponent<AudioSource>().clip = deathCries;
            GetComponent<AudioSource>().Play();
        }
    }

    [ClientRpc]
    void RpcFlash(bool flicker)
    {
        Flash(flicker);
    }

    public int Health
    {
        get
        {
            return health;
        }
    }
}
