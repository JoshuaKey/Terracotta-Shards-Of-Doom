using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameProjectile : PoolObject {
    [Header("Life")]
    public float Impulse = 10;
    public float LifeTime = 20f;
    public float Gravity;

    [Header("Damage")]
    public float Damage = 0f;
    public DamageType Type;
    public float Knockback;
    public float RigidbodyKnockback;

    [Header("Components")]
    public new Rigidbody rigidbody;
    public new Collider collider;

    private float startLife;
    private int layerMask;

    // Start is called before the first frame update
    protected override void Start() {
        if (collider == null) { collider = GetComponentInChildren<Collider>(true); }

        if (rigidbody == null) { rigidbody = GetComponentInChildren<Rigidbody>(true); }

        //collider.enabled = false;
        //rigidbody.isKinematic = true;
        layerMask = PhysicsCollisionMatrix.Instance.MaskForLayer(this.gameObject.layer);
        //rigidbody.constraints = RigidbodyConstraints.FreezeAll;

        Type = DamageType.FIRE;
    }

    private void FixedUpdate() {
        if (!rigidbody.isKinematic) {
            rigidbody.AddForce(Vector3.up * Gravity, ForceMode.Acceleration);
        }
    }

    public void Fire() {
        collider.enabled = true;
        rigidbody.isKinematic = false;
        rigidbody.interpolation = RigidbodyInterpolation.Interpolate;

        rigidbody.AddForce(this.transform.forward * Impulse, ForceMode.Impulse);

        //Debug.Break();

        StartCoroutine(Die());

        this.transform.parent = null;
        LevelManager.Instance.MoveToScene(this.gameObject);
    }

    private IEnumerator Die() {
        startLife = Time.time;

        yield return new WaitForSeconds(LifeTime);

        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        print("hit" + other.name);
        Enemy enemy = other.GetComponentInChildren<Enemy>();
        if (enemy == null) { enemy = other.GetComponentInParent<Enemy>(); }
        if (enemy != null) {
            // Damage
            float damage = enemy.health.TakeDamage(this.Type, this.Damage);
            bool isDead = enemy.health.IsDead();

            // Explosion
            if (damage > 0 && isDead) {
                Vector3 forward = this.transform.forward;
                forward.y = 0.0f;
                forward = forward.normalized;
                enemy.Explode(forward * RigidbodyKnockback, this.transform.position);
            }
        } else {
            // Physics Impulse
            Rigidbody rb = other.GetComponentInChildren<Rigidbody>();
            if (rb == null) { rb = other.GetComponentInParent<Rigidbody>(); }
            if (rb != null) {
                Vector3 forward = this.transform.forward;
                forward.y = 0.0f;
                forward = forward.normalized;
                rb.AddForce(forward * RigidbodyKnockback, ForceMode.Impulse);
            }
        }

        Destroy(this.gameObject);
    }
}
