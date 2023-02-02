using UnityEngine;
using System.Collections;

public class SphereController : MonoBehaviour
{

    public float accelerationSpeed;
    public float brakeSpeed;
    public float rotateSpeed;
    public float steadySpeed;
    private Rigidbody rb;

    public GameObject projectile;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float shoot = Input.GetAxis("RightTrigger");

        if(shoot > 0)
        {
            GameObject newProjectile = Instantiate(projectile);
            newProjectile.SetActive(true);
            newProjectile.transform.position = transform.position + (1.0f * transform.forward);
            newProjectile.transform.rotation = transform.rotation;
            newProjectile.GetComponent<Rigidbody>().velocity = 100 * transform.forward;
        }
    }

    void FixedUpdate()
    {
        // Get button inputs
        float accelerate = Input.GetAxis("AButton");
        float brake = Input.GetAxis("BButton");
        float rotateHorizontal = Input.GetAxis("LeftJoystickX");
        float rotateVertical = Input.GetAxis("LeftJoystickY");
        float roll = Input.GetAxis("RightJoystickX");

        // Set the movement depending on if the ship is accelerating or braking.
        Vector3 movement = new Vector3(0.0f, 0.0f, accelerate * accelerationSpeed);
        if(brake != 0)
        {
            // Brake only if the ship is moving
            if (rb.velocity.magnitude > 0)
            {
                movement.z = -brake * brakeSpeed;
            }
        }
        rb.AddRelativeForce(movement);

        // Set the torque of the ship
        Vector3 torque = new Vector3(rotateVertical, rotateHorizontal, roll);
        rb.AddRelativeTorque(torque * rotateSpeed);

        if(torque.magnitude == 0)
        {
            rb.AddTorque(-steadySpeed * rb.angularVelocity);
        }

        // Set the direction of the object in the forward direction
        rb.velocity = transform.forward * rb.velocity.magnitude;
    }
}