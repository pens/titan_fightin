using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class BackgroundMusic : NetworkBehaviour {

    public AudioClip noPlayers;
    public AudioClip battleMonster;
    public AudioClip battleShip;
    public AudioClip gameover;

    private AudioSource music;
    private bool battling = false;


	// Use this for initialization
	void Start () {
        music = GetComponent<AudioSource>();
        music.clip = noPlayers;
        music.volume = .1f;
        music.Play();
	}
	
	// Update is called once per frame
	void Update () {
	    if (GameManager.instance.NumPlayers > 0 && music.clip.Equals(noPlayers))
        {
            if (isServer)
            {
                music.clip = battleMonster;
                music.volume = .05f;
            } else
            {
                music.clip = battleShip;
            }
            music.Play();
            battling = true;
        } else if (battling && !GameManager.instance.IsGameRunning)
        {
            battling = false;
            music.clip = gameover;
            music.volume = .1f;
            music.Play();
        }
	}
}
