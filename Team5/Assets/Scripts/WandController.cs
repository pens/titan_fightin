using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class WandController : NetworkBehaviour {

    private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;

    private SteamVR_Controller.Device controller {
        get {
            if (trackedObj != null)
            {
                return SteamVR_Controller.Input((int)trackedObj.index);
            }
            return null;
        }
    }

    public SteamVR_Controller.Device Controller {
        get {
            return controller;
        }
    }

    private SteamVR_TrackedObject trackedObj;

    public GameObject hand;
    private PickupCollisions pickupCollisions;
    public GameObject handUi;

    [SerializeField]
    private bool inMenu;

    // Use this for initialization
    void Start()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        if (hand)
            pickupCollisions = hand.GetComponent<PickupCollisions>();
        Physics.gravity = new Vector3(0, -10, 0) * 50;
    }

    // Update is called once per frame
    void Update()
    {
        if (controller == null)
        {
            Debug.Log("Controller not initialized");
            return;
        }

        if (inMenu)
        {
            // Don't want to do any pickupcollision
            return;
        }

        if (controller.GetPressDown(triggerButton))
        {
            pickupCollisions.TriggerPress();
        }
        else if (controller.GetPressUp(triggerButton))
        {
            pickupCollisions.TriggerRelease(controller.velocity * 50, controller.angularVelocity * 50);
        }

        if (controller.GetPressDown(gripButton))
        {
            if (handUi != null)
            {
                handUi.SetActive(!handUi.activeInHierarchy);
            }
        }
    }

    public bool GetTrigger()
    {
        if (controller == null)
        {
            return false;
        }
        return controller.GetPressDown(triggerButton);
    }
}
