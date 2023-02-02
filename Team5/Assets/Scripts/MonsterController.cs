using UnityEngine;
using UnityEngine.Networking;

public class MonsterController : NetworkBehaviour {

    public GameObject controller;
    public Vector3 baseRotation;
    private const int smashDamage = 5;

    void FixedUpdate()
    {
        if (!isClient) {
            transform.position = controller.transform.position;
            transform.rotation = controller.transform.rotation * Quaternion.Euler(baseRotation.x, baseRotation.y, baseRotation.z);
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Ship Avatar"))
        {
            if (isServer)
                collider.gameObject.GetComponentInParent<ShipCombat>().KillShip();
            else if (isLocalPlayer)
                collider.gameObject.GetComponentInParent<ShipCombat>().CmdKillShip();
        }
    }
}
