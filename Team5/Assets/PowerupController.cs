using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PowerupController : NetworkBehaviour {

    private static float TIME_BETWEEN_POWERUPS = 20.0f;
    public GameObject shieldPowerupPrefab;
    private float lastPowerupTime = 0.0f;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (!isServer)
            return;

        if (Time.time - lastPowerupTime > TIME_BETWEEN_POWERUPS)
        {
            GameObject areaBoundary = GameObject.Find("Area Boundary");
            Vector3 areaSize = areaBoundary.GetComponent<BoxCollider>().size;
            Vector3 areaPosition = areaBoundary.transform.position;

            Vector3 randomPosition = new Vector3(areaSize.x * Random.value - areaSize.x / 2 + areaPosition.x,
                                                areaSize.y * Random.value - areaSize.y / 2 + areaPosition.y,
                                                areaSize.z * Random.value - areaSize.z / 2 + areaPosition.z);

            Debug.Log(randomPosition);

            GameObject shieldPowerup = (GameObject)Instantiate(shieldPowerupPrefab, randomPosition, gameObject.transform.rotation);
            NetworkServer.Spawn(shieldPowerup);
            Destroy(shieldPowerup, TIME_BETWEEN_POWERUPS);
            lastPowerupTime = Time.time;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isServer)
            return;

        if (other.gameObject.CompareTag("Ship Avatar"))
        {
            Debug.Log("Collided with: " + other.gameObject.tag);
        }
    }
}
