using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMissile : PoolObject {

    [Header("Life")]
    public Vector3 Impulse;
    public float LifeTime = 20f;

    [Header("Damage")]
    public float Damage = 0f;
    public DamageType Type;
    public GameObject Target;

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
            Vector3 dir = Target.transform.position - this.transform.position;
            Vector3 vel = Vector3.LerpUnclamped(rigidbody.velocity, dir, Time.deltaTime);
            rigidbody.velocity = vel;
        }
    }

    public void Fire() {
        collider.enabled = true;
        rigidbody.isKinematic = false;
        
        rigidbody.AddForce(Impulse, ForceMode.Impulse);

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
            enemy.health.TakeDamage(this.Type, this.Damage);
        }

        rigidbody.velocity = Vector3.zero;
        collider.isTrigger = false;
    }
}
