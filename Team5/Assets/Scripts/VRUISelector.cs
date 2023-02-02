using UnityEngine;
using System.Collections;

public class VRUISelector : MonoBehaviour {

    int highlightedHash = Animator.StringToHash("Highlighted");
    int normalHash = Animator.StringToHash("Normal");

    Animator anim;
    Animator thisAnim;
    [SerializeField]
    Animator otherAnim;

    void Start()
    {
        string headset = SteamVR.instance.hmd_TrackingSystemName;
        if (headset == "oculus")
        {
            transform.localScale = new Vector3(100, 100, 100);
            transform.position = transform.position - (Vector3.right * 25);
        } else if (headset == "lighthouse")
        {
            transform.localScale = new Vector3(25, 25, 25);
            //transform.position = transform.position + (Vector3.up * 10);
        }
        else
        {
            Debug.LogError("Unrecognized headset");
        }
    }

	void FixedUpdate()
    {
        if (PlayerSelect.playing)
        {
            return;
        }
        Vector3 fwd = GameObject.Find("Camera (head)/Camera (eye)").transform.TransformDirection(Vector3.forward);

        RaycastHit hitInfo;
        if (Physics.Raycast(GameObject.Find("Camera (head)/Camera (eye)").transform.position, fwd, out hitInfo))
        {
            print("There is something in front of the object!");
            string name = hitInfo.transform.name;
            if (name == "Monster" || name == "Space_ship")
            {
                Debug.Log(name);
                Animator prevanim = hitInfo.collider.gameObject.GetComponentInParent<Animator>();
                if (anim && anim != prevanim) anim.SetTrigger(normalHash);
                anim = prevanim;
                anim.SetTrigger(highlightedHash);
            }
            else
            {
                //if (anim) anim.SetTrigger(normalHash);
            }
        }
        else
        {
            //if (anim) anim.SetTrigger(normalHash);
        }
    }

}
