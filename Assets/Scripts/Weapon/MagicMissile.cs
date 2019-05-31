using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMissile : PoolObject {

    [Header("Life")]
    public Vector3 Impulse;
    public float TargetSpeed = 5;
    public float MinSpeed = 2.0f;
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
    protected override void Start() {
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

        if (Target != null) {
            Debug.DrawLine(this.transform.position, GetTargetPosition(), Color.blue);
        }
    }

    public void Fire() {
        collider.enabled = true;
        rigidbody.isKinematic = false;
        rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
    
        if (Target == null) {
            FallDown();
        } else {
            rigidbody.AddForce(Impulse, ForceMode.Impulse);
            //StartCoroutine(FireAnimation());
        }

        StartCoroutine(Die());

        this.transform.parent = null;
        LevelManager.Instance.MoveToScene(this.gameObject);
    }

    //private IEnumerator FireAnimation() { 

    //    while (true) {
    //        //Vector3 targetDir = Target.transform.position - transform.position;
    //        //float step = TargetSpeed * Time.deltaTime;
    //        //Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);
    //        //Debug.DrawRay(transform.position, newDir, Color.red);
    //        //transform.rotation = Quaternion.LookRotation(newDir);
    //        if (Target == null) {
    //            yield break;
    //        }
            
    //        Vector3 dir = (Target.transform.position - this.transform.position);
    //        dir = dir.normalized * TargetSpeed;
    //        rigidbody.AddForce(dir, ForceMode.Impulse);

    //        yield return new WaitForSeconds(.02f);

    //        //rigidbody.velocity = Vector3.zero;

    //        //yield return new WaitForSeconds(.33f);

    //        //transform.position = Vector3.MoveTowards(transform.position, Target.transform.position, TargetSpeed * Time.deltaTime);

    //        //Vector3 dir = (Target.transform.position - this.transform.position);
    //        //dir = dir.normalized * TargetSpeed * Time.deltaTime;
    //        //rigidbody.AddForce(dir, ForceMode.Acceleration);
    //        //yield return null;
    //    }
    //}

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

        FallDown();
    }

    private void FallDown() {
        StopAllCoroutines();

        rigidbody.velocity = Vector3.zero;
        collider.isTrigger = false;
        rigidbody.useGravity = true;
        //rigidbody.AddForce(this.transform.position - other.transform.position);
        Target = null;
        int layer = LayerMask.NameToLayer("Default");
        this.gameObject.layer = layer;
        for (int i = 0; i < this.transform.childCount; i++) {
            Transform t = this.transform.GetChild(i);
            t.gameObject.layer = layer;
        }
    }

    Vector3 BallisticVel() {
        Vector3 dir = GetTargetPosition() - transform.position; // get target direction
        float h = dir.y;  // get height difference
        dir.y = 0;  // retain only the horizontal direction
        float dist = dir.magnitude;  // get horizontal distance
        dir.y = dist;  // set elevation to 45 degrees
        dist += h;  // correct for different heights
        float vel = Mathf.Sqrt(dist * Physics.gravity.magnitude);
        return vel * dir.normalized;  // returns Vector3 velocity
    }

    public Vector3 GetTargetPosition() {
        return Target.transform.position + Vector3.up;
    }
}
