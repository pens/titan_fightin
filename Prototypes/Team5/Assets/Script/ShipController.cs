using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ShipController : NetworkBehaviour {

    public float speed;
    private Rigidbody rb;
    public Behaviour[] componentsToDisable;

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();

        if(!isLocalPlayer)
        {
            Debug.Log("YOLO");
            for(int i = 0; i < componentsToDisable.Length; i++)
            {
                componentsToDisable[i].enabled = false;
                Debug.Log("WE DISABLIN");
            }
            Debug.Log("WE DONE");
        }
    }

    void FixedUpdate()

    {
      //if (!isLocalPlayer)
      //  {
      //      return;
      //  }

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        rb.AddForce(movement * speed);
    }
}