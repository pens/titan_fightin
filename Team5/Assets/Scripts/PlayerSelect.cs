using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerSelect : MonoBehaviour {

    NetworkManager manager;
    GameObject menuUI;
    GameObject debugUI;
    InputField ip;

    Animator anim;
    public static bool playing = false;
    int highlightedHash = Animator.StringToHash("Base Layer.Highlighted");

    WandController rightWand;
    WandController leftWand;

	void Start () {
        manager = GetComponentInParent<NetworkManager>();
        menuUI = manager.transform.Find("Network UI").gameObject;
        debugUI = manager.transform.Find("Debug UI").gameObject;
        ip = debugUI.GetComponentInChildren<InputField>();
        anim = GetComponent<Animator>();
    }

    public void PlayTitan()
    {
        menuUI.SetActive(false);
        debugUI.SetActive(false);
        Debug.Log("titan");
        manager.StartServer();
    }

    public void PlayJet()
    {
        menuUI.gameObject.SetActive(false);
        debugUI.SetActive(false);
        Debug.Log("jet");
        manager.networkAddress = ip.text;
        manager.StartClient();
    }

    public void PlayJetNoTitan()
    {
        manager.StartHost();
    }

    void Update()
    {
        if (!anim || playing) {
            return;
        }

        if (!leftWand)
        {
            GameObject leftController = GameObject.Find("ViveRig/Controller (left)");
            if (leftController) leftWand = leftController.GetComponent<WandController>();
        }

        if (!rightWand)
        {
            GameObject rightController = GameObject.Find("ViveRig/Controller (right)");
            if (rightController) rightWand = rightController.GetComponent<WandController>();
        }

        AnimatorStateInfo animInfo = anim.GetCurrentAnimatorStateInfo(0);
        float select = Input.GetAxis("AButton");

        // Check if we have a controller and get the trigger value
        bool wandTrigger = false;
        if (rightWand)
        {
            wandTrigger |= rightWand.GetTrigger();
        }
        if (leftWand)
        {
            wandTrigger |= leftWand.GetTrigger();
        }
        // End of trigger checking

        if (animInfo.fullPathHash == highlightedHash && (select != 0 || wandTrigger))
        {
            //GetComponent<Camera>().cullingMask = 1 << 8;
            playing = true;
            anim.SetTrigger("Pressed");
        }
    }
}
