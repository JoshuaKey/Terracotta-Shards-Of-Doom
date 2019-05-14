using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : PoolObject {

    [Header("Life")]
    public float Impulse = 10;
    public float LifeTime = 20f;

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
    void Start() {
        if (collider == null) { collider = GetComponentInChildren<Collider>(true); }

        if (rigidbody == null) { rigidbody = GetComponentInChildren<Rigidbody>(true); }

        collider.enabled = false;
        rigidbody.isKinematic = true;
        layerMask = PhysicsCollisionMatrix.Instance.MaskForLayer(this.gameObject.layer);
        //rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    private void FixedUpdate() {
        if (!rigidbody.isKinematic && collider.isTrigger) {
            Vector3 start = this.transform.position;
            Vector3 end = start + rigidbody.velocity;
            RaycastHit hit;
            if (Physics.Linecast(start, end, out hit, layerMask)) {
                this.transform.position = hit.point;
                OnTriggerEnter(hit.collider);
            }
        }
    }

    public void Fire() {
        collider.enabled = true;
        rigidbody.isKinematic = false;

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

    private void OnTriggerEnter(Collider other) {
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
            } else {
                AudioManager.Instance.PlaySound("ceramic_tink", ESoundChannel.SFX, gameObject);
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

        rigidbody.velocity = Vector3.zero;
        collider.isTrigger = false;

        int layer = LayerMask.NameToLayer("Default");
        this.gameObject.layer = layer;
        for (int i = 0; i < this.transform.childCount; i++) {
            Transform t = this.transform.GetChild(i);
            t.gameObject.layer = layer;
        }
    }
}
