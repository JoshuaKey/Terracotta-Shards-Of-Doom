using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour {

    [Header("Speed")]
    public float Speed = 1;
    public float MaxSpeed = 10;

    [Header("Rotation")]
    public float XRotation = 75f;
    public float YRotation = 75f;
    public float YMaxRotation = 40f;

    [Header("Jump")]
    public float JumpPower = 100;
    public float JumpHoldPower = 3;

    [Header("Velocity")]
    public float Gravity = 4.5f;
    public float Dampening = 0.8f;

    private new Collider collider;
    private CharacterController controller;
    private new Camera camera;

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;

    void Start() {
        collider = GetComponent<Collider>();
        controller = GetComponent<CharacterController>();
        camera = GetComponent<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update() {
        print("Velocity: " + velocity);
        print("Rotation: " + rotation);

        UpdateCamera();
        UpdateMovment();
    }
    public void UpdateCamera() {
        float xRot = Input.GetAxis("Vertical Rotation") * YRotation * Time.deltaTime;
        float yRot = Input.GetAxis("Horizontal Rotation") * XRotation * Time.deltaTime;

        rotation += new Vector3(xRot, yRot, 0.0f);
        rotation.x = Mathf.Min(rotation.x, YMaxRotation);
        rotation.x = Mathf.Max(rotation.x, -YMaxRotation);

        this.transform.rotation = Quaternion.Euler(rotation);
    }
    public void UpdateMovment() {
        Vector3 right = transform.right * Input.GetAxis("Horizontal Movement");
        Vector3 forward = transform.forward * Input.GetAxis("Vertical Movement");
        Vector3 mvmt = forward + right;
        velocity += mvmt.normalized * Speed;

        if (controller.isGrounded) {
            if (Input.GetButtonDown("Jump")) {
                velocity.y += JumpPower;
            }

            //velocity.x = Mathf.Min(velocity.x, MaxSpeed);
            //velocity.z = Mathf.Min(velocity.z, MaxSpeed);
        } else if(velocity.y > 0.0f) {
            if (Input.GetButton("Jump")) {
                velocity.y += JumpHoldPower;
            }
        }

        velocity += Vector3.down * Gravity;

        controller.Move(velocity * Time.deltaTime);

        velocity *= Dampening;
    }

    // Test Physics based Movement...

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
}
