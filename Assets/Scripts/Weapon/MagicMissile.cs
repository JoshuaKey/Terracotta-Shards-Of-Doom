using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMissile : PoolObject {

    [Header("Life")]
    public Vector3 Impulse;
    public float TargetSpeed = 5;
    public float TargetLerpSpeed = 0.9f;
    public float LifeTime = 20f;

    [Header("Damage")]
    public float Damage = 0f;
    public DamageType Type;
    public GameObject Target;
    public float Knockback;
    public float RigidbodyKnockback;

    protected new Rigidbody rigidbody;
    protected new Collider collider;

    private float startLife;

    // Start is called before the first frame update
    void Start() {
        if (collider == null) { collider = GetComponentInChildren<Collider>(true); }

        if (rigidbody == null) { rigidbody = GetComponentInChildren<Rigidbody>(true); }

        collider.enabled = false;
        rigidbody.isKinematic = true;
    }

    private void FixedUpdate() {
        if (!rigidbody.isKinematic && Target != null) {
            Vector3 dir = (Target.transform.position - this.transform.position);
            dir = dir.normalized * TargetSpeed;
            rigidbody.AddForce(dir, ForceMode.Impulse);
        }
    }

    public void Fire() {
        collider.enabled = true;
        rigidbody.isKinematic = false;
        
        rigidbody.AddForce(Impulse, ForceMode.Impulse);
        if(Impulse == Vector3.zero) {
            rigidbody.useGravity = true;
        }

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
            }
        } 

        rigidbody.velocity = Vector3.zero;
        collider.isTrigger = false;
        rigidbody.useGravity = true;
        Target = null;

        int layer = LayerMask.NameToLayer("Default");
        this.gameObject.layer = layer;
        for(int i = 0; i < this.transform.childCount; i++) {
            Transform t = this.transform.GetChild(i);
            t.gameObject.layer = layer;
        }
    }
}
