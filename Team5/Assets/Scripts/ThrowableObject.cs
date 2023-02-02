using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ThrowableObject : NetworkBehaviour {
    private const int throwDamage = 5;
    private Vector3 initPosition;
    private Quaternion initRotation;
    private Material unhighlighted;

    enum State { Alive, Dead, Respawning, OutOfBounds }
    private State state = State.Alive;
    private float curTime;
    private const float deadTime = 15;
    private const float respawnTime = 5;
    private const float outOfBoundsTime = 5;
    private Rigidbody rb;
    private PickupCollisions pickupCollisions;

    void Start()
    {
        initPosition = transform.position;
        initRotation = transform.rotation;
        rb = GetComponent<Rigidbody>();
        unhighlighted = GetComponent<MeshRenderer>().material;
    }

    public void Die()
    {
        if (pickupCollisions != null)
        {
            Debug.Log("killing held building");
            pickupCollisions.TriggerRelease(Vector3.zero, Vector3.zero);
        }
        curTime = 0;
        state = State.Dead;
        rb.freezeRotation = true;
        rb.isKinematic = true;
        transform.position = new Vector3(0, -100, 0);
        transform.rotation = initRotation;
        GetComponent<MeshRenderer>().enabled = false;
    }

    public void Pickup(PickupCollisions pickup)
    {
        state = State.Alive;
        rb.freezeRotation = false;
        GetComponent<BoxCollider>().isTrigger = false;
        pickupCollisions = pickup;
    }

    public void Release(PickupCollisions pickup)
    {
        Debug.Assert(pickup == pickupCollisions);
        pickupCollisions = null;
    }

    public void Highlight(Material highlight)
    {
        GetComponent<MeshRenderer>().material = highlight;
    }

    public void Unhighlight()
    {
        GetComponent<MeshRenderer>().material = unhighlighted;
    }

    void Update()
    {
        if (!isServer)
        {
            return;
        }
        switch(state)
        {
            case State.Respawning:
                curTime += Time.deltaTime;
                if (curTime > respawnTime)
                {
                    state = State.Alive;
                    rb.freezeRotation = false;
                    rb.isKinematic = false;
                    transform.position = initPosition;
                    GetComponent<BoxCollider>().isTrigger = false;
                    return;
                }
                // between 0 and 1
                float t = curTime / respawnTime;
                transform.position = initPosition + (t - 1) * GetComponent<Collider>().bounds.size.y * Vector3.up;
                break;
            case State.Dead:
                curTime += Time.deltaTime;
                if (curTime > deadTime)
                {
                    curTime = 0;
                    state = State.Respawning;
                    GetComponent<MeshRenderer>().enabled = true;
                    GetComponent<BoxCollider>().isTrigger = true;
                }
                break;
            case State.OutOfBounds:
                curTime += Time.deltaTime;
                if (curTime > outOfBoundsTime)
                {
                    Die();
                }
                break;
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (!isServer) return;
        if (other.gameObject.name == "Terrain" || other.gameObject.tag == "Building")
        {
            GetComponent<AudioSource>().volume = .2f;
            GetComponent<AudioSource>().Play();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.name.Equals("Area Boundary") && state == State.Alive)
        {
            curTime = 0;
            state = State.OutOfBounds;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Equals("Area Boundary") && state == State.OutOfBounds)
        {
            curTime = 0;
            state = State.Alive;
        }
    }
}
