using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSword : AdvancedWeapon {

    [Header("Positioning")]
    public Vector3 StartPos = new Vector3(0.5f, -0.25f, 0.777f);
    public Vector3 StartRot = new Vector3(0, 45, -10);

    [Header("Aim")]
    public float MinDistance = 5f;
    public float MaxDistance = 100f;
    public LayerMask AimLayer;

    [Header("Projectile")]
    public FlameProjectile FlameProjectilePrefab;
    public Transform ProjectilePosition;
    public float ProjectileImpulse = 3.0f;
    public float ProjectileDamage = 1.0f;
    public float ProjectileReloadTime = 1.0f;

    protected new Rigidbody rigidbody;
    protected new Collider collider;
    protected Animator animator;

    private List<GameObject> enemiesHit = new List<GameObject>();
    private FlameProjectile currFlame = null;

    private void Awake() {
        CanCharge = false;
        Type = DamageType.BASIC;// Changes based off Projectile
        OldWeaponName = "Sword";

        this.name = "Fire Sword";
    }

    protected void Start() {
        if (collider == null) { collider = GetComponentInChildren<Collider>(true); }
        if (rigidbody == null) { rigidbody = GetComponentInChildren<Rigidbody>(true); }
        if (animator == null) { animator = GetComponentInChildren<Animator>(true); }

        collider.enabled = false;

        Player.Instance.health.OnDeath += OnDeath;
        animator.keepAnimatorControllerStateOnDisable = false;
    }

    private void OnDeath() {
        if (animator != null) {
            animator.Play("Sword_Still");
        }
    }

    private void OnEnable() {
        this.transform.localPosition = StartPos;
        this.transform.localRotation = Quaternion.Euler(StartRot);

        if(currFlame == null) {
            currFlame = Instantiate(FlameProjectilePrefab, this.transform);          
        }
        Type |= DamageType.FIRE;

        if (Player.Instance && Player.Instance.GetCurrentWeapon() == this) {
            PlayerHud.Instance.EnableCrosshair();
        }
    }
    private void OnDisable() {
        StopAllCoroutines();
        if (collider) {
            collider.enabled = false;
        }

        if (Player.Instance && Player.Instance.GetCurrentWeapon() == this) {
            PlayerHud.Instance.DisableCrosshair();
        }
    }

    public override void Attack() {
        if (!CanAttack()) { return; }

        base.Attack();
        animator.SetTrigger("Swing");
    }

    public void StartSwing() {
        collider.enabled = true;
        AudioManager.Instance.PlaySoundWithParent("swoosh", ESoundChannel.SFX, gameObject);

        if (currFlame != null) {
            Player player = Player.Instance;
            Camera camera = player.camera;
            Vector3 forward = camera.transform.forward;

            Ray ray = new Ray(camera.transform.position + forward * MinDistance, forward);
            RaycastHit hit;
            Vector3 aimPoint = Vector3.zero;
            if (Physics.Raycast(ray, out hit, MaxDistance, AimLayer)) {
                aimPoint = hit.point;
            } else {
                aimPoint = camera.transform.position + forward * MaxDistance;
            }
            currFlame.transform.forward = aimPoint - currFlame.transform.position;

            currFlame.Impulse = ProjectileImpulse;
            currFlame.LifeTime = 20f;
            currFlame.Damage = ProjectileDamage;
            currFlame.Knockback = this.Knockback;
            currFlame.RigidbodyKnockback = this.RigidbodyKnockback;
            currFlame.Fire();
            currFlame = null;
            Type = ~DamageType.FIRE & Type;

            StartCoroutine(Reload());
        }
    }

    public void EndSwing() {
        collider.enabled = false;
        enemiesHit.Clear();
    }

    private IEnumerator Reload() {
        yield return new WaitForSeconds(ProjectileReloadTime);

        currFlame = Instantiate(FlameProjectilePrefab, this.transform);
        Type |= DamageType.FIRE;
    }

    protected void OnTriggerEnter(Collider other) {
        TargetProjectile targetProj = other.GetComponentInChildren<TargetProjectile>();
        if (targetProj != null) {
            Vector3 dir = targetProj.transform.position - Player.Instance.transform.position;
            dir.y = 0.0f;
            dir = dir.normalized;
            targetProj.Hit(this.gameObject, dir * Knockback);
            return;
        }

        if (!enemiesHit.Contains(other.gameObject)) {
            Enemy enemy = other.GetComponentInChildren<Enemy>();
            if (enemy == null) { enemy = other.GetComponentInParent<Enemy>(); }
            if (enemy != null) {
                enemiesHit.Add(other.gameObject);

                float damage = enemy.health.TakeDamage(this.Type, this.Damage);
                bool isDead = enemy.health.IsDead();
                if (damage > 0) {
                    if (isDead) {
                        Vector3 forward = Player.Instance.camera.transform.forward;
                        //forward = forward.normalized;
                        enemy.Explode(forward * RigidbodyKnockback, Player.Instance.camera.transform.position);
                    } else {
                        Vector3 forward = Player.Instance.camera.transform.forward;
                        forward.y = 0.0f;
                        forward = forward.normalized;
                        enemy.Knockback(forward * Knockback, KnockbackDuration);
                    }
                }
            } else {
                Rigidbody rb = other.GetComponentInChildren<Rigidbody>();
                if (rb == null) { rb = other.GetComponentInParent<Rigidbody>(); }
                if (rb != null) {
                    Vector3 forward = Player.Instance.camera.transform.forward;
                    //forward = forward.normalized;
                    rb.AddForce(forward * RigidbodyKnockback, ForceMode.Impulse);
                }
            }
        }
    }
}
