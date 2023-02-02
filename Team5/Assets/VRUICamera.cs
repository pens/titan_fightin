using UnityEngine;
using System.Collections;

public class VRUICamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
        string headset = SteamVR.instance.hmd_TrackingSystemName;
        if (headset == "oculus")
        {
            var cam = GameObject.Find("ViveRig/Camera (head)");
            cam.transform.SetParent(transform);
            cam.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
    }
}
