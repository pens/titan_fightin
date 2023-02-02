using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MonsterController : NetworkBehaviour {

    public GameObject viveCamera;

    private Vector3 offset;

    void Start()
    {
    }

    void LateUpdate()
    {
        transform.position = viveCamera.transform.position;
        transform.rotation = viveCamera.transform.rotation;
    }
}
