using UnityEngine;
using System.Collections;

public class GameOver : MonoBehaviour {

	
	// Update is called once per frame
	void Update () {
        transform.position = Camera.main.transform.position + 30 * Camera.main.transform.forward;
        transform.rotation = Camera.main.transform.rotation;
    }
}
