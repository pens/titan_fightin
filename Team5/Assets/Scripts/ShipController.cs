using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ShipController : NetworkBehaviour
{
    private GameObject shipCamera;

    private Rigidbody rb;
    //public bool vrEnabled;
    public bool strafeMode;


    // Laser shooting
    public GameObject laser;
    private float lastLaserShotTime = 0.0f;
    public float laserFireRate; // lasers per second
    public float laserSpeed;
    private bool fireFromGunSpawn1 = true;
    private GameObject gunSpawn1;
    private GameObject gunSpawn2;

    // Missile shooting
    public GameObject missile;
    private float lastMissileShotTime = 0.0f;
    public float missileFireRate;
    public float missileSpeed;
    private GameObject missileSpawn;

    //speed stuff
    private bool freshlySpawned = true;
    float speed;
    public int cruiseSpeed;
    float deltaSpeed;//(speed - cruisespeed)
    public int minSpeed;
    public int maxSpeed;
    float accel, decel;

    //stablizing
    public float stabilizeRate;
    public float autostabilizeRate;
    bool stabilizing;
    bool autostabilizing = true;

    //turning stuff
    Vector3 angVel;
    Vector3 shipRot;
    public int sensitivity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gunSpawn1 = transform.Find("GunSpawn1").gameObject;
        gunSpawn2 = transform.Find("GunSpawn2").gameObject;
        missileSpawn = transform.Find("MissileSpawn").gameObject;

        if (isLocalPlayer)
        {
            Transform cameraRigTransform = transform.Find("CameraRig");

            shipCamera = GameObject.Find("ViveRig/Camera (head)");
            if (shipCamera == null || !shipCamera.activeInHierarchy)
            {
                shipCamera = new GameObject("Camera (head)");
                shipCamera.AddComponent<Camera>();
            }
            shipCamera.transform.SetParent(cameraRigTransform);
            shipCamera.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            GameObject eye = GameObject.Find("Camera (eye)");
            if (eye != null)
            {
                eye.GetComponent<Camera>().nearClipPlane = 0.01f;
            }

            // Must position/rotate camera manually for monitor play
            shipCamera.transform.position = cameraRigTransform.position;
            shipCamera.transform.rotation = cameraRigTransform.rotation;

            Destroy(transform.Find("Ship Model").gameObject);
            speed = 0;
        } else
        {
            Destroy(transform.Find("Cockpit").gameObject);
        }

    }

    public void FreshlySpawned()
    {
        freshlySpawned = true;
    }

    [Command]
    void CmdFireLaser(float speed)
    {
        // Alternate which gun to shoot  from.
        GameObject gunSpawn = fireFromGunSpawn1 ? gunSpawn1 : gunSpawn2;
        fireFromGunSpawn1 = !fireFromGunSpawn1;

        var newLaser = (GameObject)Instantiate(laser,
            gunSpawn.transform.position, gunSpawn.transform.rotation);
        newLaser.GetComponent<Rigidbody>().velocity = (speed + laserSpeed) * transform.forward;

        NetworkServer.Spawn(newLaser);
        Destroy(newLaser, 5.0f);
    }

    [Command]
    void CmdFireMissile(float speed)
    {
        // TODO: Refactor this so that we don't duplicate laser code.
        var newMissile = (GameObject)Instantiate(missile,
            missileSpawn.transform.position, missileSpawn.transform.rotation);
        newMissile.GetComponent<Rigidbody>().velocity = (speed + missileSpeed) * transform.forward;

        NetworkServer.Spawn(newMissile);
        Destroy(newMissile, 5.0f);
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            float shootLaser = Input.GetAxis("RightTrigger");
            float shootMissile = Input.GetAxis("LeftTrigger");

            float currentTime = Time.time;
            if (shootLaser > 0 && currentTime - lastLaserShotTime >= (1 / laserFireRate))
            {
                CmdFireLaser(speed);
                lastLaserShotTime = currentTime;
            }

            if (shootMissile > 0 && currentTime - lastMissileShotTime >= (1 / missileFireRate))
            {
                CmdFireMissile(speed);
                lastMissileShotTime = currentTime;
            }

            // set or unset autostabilize with back button
            if (Input.GetKeyDown(KeyCode.Joystick1Button6))
            {
                autostabilizing = !autostabilizing;
                stabilizing = false;
            }
        }
        // change volume regardless of whether you are local player
        // TODO: calculate speed for !localPlayer case?
        float engineVolume = .2f * Mathf.Clamp(speed / maxSpeed, 0.0f, 1.0f);
        GetComponent<AudioSource>().volume = engineVolume;
    }

    void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            shipRot = transform.localEulerAngles;

            //since angles are only stored (0,360), convert to +- 180
            if (shipRot.x > 180) shipRot.x -= 360;
            if (shipRot.y > 180) shipRot.y -= 360;
            if (shipRot.z > 180) shipRot.z -= 360;

            //vertical stick adds to the pitch velocity
            //         (*************************** this *******************************) is a nice way to get the square without losing the sign of the value
            float turnY = Input.GetAxis("LeftJoystickY") * Mathf.Abs(Input.GetAxis("LeftJoystickY")) * sensitivity * Time.fixedDeltaTime;
            angVel.x += turnY;

            //horizontal stick adds to the roll and yaw velocity... also thanks to the .5 you can't turn as fast/far sideways as you can pull up/down
            float turnX = Input.GetAxis("LeftJoystickX") * Mathf.Abs(Input.GetAxis("LeftJoystickX")) * sensitivity * Time.fixedDeltaTime;

            if (turnX != 0 || turnY != 0 || Input.GetAxis("AButton") > 0)
            {
                freshlySpawned = false;
            }

            angVel.y += turnX * 2f;
            angVel.z -= turnX * .5f;

            // reduce angular velocity over time if not turning
            if (turnX == 0)
            {
                angVel.x *= .99f;
            }
            if (turnY == 0)
            {
                angVel.y *= .99f;
            }

            //shoulder buttons add to the roll and yaw.  No deltatime here for a quick response
            //comment out the .y parts if you don't want to turn when you hit them
            if (Input.GetKey(KeyCode.Joystick1Button4))
            {
                //angVel.y -= 20;
                angVel.z += 50;
                speed -= 5 * Time.fixedDeltaTime;
            }
            if (Input.GetKey(KeyCode.Joystick1Button5))
            {
                //angVel.y += 20;
                angVel.z -= 50;
                speed -= 5 * Time.fixedDeltaTime;
            }

            // NO
            /*
            lel bf controls!
            */
            /*float turnY2 = Input.GetAxis("RightJoystickY") * Mathf.Abs(Input.GetAxis("RightJoystickY")) * sensitivity * Time.fixedDeltaTime;
            angVel.x += turnY2;
            angVel.z += Input.GetAxis("RightJoystickX") * 50;
            */
            /*
            end bf controls
            */

            //your angular velocity is higher when going slower, and vice versa.  There probably exists a better function for this.
            angVel /= 1 + deltaSpeed * .001f;

            //this is what limits your angular velocity.  Basically hard limits it at some value due to the square magnitude, you can
            //tweak where that value is based on the coefficient
            angVel -= angVel.normalized * angVel.sqrMagnitude * .08f * Time.fixedDeltaTime;

            //and finally rotate.  
            transform.Rotate(angVel * Time.fixedDeltaTime);

            // stabilize roll if steadying
            if (Input.GetKey(KeyCode.Joystick1Button8) && !autostabilizing)
            {
                stabilizing = true;
            }
            if (stabilizing)
            {
                transform.Rotate(0, 0, -stabilizeRate * shipRot.z * Time.fixedDeltaTime);
                if (Mathf.Abs(shipRot.z) < 2)
                {
                    stabilizing = false;
                }
            }
            // auto-stabilize
            if (!Input.GetKey(KeyCode.Joystick1Button4) && !Input.GetKey(KeyCode.Joystick1Button5) && Mathf.Abs(shipRot.z) >= 2 && autostabilizing)
            {
                transform.Rotate(0, 0, -autostabilizeRate * shipRot.z);
            }

            //LINEAR DYNAMICS//

            deltaSpeed = speed - cruiseSpeed;

            //This, I think, is a nice way of limiting your speed.  Your acceleration goes to zero as you approach the min/max speeds, and you initially
            //brake and accelerate a lot faster.  Could potentially do the same thing with the angular stuff.
            decel = speed - minSpeed;
            accel = maxSpeed - speed;

            //simple accelerations
            if (Input.GetAxis("AButton") > 0)
            {
                speed += accel * Time.fixedDeltaTime;
            }
            else if (Input.GetAxis("BButton") > 0)
            {
                speed -= decel * Time.fixedDeltaTime;
            }
            else if (Mathf.Abs(deltaSpeed) > .1f && !freshlySpawned)
            {
                //if not accelerating or decelerating, tend toward cruise, using a similar principle to the accelerations above
                //(added clamping since it's more of a gradual slowdown/speedup)
                speed -= Mathf.Clamp(deltaSpeed * Mathf.Abs(deltaSpeed), -30, 100) * Time.fixedDeltaTime;
            }

            // this is what actually makes the whole ship move through the world!
            transform.Translate(transform.forward * speed * Time.fixedDeltaTime, Space.World);
        }
    }
}