using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : PoolObject {

    [Header("Life")]
    public float Impulse = 10;
    public float LifeTime = 20f;
    public float Acceleration = 10.0f;

    [Header("Damage")]
    public float Damage = 0f;
    public DamageType Type;
    public float Knockback;
    public float KnockbackDuration = 1.0f;
    public float RigidbodyKnockback;
    public float ExplosionRadius;

    [Header("Components")]
    public new Rigidbody rigidbody;
    public new Collider collider;
    public GameObject ExplosionEffect;

    private float startLife;
    private int layerMask;

    // Start is called before the first frame update
    protected override void Start() {
        if (collider == null) { collider = GetComponentInChildren<Collider>(true); }

        if (rigidbody == null) { rigidbody = GetComponentInChildren<Rigidbody>(true); }

        collider.enabled = false;
        rigidbody.isKinematic = true;
        rigidbody.interpolation = RigidbodyInterpolation.None;
        layerMask = PhysicsCollisionMatrix.Instance.MaskForLayer(this.gameObject.layer);
        //rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    private void FixedUpdate() {
        if (!rigidbody.isKinematic && collider.isTrigger) {
            Vector3 start = this.transform.position;
            Vector3 end = start + rigidbody.velocity * Time.fixedDeltaTime;
            RaycastHit hit;
            if (Physics.Linecast(start, end, out hit, layerMask)) {
                this.transform.position = hit.point;
                OnTriggerEnter(hit.collider);
            }

            rigidbody.AddForce(this.transform.forward * Acceleration * Time.deltaTime, ForceMode.Acceleration);
        }
    }

    public void Fire() {
        collider.enabled = true;
        rigidbody.isKinematic = false;
        rigidbody.interpolation = RigidbodyInterpolation.Interpolate;

        rigidbody.AddForce(this.transform.forward * Impulse, ForceMode.Impulse);

        StartCoroutine(Die());

        this.transform.parent = null;
        LevelManager.Instance.MoveToScene(this.gameObject);
    }

    private IEnumerator Die() {
        startLife = Time.time;

        yield return new WaitForSeconds(LifeTime);

        Destroy(this.gameObject);
    }

    private void Explosion() {
        ExplosionEffect.gameObject.SetActive(true);
        ExplosionEffect.transform.parent = null;

        AudioManager.Instance.PlaySoundAtLocation("cannon", ESoundChannel.SFX, this.transform.position);

        Collider[] colliders = Physics.OverlapSphere(this.transform.position, ExplosionRadius, layerMask);
        foreach (Collider c in colliders) {
            Enemy enemy = c.GetComponentInChildren<Enemy>();
            if (enemy == null) { enemy = c.GetComponentInParent<Enemy>(); }
            if (enemy != null) {

                // Damage
                float damage = enemy.health.TakeDamage(this.Type, this.Damage);
                bool isDead = enemy.health.IsDead();

                // Knockback
                if (damage > 0) {
                    if (isDead) {
                        Vector3 dir = c.transform.position - this.transform.position;
                        enemy.Explode(dir * RigidbodyKnockback, this.transform.position);
                    } else {
                        Vector3 dir = c.transform.position - this.transform.position;
                        dir.y = 0.0f;
                        dir = dir.normalized;
                        enemy.Knockback(dir * Knockback, KnockbackDuration);
                    }
                }
            } else {
                Rigidbody rb = c.GetComponentInChildren<Rigidbody>();
                if (rb == null) { rb = c.GetComponentInParent<Rigidbody>(); }

                if (rb != null) {
                    Vector3 dir = c.transform.position - this.transform.position;
                    dir = dir.normalized;
                    rb.AddForceAtPosition(dir * RigidbodyKnockback, this.transform.position, ForceMode.Impulse);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        Explosion();

        collider.enabled = false;

        Destroy(this.gameObject);
    }
}
