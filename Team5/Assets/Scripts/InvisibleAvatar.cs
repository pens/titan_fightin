using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class InvisibleAvatar : NetworkBehaviour {

	void Start () {
        if ((isServer && gameObject.tag == "Monster Avatar") || (isLocalPlayer && gameObject.tag == "Ship Avatar"))
        {
            MeshRenderer renderer = GetComponent<MeshRenderer>();
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        }
        
    }
}
