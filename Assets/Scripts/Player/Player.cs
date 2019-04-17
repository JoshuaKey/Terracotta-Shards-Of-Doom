using Luminosity.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour {

    [Header("Movement")]
    public bool CanMove = true;
    public bool CanWalk = true;
    public float MaxSpeed = 7.5f; // + 2.5 Speed Boost
    public float AccelerationFactor = 0.1f;

    [Header("Jump")]
    public bool CanJump = true;
    public float JumpPower = 15;
    public float FallStrength = 2.5f;
    public float LowJumpStrength = 2f;
    public float Gravity = 30f;

    [Header("Rotation")]
    public bool CanRotate = true;
    public float XRotationSpeed = 60f;
    public float YRotationSpeed = 60f;
    public float YMinRotation = -40f;
    public float YMaxRotation = 40f;
    public new Camera camera;

    [Header("Combat")]
    public bool CanAttack = true;
    public int CurrWeaponIndex = 0;
    public Health health;
    public GameObject WeaponParent;

    [Header("Interact")]
    public float InteractDistance = 2.0f;
    public LayerMask InteractLayer;
    public bool CanInteract = true;

    //[Header("UI")]
    //public PlayerHud HUD;

    public static Player Instance;

    [HideInInspector]
    public Vector3 velocity = Vector3.zero;
    [HideInInspector]
    public Vector3 rotation = Vector3.zero;
    [HideInInspector]
    public List<Weapon> weapons = new List<Weapon>();

    private new Collider collider;
    private CharacterController controller;
    private int playerLayerMask;

    void Start() {
        if (Instance != null) { Destroy(this.gameObject); return; }
        Instance = this;

        if (collider == null) { collider = GetComponentInChildren<Collider>(true); }

        if (controller == null) { controller = GetComponentInChildren<CharacterController>(true); }

        if (health == null) { health = GetComponentInChildren<Health>(true); }

        if (camera == null) { camera = GetComponentInChildren<Camera>(true); }

        weapons.AddRange(GetComponentsInChildren<Weapon>(true));
        foreach(Weapon w in weapons) { w.gameObject.SetActive(false); }
        CurrWeaponIndex = Mathf.Min(weapons.Count - 1, CurrWeaponIndex);
        SwapWeapon(CurrWeaponIndex);

        // Physics
        playerLayerMask = 1 << this.gameObject.layer;

        // Camera
        Cursor.lockState = CursorLockMode.Locked;
        rotation = this.transform.rotation.eulerAngles;

        this.health.OnDeath += this.Die;
        this.health.OnDamage += ChangeHealthUI;
        this.health.OnHeal += ChangeHealthUI;
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

        if (Input.GetKeyDown(KeyCode.T)) {
            this.health.TakeDamage(DamageType.TRUE, 0.5f);
        }
    }
    private void LateUpdate() {
        if (CanRotate) {
            UpdateCamera();
        }   
    }

    public void UpdateCamera() {
        // Rotation Input
        float xRot = InputManager.GetAxis("Vertical Rotation") * YRotationSpeed * Time.deltaTime;
        float yRot = InputManager.GetAxis("Horizontal Rotation") * XRotationSpeed * Time.deltaTime;

        // Add to Existing Rotation
        rotation += new Vector3(xRot, yRot, 0.0f);

        // Constrain Rotation
        rotation.x = Mathf.Min(rotation.x, YMaxRotation);
        rotation.x = Mathf.Max(rotation.x, YMinRotation);
        if (rotation.y > 360 || rotation.y < -360) {
            rotation.y = rotation.y % 360;
        }

        camera.transform.forward = Quaternion.Euler(rotation) * Vector3.forward;
    }
    public void UpdateMovement() {
        if (CanWalk) {
            Walk();
        }

        if (CanJump) {
            Jump();
        }      

        // Add Gravity
        if (!controller.isGrounded) {
            velocity.y -= Gravity * Time.deltaTime;
        }

        // Move with Delta Time
        controller.Move(velocity * Time.deltaTime);
    }
    public void UpdateCombat() {
        // Check for Switch
        if (InputManager.GetButtonDown("Next Weapon")) {
            int nextIndex = CurrWeaponIndex + 1 >= weapons.Count ? 0 : CurrWeaponIndex + 1;
            SwapWeapon(nextIndex);
        }
        if (InputManager.GetButtonDown("Prev Weapon")) {
            int prevIndex = CurrWeaponIndex - 1 < 0 ? weapons.Count - 1 : CurrWeaponIndex - 1;
            SwapWeapon(prevIndex);
        }

        // Check for Attack
        Weapon weapon = GetCurrentWeapon();
        if (weapon.CanAttack()) {
            if (weapon.CanCharge) {
                if (InputManager.GetButton("Attack")) {
                    weapon.Charge();
                }
                if (InputManager.GetButtonUp("Attack")) {
                    weapon.Attack();
                }
            } else {
                if (InputManager.GetButton("Attack")) {
                    weapon.Attack();
                }
            }
        }
    }
    public void UpdateInteractable() {
        Ray ray = new Ray(camera.transform.position, camera.transform.forward);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, InteractDistance, InteractLayer)) {
            Interactable interactable = hit.collider.GetComponentInChildren<Interactable>(true);
            if (interactable == null) { interactable = hit.collider.GetComponentInParent<Interactable>(); }

            if (interactable.CanInteract) {
                PlayerHud.Instance.SetInteractText("F", interactable.name);

                if (InputManager.GetButtonDown("Interact")) {
                    interactable.Interact();
                }
            } else {
                PlayerHud.Instance.DisableInteractText();
            }

        } else {
            PlayerHud.Instance.DisableInteractText();
        }
    }

    public void Walk() {
        float rightMove = InputManager.GetAxisRaw("Vertical Movement");
        float frontMove = InputManager.GetAxisRaw("Horizontal Movement");
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
    public void Jump() {
        // Check for Jump
        if (controller.isGrounded) {
            if (InputManager.GetButtonDown("Jump")) {
                velocity.y = JumpPower;
            } else {
                velocity.y = -1;
            }
        }
        // By Default the Player does a "long jump" by holding the Jump Button
        else {
            // If we are falling, add more Gravity
            if (velocity.y < 0.0f) {
                velocity.y -= Gravity * (FallStrength - 1f) * Time.deltaTime;
                //Debug.Break();
            }
            // If we are not doing a "long jump", add more Gravity 
            else if (velocity.y > 0.0f && !InputManager.GetButton("Jump")) {
                velocity.y -= Gravity * (LowJumpStrength - 1f) * Time.deltaTime;
            }
        }
    }

    public void ChangeHealthUI(float val) {
        PlayerHud.Instance.SetPlayerHealth(this.health.CurrentHealth / this.health.MaxHealth);
    }
    public void Die() {
        LevelManager.Instance.RestartLevel();
    }

    public void LookTowards(Vector3 forward) {
        camera.transform.forward = forward;
        rotation = camera.transform.rotation.eulerAngles;

        if (rotation.x > 180) { rotation.x = rotation.x - 360; }
        if (rotation.x < -180) { rotation.x = rotation.x + 360; }
    }

    public void AddWeapon(Weapon newWeapon) {
        weapons.Add(newWeapon);

        newWeapon.gameObject.SetActive(false);
        newWeapon.transform.SetParent(WeaponParent.transform, false);
    }
    public void SwapWeapon(int index) {
        Weapon oldWeapon = GetCurrentWeapon();
        oldWeapon.gameObject.SetActive(false);
        oldWeapon.transform.SetParent(WeaponParent.transform, false);

        CurrWeaponIndex = index;

        Weapon newWeapon = GetCurrentWeapon();
        newWeapon.gameObject.SetActive(true);
        newWeapon.transform.SetParent(camera.transform, false);
    }
    public Weapon GetCurrentWeapon() {
        return weapons[CurrWeaponIndex];
    }

    [ContextMenu("Setup Controller")]
    public void SetupControllerRotation() {
        XRotationSpeed = 150f;
        YRotationSpeed = 100f;
    }
    [ContextMenu("Setup Mouse and Keyboard")]
    public void SetupMouseRotation() {
        XRotationSpeed = 75f;
        YRotationSpeed = 75f;
    }

    // Testing --------------------------------------------------------------
    private void OnGUI() {
        GUI.Label(new Rect(10, 10, 150, 20), "Vel: " + velocity);
        GUI.Label(new Rect(10, 30, 150, 20), "Rot: " + rotation);

        GUI.Label(new Rect(10, 50, 150, 20), "Inp: " + new Vector2(InputManager.GetAxisRaw("Vertical Movement"), InputManager.GetAxisRaw("Horizontal Movement")));
    }

    private void OnTriggerEnter(Collider other) {
        print("Player Collision: " + other.name);
    }


}