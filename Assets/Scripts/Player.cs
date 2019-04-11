using Luminosity.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour {

    [Header("Speed")]
    public float MaxSpeed = 5;
    public float AccelerationFactor = 0.9f;
    public bool CanWalk = true;

    [Header("Rotation")]
    public bool Use3rdPerson = true;
    public float CameraOffset = 5f;
    public float XRotation = 75f;
    public float YRotation = 75f;
    public float YMinRotation = -40f;
    public float YMaxRotation = 40f;
    public bool CanRotate = true;

    [Header("Jump")]
    public float JumpPower = 100;
    public bool CanJump = true;

    [Header("Velocity")]
    public float Gravity = 4.5f;
    public float FallStrength = 2.5f;
    public float LowJumpStrength = 2f;
    public bool CanMove = true;

    [Header("Combat")]
    public float AttackSpeed = .5f;
    public float Damage = 1f;
    public float Health = 3f;
    public float MaxHealth = 3f;
    public Weapon weapon;
    public bool CanAttack = true;

    [Header("Interact")]
    public float InteractDistance = 2.0f;
    public LayerMask InteractLayer;
    public bool CanInteract = true;

    [Header("UI")]
    public PlayerHud HUD;

    public static Player Instance;

    private new Collider collider;
    private CharacterController controller;
    private new Camera camera;

    // Movement
    [HideInInspector]
    public Vector3 velocity = Vector3.zero;
    [HideInInspector]
    public Vector3 rotation = Vector3.zero;

    private Vector3 cameraOrigin;

    // Attack
    private float nextAttackTime = 0.0f;

    // Collision
    private int playerLayerMask;

    void Start() {
        if(Instance == null) {
            Instance = this;
        } else {
            Destroy(this.gameObject);
        }

        collider = GetComponent<Collider>();
        if (collider == null) { collider = GetComponentInChildren<Collider>(); }

        controller = GetComponent<CharacterController>();
        if (controller == null) { controller = GetComponentInChildren<CharacterController>(); }

        camera = GetComponent<Camera>();
        if (camera == null) { camera = GetComponentInChildren<Camera>(); }

        this.gameObject.layer = LayerMask.NameToLayer("Player");
        playerLayerMask = 1 << this.gameObject.layer;

        //// Camera
        Cursor.lockState = CursorLockMode.Locked;
        if (Use3rdPerson) {
            SetupThirdPerson();
        } else {
            SetupFirstPerson();
        }
        cameraOrigin = camera.transform.localPosition;
        rotation = this.transform.rotation.eulerAngles;

        //// Character controller
        controller.skinWidth = 0.08f;
        controller.slopeLimit = 90f;
        controller.stepOffset = 0.4f;

        //// Doom Movement
        MaxSpeed = 7.5f; // + 2.5 Speed Boost
        AccelerationFactor = 0.1f;

        //// Bunny Jump
        ////Gravity = 130f;
        ////JumpPower = 25f;

        //// Platform Jump
        Gravity = 30.0f;
        JumpPower = 15;
        FallStrength = 2.5f;
        LowJumpStrength = 2f;

        weapon.OnEnemyHit += OnEnemyAttack;
    }

    void Update() {
        if (CanMove) {
            UpdateMovement();
        }
        if (CanAttack) {
            UpdateCombat();
        }
        if (CanInteract) {
            UpdateInteractable();
        } 

        TestAim();
        //TestInput();
    }
    private void LateUpdate() {
        // Camera Updates Last (Makes it not Jittery...)
        if (CanRotate) {
            UpdateCamera();
        }   
    }

    public void UpdateCamera() {
        // Rotation Input
        float xRot = InputManager.GetAxis("Vertical Rotation") * YRotation * Time.deltaTime;
        float yRot = InputManager.GetAxis("Horizontal Rotation") * XRotation * Time.deltaTime;

        // Add to Existing Rotation
        rotation += new Vector3(xRot, yRot, 0.0f);

        // Constrain Rotation
        rotation.x = Mathf.Min(rotation.x, YMaxRotation);
        rotation.x = Mathf.Max(rotation.x, YMinRotation);
        if (rotation.y > 360 || rotation.y < -360) {
            rotation.y = rotation.y % 360;
        }

        this.transform.forward = Quaternion.Euler(rotation) * Vector3.forward;
    }
    public void UpdateMovement() {
        if (CanWalk) {
            DoomMovement();
        }

        if (CanJump) {
            PlatformJump();
        }
        //BunnyJump();
        

        // Add Gravity
        if (!controller.isGrounded) {
            velocity.y -= Gravity * Time.deltaTime;
        }

        // Move with Delta Time
        controller.Move(velocity * Time.deltaTime);
    }
    public void UpdateCombat() {
        if (InputManager.GetButtonDown("Attack") && Time.time >= nextAttackTime) {
            nextAttackTime = Time.time + AttackSpeed;

            StartCoroutine(weapon.Swing(AttackSpeed));
        }
    }
    public void UpdateInteractable() {
        Ray ray = new Ray(camera.transform.position, camera.transform.forward);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, InteractDistance, InteractLayer)) {
            Interactable interactable = hit.collider.GetComponent<Interactable>();

            HUD.SetInteractText("F", interactable.name);

            if (InputManager.GetButtonDown("Interact")) {
                interactable.Interact();
            }
        } else {
            HUD.DisableInteractText();
        }
    }

    public void Heal(float health) {
        Health = Mathf.Max(MaxHealth, Health + health);

        print(this.name + " (Heal): " + Health + "/" + MaxHealth);
    }
    public void TakeDamage(float damage) {
        Health -= damage;

        print(this.name + " (Damage): " + Health + "/" + MaxHealth);
    }
    public bool IsDead() {
        return Health <= 0.0f;
    }

    public void DoomMovement() {
        float rightMove = InputManager.GetAxisRaw("Vertical Movement");
        float frontMove = InputManager.GetAxisRaw("Horizontal Movement");
        //print("Forward: " + frontMove);
        //print("Right: " + rightMove);
        Vector3 movement = Vector3.zero;

        if (Mathf.Abs(rightMove) >= 0.1f || Mathf.Abs(frontMove) >= 0.1f) { // Hot Fix
            // Get Movement
            Vector3 forward = camera.transform.forward * rightMove;
            Vector3 right = camera.transform.right * frontMove;
            // Combine Forward and Right Movement
            movement = forward + right;
            // Ignore Y component
            movement.y = 0f;
            // Normalize Movement Direction
            movement = movement.normalized;
        }

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
            if (InputManager.GetButtonDown("Jump")) {
                velocity.y = JumpPower;
            }
        }
    }
    public void PlatformJump() {
        // Check for Jump
        if (controller.isGrounded) {
            if (InputManager.GetButtonDown("Jump")) {
                velocity.y = JumpPower;
            }
        }
        // By Default the Player does a "long jump" by holding the Jump Button
        else {
            // If we are falling, add more Gravity
            if (velocity.y < 0.0f) {
                velocity.y -= Gravity * (FallStrength - 1f) * Time.deltaTime;
            }
            // If we are not doing a "long jump", add more Gravity 
            else if (velocity.y > 0.0f && !InputManager.GetButton("Jump")) {
                velocity.y -= Gravity * (LowJumpStrength - 1f) * Time.deltaTime;
            }
        }
    }

    [ContextMenu("Setup First Person")]
    public void SetupFirstPerson() {
        CameraOffset = 0.1f;

        YMinRotation = -60f;
        YMaxRotation = 60f;
    }
    [ContextMenu("Setup Third Person")]
    public void SetupThirdPerson() {
        Use3rdPerson = true;

        CameraOffset = 5f;

        YMinRotation = -10f;
        YMaxRotation = 85f;

        rotation.x = 40f;
    }
    [ContextMenu("Setup Controller")]
    public void SetupController() {
        XRotation = 150f;
        YRotation = 100f;
    }
    [ContextMenu("Setup Mouse and Keyboard")]
    public void SetupMouseAndKeyboard() {
        XRotation = 75f;
        YRotation = 75f;
    }

    private void OnEnemyAttack(Enemy enemy) {
        enemy.TakeDamage(Damage);
        if (enemy.IsDead()) {
            //enemy.gameObject.SetActive(false);
        }
    }

    // Testing --------------------------------------------------------------
    private int joystickAmo = 0;
    private void TestInput() {
        if (InputManager.GetJoystickNames().Length != joystickAmo) {
            joystickAmo = InputManager.GetJoystickNames().Length;
            for (int i = 0; i < joystickAmo; i++) {
                print(InputManager.GetJoystickNames()[i] + " is Connected!");
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
        float val = InputManager.GetAxis(name);
        if (val != .0f) {
            print(name + " Axis: " + val);
        }

    }
    private void PrintButton(string name) {
        bool val = InputManager.GetButton(name);
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
        if (Physics.Raycast(ray, out hit, distance, ~playerLayerMask)) {
            print("Pointing at " + hit.collider.gameObject.name);
        }
    }

    private void OnGUI() {
        GUI.Label(new Rect(10, 10, 150, 20), "Vel: " + velocity);
        GUI.Label(new Rect(10, 30, 150, 20), "Rot: " + rotation);

        GUI.Label(new Rect(10, 50, 150, 20), "Inp: " + new Vector2(InputManager.GetAxisRaw("Vertical Movement"), InputManager.GetAxisRaw("Horizontal Movement")));
    }
}

// Rotation seems to Be good
// Controller input is working
// Jump is Ok
// Mvmt is Ok

// DOOM
// - Quick, but not Instantaneous
// - Uses Velocity
// - Velocity Persists after movement, but quickly fades
// - Head Bob
// - Speed += ((MoveDirection * MaximumSpeed) - Speed) * AccelerationFactor