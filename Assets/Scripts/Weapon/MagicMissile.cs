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

    [Header("Target")]
    public GameObject Target;
    public float PeakHeight = 1;
    public float SpeedOverTime = .2f;

    [Header("Knockback")]
    public float Knockback;
    public float RigidbodyKnockback;

    protected new Rigidbody rigidbody;
    protected new Collider collider;

    private float startLife;

    private Vector3 startPos;

    // Start is called before the first frame update
    protected override void Start() {
        if (collider == null) { collider = GetComponentInChildren<Collider>(true); }

        if (rigidbody == null) { rigidbody = GetComponentInChildren<Rigidbody>(true); }

        collider.enabled = false;
        rigidbody.isKinematic = true;
    }

    public void Fire() {
        collider.enabled = true;
        rigidbody.isKinematic = false;
        rigidbody.interpolation = RigidbodyInterpolation.Interpolate;

        startPos = this.transform.position;

        if (Target == null) {
            FallDown();
        } else {
            StartCoroutine(FireAnimation());
        }

        StartCoroutine(Die());

        this.transform.parent = null;
        LevelManager.Instance.MoveToScene(this.gameObject);
    }

    private IEnumerator FireAnimation() {
        if (Target == null) {
            yield break;
        }

        Vector3 peak = Utility.CreatePeak(startPos, GetTargetPosition(), PeakHeight);

        float startTime = Time.time;
        float length = (GetTargetPosition() - startPos).magnitude * SpeedOverTime;
        while (Time.time < startTime + length) {
            float t = (Time.time - startTime) / length;

            if (Target != null) {
                Vector3 pos = Interpolation.BezierCurve(startPos, peak, GetTargetPosition(), t);
                this.transform.position = pos;
            } else {
                FallDown();
                yield break;
            }  

            yield return null;
        }

        FallDown();

        if (Target != null) {
            this.transform.position = GetTargetPosition();
        }
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

    public Vector3 GetTargetPosition() {
        //return Target.transform.position + Vector3.up;
        return Target.transform.position;
    }
}
