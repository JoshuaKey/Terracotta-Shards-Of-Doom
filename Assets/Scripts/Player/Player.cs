using Luminosity.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour {

    [Header("Movement")]
    public bool CanMove = true;
    public bool CanWalk = true;
    public float MaxSpeed = 7.5f; // + 2.5 Speed Boost
    public float AccelerationFactor = 0.1f;
    public float MaxVelocity = 100.0f;

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
    public bool CanSwapWeapon = false;
    public int CurrWeaponIndex = 0;
    public float WeaponWheeltimeScale = .5f;
    public Health health;
    public GameObject WeaponParent;

    [Header("Interact")]
    public float InteractDistance = 2.0f;
    public LayerMask InteractLayer;
    public CompassPot compass;
    public bool CanInteract = true;

    [Space]
    [Header("Debug")]
    public Sword SwordPrefab;
    public Bow BowPrefab;
    public Hammer HammerPrefab;
    public Spear SpearPrefab;
    public CrossBow CrossBowPrefab;
    public Magic MagicPrefab;

    public static Player Instance;

    [HideInInspector]
    public Vector3 velocity = Vector3.zero;
    [HideInInspector]
    public Vector3 rotation = Vector3.zero;
    [HideInInspector]
    public List<Weapon> weapons = new List<Weapon>();
    [HideInInspector]
    public int layerMask;

    private new Collider collider;
    private CharacterController controller;
    private Vector2 weaponWheelRotation = Vector2.zero;

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

        Weapon newWeapon = GetCurrentWeapon();
        newWeapon.gameObject.SetActive(true);
        newWeapon.transform.SetParent(camera.transform, false);

        string[] weaponNames = weapons.Select(x => x.name).ToArray();
        PlayerHud.Instance.SetWeaponWheel(weaponNames);
        PlayerHud.Instance.DisableWeaponWheel();
        if (weapons.Count > 1) {
            PlayerHud.Instance.EnableWeaponToggle();
            CanSwapWeapon = true;
        } else {       
            PlayerHud.Instance.DisableWeaponToggle();
            CanSwapWeapon = false;
        }  

        // Physics
        layerMask = 1 << this.gameObject.layer;

        // Camera
        Cursor.lockState = CursorLockMode.Locked;
        rotation = this.transform.rotation.eulerAngles;

        PlayerHud.Instance.EnablePlayerHealthBar();
        this.health.OnDeath += this.Die;
        this.health.OnDamage += ChangeHealthUI;
        this.health.OnHeal += ChangeHealthUI;
    }

    void Update() {
        if (CanMove) {
            UpdateMovement();
        }
        UpdateCombat();
        if (CanInteract) {
            UpdateInteractable();
        }
        if (InputManager.GetButtonDown("UI_Cancel"))
        {
            if (PauseMenu.Instance.activeSelf)
            {
                PauseMenu.Instance.SetActive(false);
            }
            else
            {
                PauseMenu.Instance.SetActive(true);
            }
        }

        // Debug...
        if (Application.isEditor) {
            if (Input.GetKeyDown(KeyCode.T)) {
                this.health.TakeDamage(DamageType.TRUE, 0.5f);
            }
            if (Input.GetKeyDown(KeyCode.Z)) {
                Weapon w = GameObject.Instantiate(BowPrefab);
                w.transform.SetParent(WeaponParent.transform, false);
                AddWeapon(w);
            }
            if (Input.GetKeyDown(KeyCode.X)) {
                Weapon w = GameObject.Instantiate(HammerPrefab);
                w.transform.SetParent(WeaponParent.transform, false);
                AddWeapon(w);
            }
            if (Input.GetKeyDown(KeyCode.C)) {
                Weapon w = GameObject.Instantiate(SpearPrefab);
                w.transform.SetParent(WeaponParent.transform, false);
                AddWeapon(w);
            }
            if (Input.GetKeyDown(KeyCode.V)) {
                Weapon w = GameObject.Instantiate(CrossBowPrefab);
                w.transform.SetParent(WeaponParent.transform, false);
                AddWeapon(w);
            }
            if (Input.GetKeyDown(KeyCode.B)) {
                Weapon w = GameObject.Instantiate(MagicPrefab);
                w.transform.SetParent(WeaponParent.transform, false);
                AddWeapon(w);
            }
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
        Walk();

        if (CanJump) {
            Jump();
        }      

        // Add Gravity
        if (!controller.isGrounded) {
            velocity.y -= Gravity * Time.deltaTime;
        }

        velocity = Vector3.ClampMagnitude(velocity, MaxVelocity);

        // Move with Delta Time
        controller.Move(velocity * Time.deltaTime);
    }
    public void UpdateCombat() {
        Weapon weapon = GetCurrentWeapon();

        // Check for Weapon Swap
        if (CanSwapWeapon && weapon.CanAttack()) {
            // Weapon Toggle
            if (InputManager.GetButtonDown("Next Weapon")) {
                int nextIndex = CurrWeaponIndex + 1 >= weapons.Count ? 0 : CurrWeaponIndex + 1;
                SwapWeapon(nextIndex);
            }
            if (InputManager.GetButtonDown("Prev Weapon")) {
                int prevIndex = CurrWeaponIndex - 1 < 0 ? weapons.Count - 1 : CurrWeaponIndex - 1;
                SwapWeapon(prevIndex);
            }

            // Weapon Wheel
            if (InputManager.GetButtonDown("Weapon Wheel")) {
                Time.timeScale = WeaponWheeltimeScale;
                PlayerHud.Instance.EnableWeaponWheel();
                CanRotate = false;
                CanAttack = false;
            } 
            else if (InputManager.GetButtonUp("Weapon Wheel")) {
                Time.timeScale = 1f;
                PlayerHud.Instance.DisableWeaponWheel();
                CanRotate = true;
                CanAttack = true;
                weaponWheelRotation = Vector3.zero;
            } 
            else if (InputManager.GetButton("Weapon Wheel")) {
                UpdateWeaponWheelRotation();

                int index = -1;
                float currAngle = 0;
                if (weaponWheelRotation != Vector2.zero) {
                    float weaponAngle = Mathf.PI * 2 / weapons.Count;
                    currAngle = Mathf.Atan2(weaponWheelRotation.x, weaponWheelRotation.y);
                    
                    if(currAngle < 0) {
                        currAngle = Mathf.PI * 2 + currAngle;
                    }

                    index = (int)(currAngle / weaponAngle);
                }
                PlayerHud.Instance.HighlightWeaponWheel(index, weaponWheelRotation);
                if(index != -1) {
                    this.SwapWeapon(index);
                }
            }

            // Number Bar
            if (InputManager.GetButtonDown("Weapon1")) { 
                SwapWeapon(0);
            }
            if (InputManager.GetButtonDown("Weapon2")) {
                SwapWeapon(1);
            }
            if (InputManager.GetButtonDown("Weapon3")) {
                SwapWeapon(2);
            }
            if (InputManager.GetButtonDown("Weapon4")) {
                SwapWeapon(3);
            }
            if (InputManager.GetButtonDown("Weapon5")) {
                SwapWeapon(4);
            }
            if (InputManager.GetButtonDown("Weapon6")) {
                SwapWeapon(5);
            }

            // Scroll Wheel
            if (Input.GetAxis("Mouse ScrollWheel") != 0.0f) {
                int index = (int)(CurrWeaponIndex + Input.mouseScrollDelta.y);
                if(index >= weapons.Count) {
                    index = 0;
                } else if (index < 0) {
                    index = weapons.Count - 1;
                }
                SwapWeapon(index);
            }
        }

        // Check for Attack
        if (CanAttack) {          
            if (weapon.CanAttack()) {
                if (weapon.CanCharge) {
                    if (InputManager.GetButton("Attack")) {
                        weapon.Charge();
                    } else {
                        weapon.Attack();
                    }
                } else {
                    if (InputManager.GetButton("Attack")) {
                        weapon.Attack();
                    }
                }
            }
        }
    }
    public void UpdateInteractable() {
        // Check for Compass
        {
            if (InputManager.GetButtonDown("Compass")) {
                compass.Activate(-camera.transform.forward);
            } 
            else if (compass.gameObject.activeInHierarchy) {
                Transform target;
                if (EnemyManager.Instance.MainProgression.IsComplete()) {
                    target = EnemyManager.Instance.MainProgression.ProgressionObject.transform;
                } else {
                    target = EnemyManager.Instance.GetClosestEnemy(this.transform.position).transform;
                }
                Vector3 dir = target.position - this.transform.position;
                dir = dir.normalized;
                compass.SetDirection(dir);
            }
        }


        // Check for interactable
        {
            Ray ray = new Ray(camera.transform.position, camera.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, InteractDistance, InteractLayer)) {
                Interactable interactable = hit.collider.GetComponentInChildren<Interactable>(true);
                if (interactable == null) { interactable = hit.collider.GetComponentInParent<Interactable>(); }

                if (interactable.CanInteract) {
                    PlayerHud.Instance.SetInteractText("f", interactable.name);

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
    }

    public void Walk() {
        Vector3 movement = Vector3.zero;
        float rightMove = 0.0f;
        float frontMove = 0.0f;

        if (CanWalk) {
            rightMove = InputManager.GetAxisRaw("Vertical Movement");
            frontMove = InputManager.GetAxisRaw("Horizontal Movement");
        }      

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

    public void ChangeHealthUI(float val) {
        PlayerHud.Instance.SetPlayerHealthBar(this.health.CurrentHealth / this.health.MaxHealth);
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

    public Vector3 UpdateWeaponWheelRotation() {
        // Rotation Input
        float yRot = -InputManager.GetAxis("Vertical Rotation") * YRotationSpeed * Time.deltaTime;
        float xRot = InputManager.GetAxis("Horizontal Rotation") * XRotationSpeed * Time.deltaTime;

        // Add to Existing Rotation
        weaponWheelRotation += new Vector2(xRot, yRot);
        weaponWheelRotation = weaponWheelRotation.normalized;

        return weaponWheelRotation;
    }
    public void AddWeapon(Weapon newWeapon) {
        Player.Instance.CanSwapWeapon = true;
        PlayerHud.Instance.EnableWeaponToggle();

        weapons.Add(newWeapon);

        newWeapon.gameObject.SetActive(false);
        newWeapon.transform.SetParent(WeaponParent.transform, false);

        string[] weaponNames = weapons.Select(x => x.name).ToArray();
        PlayerHud.Instance.SetWeaponWheel(weaponNames);
        PlayerHud.Instance.DisableWeaponWheel();
    }
    public void SwapWeapon(int index) {
        if (index == CurrWeaponIndex) { return; }
        if(index < 0) { index = 0; }
        if(index >= weapons.Count) { index = weapons.Count - 1; }

        Weapon oldWeapon = GetCurrentWeapon();
        oldWeapon.gameObject.SetActive(false);
        oldWeapon.transform.SetParent(WeaponParent.transform, false);

        CurrWeaponIndex = index;

        Weapon newWeapon = GetCurrentWeapon();
        newWeapon.gameObject.SetActive(true);
        newWeapon.transform.SetParent(camera.transform, false);

        int nextIndex = CurrWeaponIndex + 1 >= weapons.Count ? 0 : CurrWeaponIndex + 1;
        int prevIndex = CurrWeaponIndex - 1 < 0 ? weapons.Count - 1 : CurrWeaponIndex - 1;
        PlayerHud.Instance.SetWeaponToggle(weapons[prevIndex].name, newWeapon.name, weapons[nextIndex].name);
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
        GUI.Label(new Rect(10, 70, 150, 20), "Wea Rot: " + weaponWheelRotation);
    }

}