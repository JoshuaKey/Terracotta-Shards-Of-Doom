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
    public float HorizontalRotationSensitivity = 1.0f;
    public float VerticalRotationSensitivity = 1.0f;
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
    public bool CanInteract = true;
    
    [Header("Compass")]
    public CompassPot compass;
    public Transform Shoulder;

    [Header("IsGrounded")]
    public float MinJumpAngle = 30f;

    [Header("Death")]
    public float CameraDeathRotation = 30;
    public float CameraDeathDistance = 3.0f;
    public float CameraDeathAnimationSpeed = 1.0f;
    public float MinDeathTimeScale = .01f;
    public BrokenPot PlayerBrokenPotPrefab;
    public BrokenPot brokenPot;

    [Header("Player Stats")]
    private int coins;
    public int Coins
    {
        get
        {
            return coins;
        }
            set
        {
            coins = Mathf.Min(value, 420420);
        }
    }

    public static Player Instance;

    [HideInInspector]
    public Vector3 velocity = Vector3.zero;
    [HideInInspector]
    public Vector3 rotation = Vector3.zero;
    [HideInInspector]
    public List<Weapon> weapons = new List<Weapon>();
    [HideInInspector]
    public int layerMask;
    [HideInInspector]
    private bool wasGrounded = false;
    private bool isGrounded = false;

    private new Collider collider;
    private CharacterController controller;
    private Vector2 weaponWheelRotation = Vector2.zero;

    private Vector3 cameraPosition;

    private void Awake() {
        if (Instance != null) { Destroy(this.gameObject); return; }
        Instance = this;      
    }

    void Start() {
        if (collider == null) { collider = GetComponentInChildren<Collider>(true); }

        if (controller == null) { controller = GetComponentInChildren<CharacterController>(true); }

        if (health == null) { health = GetComponentInChildren<Health>(true); }

        if (camera == null) { camera = GetComponentInChildren<Camera>(true); }

        if (brokenPot == null) { brokenPot = GetComponentInChildren<BrokenPot>(true); }
        if (brokenPot == null) { brokenPot = GameObject.Instantiate(PlayerBrokenPotPrefab, this.transform); }

        //Weapons
        Weapon[] weaponArray = GetComponentsInChildren<Weapon>(true);
        CurrWeaponIndex = Mathf.Min(weaponArray.Length - 1, CurrWeaponIndex);
        foreach (Weapon w in weaponArray) {
            AddWeapon(w);
        }

        // Physics
        layerMask = 1 << this.gameObject.layer;

        // Camera
        Cursor.lockState = CursorLockMode.Locked;
        rotation = this.transform.rotation.eulerAngles;
        cameraPosition = camera.transform.localPosition;

        // Compass
        compass.Origin = Shoulder;
        compass.Base = camera.transform;
        compass.transform.SetParent(Shoulder);

        // Input Scheme Rotation
        CheckInputScheme();

        PlayerHud.Instance.EnablePlayerHealthBar();
        this.health.OnHeal += this.ChangeHealthUI;
        this.health.OnDamage += this.ChangeHealthUI;
        this.health.OnDamage += this.OnDamage;
        this.health.OnDeath += this.Die;

        Settings.OnLoad += OnSettingsLoad;
        PlayerStats.OnLoad += OnStatsLoad;
        InputManager.ControlSchemesChanged += OnControlSchemeChanged;
        InputManager.PlayerControlsChanged += OnPlayerControlChanged;
    }

    void Update() {
        if (CanMove) {
            UpdateMovement();
        }

        wasGrounded = isGrounded;
        isGrounded = false;

        UpdateCombat();
        if (CanInteract) {
            UpdateInteractable();
        }
        if (InputManager.GetButtonDown("Pause Menu"))
        {
            //if (Time.timeScale == 0)
            //{
            //    PauseMenu.Instance.DeactivatePauseMenu();
            //}
            //else
            {
                PauseMenu.Instance.ActivatePauseMenu();
            }
        }


        // Debug...
        if (Input.GetKeyDown(KeyCode.Alpha0)) {
            string levelName = LevelManager.Instance.GetLevelName();
            LevelManager.Instance.Levels[levelName].IsCompleted = true;
            LevelManager.Instance.LoadScene("Hub");
        }
        if (Application.isEditor) {
            if (Input.GetKeyDown(KeyCode.T)) {
                this.health.TakeDamage(DamageType.TRUE, 0.5f);
            }
            if (Input.GetKeyDown(KeyCode.H)) {
                this.health.Heal(3.0f);
            }
            if (Input.GetKeyDown(KeyCode.Equals)) {
                LevelManager.Instance.LoadScene("Combat Scene");
            }
            if (Input.GetKeyDown(KeyCode.Z)) {
                Weapon w = WeaponManager.Instance.GetWeapon("Bow");
                w.transform.SetParent(WeaponParent.transform, false);
                AddWeapon(w);
            }
            if (Input.GetKeyDown(KeyCode.X)) {
                Weapon w = WeaponManager.Instance.GetWeapon("Hammer");
                w.transform.SetParent(WeaponParent.transform, false);
                AddWeapon(w);
            }
            if (Input.GetKeyDown(KeyCode.C)) {
                Weapon w = WeaponManager.Instance.GetWeapon("Spear");
                w.transform.SetParent(WeaponParent.transform, false);
                AddWeapon(w);
            }
            if (Input.GetKeyDown(KeyCode.V)) {
                Weapon w = WeaponManager.Instance.GetWeapon("Crossbow");
                w.transform.SetParent(WeaponParent.transform, false);
                AddWeapon(w);
            }
            if (Input.GetKeyDown(KeyCode.B)) {
                Weapon w = WeaponManager.Instance.GetWeapon("Magic");
                w.transform.SetParent(WeaponParent.transform, false);
                AddWeapon(w);
            }
            // Advanced
            if (Input.GetKeyDown(KeyCode.N)) {
                Weapon w = WeaponManager.Instance.GetWeapon("Fire Sword");
                w.transform.SetParent(WeaponParent.transform, false);
                AddWeapon(w);
            }
            if (Input.GetKeyDown(KeyCode.M)) {
                Weapon w = WeaponManager.Instance.GetWeapon("Ice Bow");
                w.transform.SetParent(WeaponParent.transform, false);
                AddWeapon(w);
            }
            if (Input.GetKeyDown(KeyCode.Comma)) {
                Weapon w = WeaponManager.Instance.GetWeapon("Rock Hammer");
                w.transform.SetParent(WeaponParent.transform, false);
                AddWeapon(w);
            }
            if (Input.GetKeyDown(KeyCode.Period)) {
                Weapon w = WeaponManager.Instance.GetWeapon("Lightning Spear");
                w.transform.SetParent(WeaponParent.transform, false);
                AddWeapon(w);
            }
            if (Input.GetKeyDown(KeyCode.Slash)) {
                Weapon w = WeaponManager.Instance.GetWeapon("Magic Missile");
                w.transform.SetParent(WeaponParent.transform, false);
                AddWeapon(w);
            }
            if (Input.GetKeyDown(KeyCode.Quote)) {
                Weapon w = WeaponManager.Instance.GetWeapon("Magic Magic");
                w.transform.SetParent(WeaponParent.transform, false);
                AddWeapon(w);
            }
            // Money
            if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown(KeyCode.Alpha4))
            {
                Coins += 2000;
                Debug.Log("Motherlode");
                PlayerHud.Instance.SetCoinCount(Coins);
                PlayerHud.Instance.PlayCoinAnimation();
            }
        }

    }
    private void LateUpdate() {
        if (CanRotate) {
            UpdateCamera();
        }
    }

    public void OnDamage(float damage)
    {
        AudioManager.Instance.PlaySoundWithParent("player_hit", ESoundChannel.SFX, gameObject);
    }
    public void UpdateCamera() {
        // Rotation Input
        float xRot = InputManager.GetAxis("Vertical Rotation");
        xRot *= YRotationSpeed * HorizontalRotationSensitivity * Time.deltaTime;
        float yRot = InputManager.GetAxis("Horizontal Rotation");
        yRot *= XRotationSpeed * VerticalRotationSensitivity * Time.deltaTime;

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
        if (CanSwapWeapon && weapon.CanSwap()) { 
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
        //// Check for Compass
        //{
        //    if (InputManager.GetButtonDown("Compass")) {
        //        compass.Activate();
        //    } 
        //    else if (compass.gameObject.activeInHierarchy) {
        //        Transform target;
        //        if (EnemyManager.Instance.MainProgression.IsComplete()) {
        //            target = EnemyManager.Instance.MainProgression.ProgressionObject.transform;
        //        } else {
        //            target = EnemyManager.Instance.GetClosestEnemy(this.transform.position).transform;
        //        }
        //        compass.Target = target;
        //    }
        //}


        // Check for interactable
        {
            Ray ray = new Ray(camera.transform.position, camera.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, InteractDistance, InteractLayer)) {
                Interactable interactable = hit.collider.GetComponentInChildren<Interactable>(true);
                if (interactable == null) { interactable = hit.collider.GetComponentInParent<Interactable>(); }

                if (interactable.CanInteract) {
                    PlayerHud.Instance.EnableInteractText();

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
        if (wasGrounded) {
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
    public void Knockback(Vector3 force) {
        velocity += force;
    }

    public void ChangeHealthUI(float val) {
        PlayerHud.Instance.SetPlayerHealthBar(this.health.CurrentHealth / this.health.MaxHealth);
    }
    public void Respawn() {
        StopAllCoroutines();

        // Enable Player
        this.enabled = true;
        collider.enabled = true;
        Time.timeScale = 1.0f;

        // Reattach Camera
        this.camera.transform.parent = this.transform;
        camera.transform.localPosition = cameraPosition;

        // Show Weapon
        Weapon newWeapon = GetCurrentWeapon();
        newWeapon.gameObject.SetActive(true);
        newWeapon.transform.SetParent(camera.transform, false);

        // Reset Broken Pot
        brokenPot = GameObject.Instantiate(PlayerBrokenPotPrefab, this.transform);         

        // UI
        DeathScreen.Instance.DisableDeathScreen();
        PlayerHud.Instance.EnablePlayerHud();
        PlayerHud.Instance.SetPlayerHealthBar(1.0f, true);

        // Respawn...
        Game.Instance.LoadPlayerStats();
    }
    public void Die() {
        StartCoroutine(CameraDeathAnimation());      
    }
    private IEnumerator CameraDeathAnimation() {
        // Play Sound
        string[] sounds = {
            "ceramic_shatter1",
            "ceramic_shatter2",
            "ceramic_shatter3",
        };
        AudioManager.Instance.PlaySoundAtLocation(sounds[Random.Range(0, sounds.Length)], ESoundChannel.SFX, transform.position);

        // Disable Player
        this.enabled = false;
        collider.enabled = false;

        // Enable Broken Pot
        brokenPot.transform.parent = null;
        brokenPot.gameObject.SetActive(true);
        LevelManager.Instance.MoveToScene(brokenPot.gameObject);
        brokenPot.Explode(3.0f, this.camera.transform.position);

        // Explosion Effect?
        int layermask = PhysicsCollisionMatrix.Instance.MaskForLayer(this.gameObject.layer);
        Collider[] colliders = Physics.OverlapSphere(Player.Instance.transform.position, 2.0f, layermask);
        foreach (Collider c in colliders) {
            Enemy enemy = c.GetComponentInChildren<Enemy>();
            if (enemy == null) { enemy = c.GetComponentInParent<Enemy>(); }
            if (enemy != null) {
                Vector3 dir = c.transform.position - Player.Instance.transform.position;
                dir = dir.normalized;
                enemy.Knockback(dir * 5, 1.0f);
            }
        }

        // Deparent camera
        this.camera.transform.parent = null;

        // Hide Weapon
        Weapon currWeapon = GetCurrentWeapon();
        currWeapon.gameObject.SetActive(false);
        currWeapon.transform.SetParent(WeaponParent.transform, false);

        PlayerHud.Instance.DisablePlayerHud();
        DeathScreen.Instance.EnableDeathScreen();

        // Determine Camera Destination
        float length = 1.0f;
        float startTime = Time.time;
        Vector3 startPos = -this.transform.forward + camera.transform.position;

        Vector3 endPos = -this.transform.forward * CameraDeathDistance;
        endPos = Quaternion.Euler(CameraDeathRotation, 0.0f, 0.0f) * endPos;
        endPos += camera.transform.position;

        while (Time.time < startTime + length) {
            float t = (Time.time - startTime) / length;

            Time.timeScale = Mathf.Max(1 - t, MinDeathTimeScale);

            float alpha = Mathf.Lerp(0, 128, t);
            DeathScreen.Instance.SetBackgroundAlpha(alpha);

            t = Interpolation.CubicOut(t);
            Vector3 pos = Vector3.Lerp(startPos, endPos, t);
            this.camera.transform.position = pos;
            this.camera.transform.LookAt(this.transform);

            if (InputManager.GetButtonDown("UI_Submit")) {
                Respawn();
            }

            yield return null;
        }

        DeathScreen.Instance.SetBackgroundAlpha(128);
        Time.timeScale = 0.0f;

        // Check for restart
        while (true) {
            if (InputManager.GetButtonDown("UI_Submit")) {
                Respawn();
            }

            yield return null;
        }
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
        if(newWeapon is AdvancedWeapon) {
            AdvancedWeapon w = (AdvancedWeapon)newWeapon;
            int index = weapons.FindIndex(x => x.name == w.OldWeaponName);

            Weapon oldWeapon = weapons[index];
            Destroy(oldWeapon.gameObject);

            weapons[index] = newWeapon;

            oldWeapon.gameObject.SetActive(false);
        } else {
            weapons.Add(newWeapon);
        }  

        newWeapon.gameObject.SetActive(false);
        newWeapon.transform.SetParent(WeaponParent.transform, false);

        // This works if we are adding MagicMagic, but holding Magic, MagicMagic will be enabled
        Weapon currWeapon = GetCurrentWeapon();
        currWeapon.gameObject.SetActive(true);
        currWeapon.transform.SetParent(camera.transform, false);

        string[] weaponNames = weapons.Select(x => x.name).ToArray();
        int nextIndex = CurrWeaponIndex + 1 >= weapons.Count ? 0 : CurrWeaponIndex + 1;
        int prevIndex = CurrWeaponIndex - 1 < 0 ? weapons.Count - 1 : CurrWeaponIndex - 1;

        PlayerHud.Instance.SetWeaponToggle(weapons[prevIndex].name, 
            weapons[CurrWeaponIndex].name, weapons[nextIndex].name);
        PlayerHud.Instance.SetWeaponWheel(weaponNames);
        PlayerHud.Instance.DisableWeaponWheel();

        if (weapons.Count > 1) {
            PlayerHud.Instance.EnableWeaponToggle();
            Player.Instance.CanSwapWeapon = true;
        } else {
            PlayerHud.Instance.DisableWeaponToggle();
            Player.Instance.CanSwapWeapon = false;
        }
    }
    public void SwapWeapon(int index) {
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
    public void HideWeapon() {
        Weapon oldWeapon = GetCurrentWeapon();
        oldWeapon.gameObject.SetActive(false);
        oldWeapon.transform.SetParent(WeaponParent.transform, false);
    }
    public void ShowWeapon() {
        Weapon newWeapon = GetCurrentWeapon();
        newWeapon.gameObject.SetActive(true);
        newWeapon.transform.SetParent(camera.transform, false);
    }
    public Weapon GetCurrentWeapon() {
        return CurrWeaponIndex >= weapons.Count ? null : weapons[CurrWeaponIndex];
    }

    private void OnPlayerControlChanged(PlayerID id) { CheckInputScheme(); }
    private void OnControlSchemeChanged() { CheckInputScheme(); }
    public void CheckInputScheme() {
        if(InputManager.PlayerOneControlScheme.Name == InputController.Instance.ControllerSchemeName) {
            XRotationSpeed = 150f;
            YRotationSpeed = 100f;
        } else {
            XRotationSpeed = 60f;
            YRotationSpeed = 60f;
        }
    }
    private void OnSettingsLoad(Settings settings) {
        Player.Instance.camera.fieldOfView = settings.FOV;
        Player.Instance.VerticalRotationSensitivity = settings.VerticalSensitivity;
        Player.Instance.HorizontalRotationSensitivity = settings.HorizontalSensitivity;
    }
    private void OnStatsLoad(PlayerStats stats) {
        this.Coins = stats.Coins;
        PlayerHud.Instance.SetCoinCount(this.Coins);

        foreach (Weapon w in weapons) { Destroy(w.gameObject); }
        weapons.Clear();
        CurrWeaponIndex = 0;

        foreach(string weaponName in stats.Weapons) {
            Weapon w = WeaponManager.Instance.GetWeapon(weaponName);
            AddWeapon(w);
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit) {
        // If we are touching the "ground"
        if ((hit.controller.collisionFlags & CollisionFlags.Below) != 0) {
            velocity.y = Mathf.Max(velocity.y, -1.0f);
            float angle = Vector3.Angle(Vector3.down, -hit.normal);

            // Can we jump based off the collision normal...
            if (angle < MinJumpAngle) {
                isGrounded = true;
                return;
            }
        }
    }

    // Testing --------------------------------------------------------------
    private void OnGUI() {
        if (Application.isEditor) {
            GUI.Label(new Rect(10, 10, 150, 20), "Vel: " + velocity);
            GUI.Label(new Rect(10, 30, 150, 20), "Rot: " + rotation);

            GUI.Label(new Rect(10, 50, 150, 20), "Inp: " + new Vector2(InputManager.GetAxisRaw("Vertical Movement"), InputManager.GetAxisRaw("Horizontal Movement")));
            GUI.Label(new Rect(10, 70, 150, 20), "Wea Rot: " + weaponWheelRotation);
            GUI.Label(new Rect(10, 90, 150, 20), "Grounded: " + controller.isGrounded + " " + wasGrounded);
        }
    }

}