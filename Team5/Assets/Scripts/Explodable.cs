using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Explodable : NetworkBehaviour {

    public GameObject explosionPrefab;

    public void Explode(string collidedObject)
    {
        Debug.Assert(isServer);
        ExplodeHelper();
        RpcExplode(collidedObject);
    }

    [ClientRpc]
    void RpcExplode(string collidedObject)
    {
        Debug.Log(collidedObject);
        ExplodeHelper();
    }

    private void ExplodeHelper()
    {
        Object exp = Instantiate(explosionPrefab, transform.position, transform.rotation);
        Destroy(exp, 5f);
    }
}
