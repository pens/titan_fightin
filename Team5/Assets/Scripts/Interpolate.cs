using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Interpolate : NetworkBehaviour {

    [SyncVar]
    private Vector3 syncPosition;
    [SyncVar]
    private Quaternion syncRotation;
    private static float lerpRate = 15;

    void FixedUpdate()
    {
        var rb = GetComponent<Rigidbody>();
        if (rb == null || rb.isKinematic) {
            if (!isClient)
            {
                syncPosition = transform.position;
                syncRotation = transform.rotation;
            }
            else
            {
                var latency = NetworkManager.singleton.client.GetRTT() / 1000;
                transform.position = Vector3.Lerp(transform.position, syncPosition, (Time.deltaTime + latency) * lerpRate);
                transform.rotation = Quaternion.Slerp(transform.rotation, syncRotation, (Time.deltaTime + latency) * lerpRate);
            }
        }
    }
}
