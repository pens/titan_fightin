using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class PickupCollisions : NetworkBehaviour
{
    public bool isLeft = false;

    private GameObject highlighted = null;
    public Material highlightMaterial;
    private GameObject holding = null;
    private Vector3 posOffset;
    private Quaternion initControllerRot;
    private Quaternion initPickupRot;
    
    private HashSet<GameObject> possiblePickups = new HashSet<GameObject>();
    public GameObject otherHand;
    private PickupCollisions otherHandPickup;

    [SyncVar]
    private float animPct;
    void Start()
    {
        otherHandPickup = otherHand.GetComponent<PickupCollisions>();
    }

    private GameObject choosePickup()
    {
        GameObject bestChoice = null;
        float minDistance = float.MaxValue;
        Vector3 contactPoint = transform.FindChild("ContactPoint").transform.position;
        foreach (GameObject pickup in possiblePickups)
        {
            float dist = pickup.GetComponent<Collider>().bounds.SqrDistance(contactPoint);
            if (dist < minDistance && pickup.GetComponent<ThrowableObject>()) {
                minDistance = dist;
                bestChoice = pickup;
            }
        }
        return bestChoice;
    }

    public void TriggerPress()
    {
        if (possiblePickups.Count > 0 && !HoldingSomething())
        {
            holding = highlighted;
            if (holding == null)
            {
                return;
            }
            GetComponent<AudioSource>().Play(); // grab sound
            holding.GetComponent<ThrowableObject>().Pickup(this);
            holding.GetComponent<ThrowableObject>().Unhighlight();
            highlighted = null;
            posOffset = holding.transform.position - this.transform.position;
            initControllerRot = this.transform.rotation;
            initPickupRot = holding.transform.rotation;
            holding.GetComponent<Rigidbody>().useGravity = false;
            holding.GetComponent<Rigidbody>().isKinematic = true;
            holding.GetComponent<NetworkTransform>().transformSyncMode = NetworkTransform.TransformSyncMode.SyncTransform;
            if (otherHandPickup.holding == holding)
            {
                otherHandPickup.holding = null;
            }
            RpcParent(holding);
        }
    }

    public void TriggerRelease(Vector3 velocity, Vector3 angularVelocity)
    {
        if (HoldingSomething())
        {
            var rb = holding.GetComponent<Rigidbody>();
            rb.velocity = velocity;
            rb.angularVelocity = angularVelocity;
            rb.useGravity = true;
            rb.isKinematic = false;
            holding.GetComponent<NetworkTransform>().transformSyncMode = NetworkTransform.TransformSyncMode.SyncRigidbody3D;
            RpcUnparent(holding, velocity, angularVelocity);
            holding = null;
        }
    }

    public bool HoldingSomething()
    {
        return holding != null;
    }

    void Update()
    {

        GetComponentInChildren<Animator>().Play("Armature|Grab", 0, animPct);
        if (!isServer) {
            return;
        }

        /*
            ANIMATION
            TODO - add variables for magic constants
        */
        //if (grabbing && animPct < 1.0f)
        //{
        //    animPct += .1f;
        //}
        //else if (!grabbing && animPct > 0.0f)
        //{
        //    animPct -= .1f;
        //}
        if (isLeft)
            animPct = GameObject.Find("ViveRig/Controller (left)").GetComponent<WandController>().Controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).x;
        else
            animPct = GameObject.Find("ViveRig/Controller (right)").GetComponent<WandController>().Controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).x;
        Mathf.Clamp(animPct, 0.0f, 1.0f);
        animPct /= 2;
        if (HoldingSomething())
        {
            var renderers = GetComponentsInChildren<Renderer>(true);
            foreach (var r in renderers)
                r.enabled = false;
            var canvas = GetComponentInChildren<Canvas>();
            if (canvas)
                canvas.enabled = false;
        }
        else
        {
            var renderers = GetComponentsInChildren<Renderer>(true);
            foreach (var r in renderers)
                r.enabled = true;
            var canvas = GetComponentInChildren<Canvas>(true);
            if (canvas)
                canvas.enabled = true;
        }
        /*
            END ANIMATION
        */

        if (HoldingSomething())
        {
            Quaternion controllerRotDiff = this.transform.rotation * Quaternion.Inverse(initControllerRot);
            holding.transform.position = controllerRotDiff * posOffset + this.transform.position;
            holding.transform.rotation = controllerRotDiff * initPickupRot;
        }
        else
        {
            GameObject bestChoice = choosePickup();
            if (bestChoice != highlighted)
            {
                if (highlighted != null)
                {
                    highlighted.GetComponent<ThrowableObject>().Unhighlight();
                    highlighted = null;
                }
                if (bestChoice != null)
                {
                    highlighted = bestChoice;
                    highlighted.GetComponent<ThrowableObject>().Highlight(highlightMaterial);
                }
            }
        }
    }


    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.GetComponent<ThrowableObject>() != null)
        {
            possiblePickups.Add(collider.gameObject);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.GetComponent<ThrowableObject>() != null)
        {
            possiblePickups.Remove(collider.gameObject);
        }
    }

    [ClientRpc]
    void RpcParent(GameObject holding)
    {
        holding.GetComponent<Rigidbody>().useGravity = false;
        holding.GetComponent<Rigidbody>().isKinematic = true;
        holding.GetComponent<NetworkTransform>().transformSyncMode = NetworkTransform.TransformSyncMode.SyncTransform;
    }

    [ClientRpc]
    void RpcUnparent(GameObject holding, Vector3 velocity, Vector3 angularVelocity)
    {
        holding.GetComponent<Rigidbody>().angularVelocity = angularVelocity;
        holding.GetComponent<Rigidbody>().useGravity = true;
        holding.GetComponent<Rigidbody>().isKinematic = false;
        holding.GetComponent<NetworkTransform>().transformSyncMode = NetworkTransform.TransformSyncMode.SyncRigidbody3D;
    }
}
