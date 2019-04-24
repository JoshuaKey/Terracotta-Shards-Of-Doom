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

    //private void FixedUpdate() {
        //if (!rigidbody.isKinematic) {
        //    Vector3 start = this.transform.position;
        //    Vector3 end = start + rigidbody.velocity;
        //    int layermask = PhysicsCollisionMatrix.Instance.MaskForLayer(this.gameObject.layer);
        //    RaycastHit hit;
        //    if(Physics.Linecast(start, end, out hit, layermask)) {
        //        OnTriggerEnter(hit.collider);
        //    }
        //}
    //}

    public void Fire() {
        collider.enabled = true;
        rigidbody.isKinematic = false;
        startLife = Time.time;

        rigidbody.AddForce(this.transform.forward * Impulse, ForceMode.Impulse);

        this.transform.parent = null;
        LevelManager.Instance.MoveToScene(this.gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        Enemy enemy = other.GetComponentInChildren<Enemy>();
        if (enemy == null) { enemy = other.GetComponentInParent<Enemy>(); }
        if (enemy != null) {
            enemy.health.TakeDamage(this.Type, this.Damage);
            //OnEnemyHit?.Invoke(enemy);
        }

        rigidbody.velocity = Vector3.zero;
        collider.isTrigger = false;
    }
}
