using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour {

    // Slope Offset = 45 (Also Affect Upward block Movement)
    // Step Offset = .4 (Allows Moving onto a block .5 Meters high and .75 at Edge)
    // Skin Width = .2  OR .08 (?, Causes the Collider to "float")
    // Min Move Distance (Aka, We have to move at least this much, or the controller wont actually move)
    // 

    [Header("Speed")]
    public float Speed = 1;
    public float MaxSpeed = 5;
    public float AccelerationFactor = 0.9f;

    [Header("Rotation")]
    public float XRotation = 75f;
    public float YRotation = 75f;
    public float YMaxRotation = 40f;

    [Header("Jump")]
    public float JumpPower = 100;

    [Header("Velocity")]
    public float Gravity = 4.5f;
    public float FallStrength = 2.5f;
    public float LowJumpStrength = 2f;
    public float Dampening = 0.8f;

    private new Collider collider;
    private CharacterController controller;
    private new Camera camera;

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;

    private int playerLayer;

    void Start() {
        collider = GetComponent<Collider>();
        if (collider == null) { collider = GetComponentInChildren<Collider>(); }

        controller = GetComponent<CharacterController>();
        if (controller == null) { controller = GetComponentInChildren<CharacterController>(); }

        camera = GetComponent<Camera>();
        if (camera == null) { camera = GetComponentInChildren<Camera>(); }

        playerLayer = LayerMask.NameToLayer("Player");

        Cursor.lockState = CursorLockMode.Locked;

        // Doom Movement
        MaxSpeed = 20f;
        AccelerationFactor = 0.1f;

        // Bunny Jump
        //Gravity = 130f;
        //JumpPower = 25f;

        // Platform Jump
        Gravity = 30.0f;
        JumpPower = 15;
        FallStrength = 2.5f;
        LowJumpStrength = 2f;
    }

    void Update() {
        UpdateCamera();
        UpdateMovment();

        TestAim();
        //TestInput();
    }
    public void UpdateCamera() {
        FirstPersonCamera();
        //ThirdPersonCamera();
    }
    public void FirstPersonCamera() {
        float xRot = Input.GetAxis("Vertical Rotation") * YRotation * Time.deltaTime;
        float yRot = Input.GetAxis("Horizontal Rotation") * XRotation * Time.deltaTime;

        rotation += new Vector3(xRot, yRot, 0.0f);
        rotation.x = Mathf.Min(rotation.x, YMaxRotation);
        rotation.x = Mathf.Max(rotation.x, -YMaxRotation);
        if (rotation.y > 360 || rotation.y < -360) {
            rotation.y = rotation.y % 360;
        }

        this.transform.rotation = Quaternion.Euler(rotation);
    }

    public void UpdateMovment() {
        DoomMovement();

        //BunnyJump();
        PlatformJump();

        // Add Gravity
        if (!controller.isGrounded) {
            velocity.y -= Gravity * Time.deltaTime;
        }

        // Move with Delta Time
        controller.Move(velocity * Time.deltaTime);

        // Test Dampening
    }

    public void DoomMovement() {
        // Get Movement
        Vector3 forward = this.transform.forward * Input.GetAxisRaw("Vertical Movement");  
        Vector3 right = this.transform.right * Input.GetAxisRaw("Horizontal Movement");
        // Combine Forward and Right Movement
        Vector3 movement = forward + right; 
        // Ignore Y component
        movement.y = 0f; 
        // Normalize Movement Direction
        movement = movement.normalized;

        // Ignore Y Component
        Vector3 currMovement = velocity;
        currMovement.y = 0f;

        // This Equation moves us towards out desired Movement based off our curr Velocity.
        // The Acceleration allows us to move instantly (1), or build up (.1)
        velocity += ((movement * MaxSpeed) - currMovement) * AccelerationFactor;
    }

    public void BunnyJump() {
        // Check for Jump
        if (controller.isGrounded) {
            if (Input.GetButtonDown("Jump")) {
                velocity.y = JumpPower;
            }
        }
    }
    public void PlatformJump() {
        // Check for Jump
        if (controller.isGrounded) {
            if (Input.GetButtonDown("Jump")) {
                velocity.y = JumpPower;
            }
        }

        // By Default the Player does a "long jump" by holding the Jump Button

        // If we are falling, add more Gravity
        if(velocity.y < 0.0f) {
            velocity.y -= Gravity * (FallStrength - 1f) * Time.deltaTime;
        } 
        // If we are not doing a "long jump", add more Gravity 
        else if(velocity.y > 0.0f && !Input.GetButton("Jump")) {
            velocity.y -= Gravity * (LowJumpStrength - 1f) * Time.deltaTime;
        }
    }

    // Testing... --------------------------------------------------------------
    private int joystickAmo = 0;
    private void TestInput() {
        if(Input.GetJoystickNames().Length != joystickAmo) {
            joystickAmo = Input.GetJoystickNames().Length;
            for (int i = 0; i < joystickAmo; i++) {
                print(Input.GetJoystickNames()[i] + " is Connected!");
            }
        }    

        PrintAxis("Horizontal Movement");
        PrintAxis("Vertical Movement");
        PrintAxis("Horizontal Rotation");
        PrintAxis("Vertical Rotation");

        PrintAxisAsButton("Attack");

        PrintButton("Jump");
        PrintButton("Submit");
        PrintButton("Cancel");

        // We can use manual button and axis names as well, "joystick 1 button 0"
    }
    private void PrintAxis(string name) {
        float val = Input.GetAxis(name);
        if (val != .0f) {
            print(name + " Axis: " + val);
        }

    }
    private void PrintButton(string name) {
        bool val = Input.GetButton(name);
        if (val) {
            print(name + " is Pressed!");
        }
    }
    private void PrintAxisAsButton(string name) {
        bool val = InputExt.GetAxisAsButton(name);
        if (val) {
            print(name + " is Pressed!");
        }
    }

    private void TestAim() {
        Ray ray = new Ray(camera.transform.position, camera.transform.forward);
        float distance = 10;

        Debug.DrawRay(ray.origin, ray.direction * distance, Color.green);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, distance, playerLayer)) {
            print("Pointing at " + hit.collider.gameObject.name);
        }
    }

    private void OnGUI() {
        GUI.Label(new Rect(10, 10, 150, 20), "Vel: " + velocity);
        GUI.Label(new Rect(10, 30, 150, 20), "Rot: " + rotation);
    }
}

// Rotation seems to Be good
// Controller input is working

// Fix Jump Feel
// Fix Mvmt Feel
// Test Controller Mvmt and Jump Feel
// Combat

// How exactly do we want the Movement?
// Like doom?
// Like Slime Rancher 
// Like Banjo and Kazooie?

// DOOM
// - Quick, but not Instantaneous
// - Uses Velocity
// - Velocity Persists after movement, but quickly fades
// - Head Bob
// - Speed += ((MoveDirection * MaximumSpeed) - Speed) * AccelerationFactor