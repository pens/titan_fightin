using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ProjectileController : NetworkBehaviour {

    public int damage;
    public bool isRocket;

    void OnTriggerEnter(Collider collider)
    {
        if (!isServer)
            return;

        GameObject collidedObject = collider.gameObject;
        if (collidedObject.name.Equals("Area Boundary"))
            return;
        else if (collidedObject.CompareTag("Monster Avatar"))
            collidedObject.GetComponentInParent<MonsterCombat>().TakeDamage(damage, isRocket);
        else if (isRocket && collidedObject.CompareTag("Building"))
            collidedObject.GetComponent<ThrowableObject>().Die();

        if (isRocket)
        {
            GetComponent<Explodable>().Explode(collidedObject.name);
        }

        Destroy(gameObject);
    }
}
