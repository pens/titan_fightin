using UnityEngine;
using System.Collections;

public class GameOverUI : MonoBehaviour {

    public GameObject controller;

    // Update is called once per frame
    void Update () {
        transform.position = Camera.main.transform.position + 30 * transform.forward;
        transform.rotation = Camera.main.transform.rotation;
    }
}
