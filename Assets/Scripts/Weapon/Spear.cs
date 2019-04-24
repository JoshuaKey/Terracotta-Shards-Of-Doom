using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : Weapon {

    [Header("Swing Animation")]
    public Vector3 StartPos = new Vector3(0.4f, -1, 0.77f);
    public Vector3 EndPos = new Vector3(0.4f, -1, 0.77f);
    public float AnimationSwingTime;

    [Header("Recoil Animation")]
    public Vector3 StartRot = Vector3.zero;
    public Vector3 EndRot = new Vector3(120, 0, 0);
    public float AnimationRecoilTime;

    protected new Rigidbody rigidbody;
    protected new Collider collider;

    private List<GameObject> enemiesHit = new List<GameObject>();

    protected void Start() {
        if (collider == null) { collider = GetComponentInChildren<Collider>(true); }
        if (rigidbody == null) { rigidbody = GetComponentInChildren<Rigidbody>(true); }

        collider.enabled = false;

        CanCharge = false;
        Type = DamageType.BASIC;

        this.name = "Spear";
    }

    private void OnDisable() {
        StopAllCoroutines();
        this.transform.localPosition = StartPos;
        this.transform.localRotation = Quaternion.Euler(StartRot);
    }

    public override void Attack() {
        if (!CanAttack()) { return; }

        base.Attack();
        StartCoroutine(Stab());
    }

    private IEnumerator Stab() {
        yield return null;
    }

    protected void OnTriggerEnter(Collider other) {
        if (!enemiesHit.Contains(other.gameObject)) {
            Enemy enemy = other.GetComponentInChildren<Enemy>();
            if (enemy == null) { enemy = other.GetComponentInParent<Enemy>(); }
            if (enemy != null) {
                enemiesHit.Add(other.gameObject);

                float damage = enemy.health.TakeDamage(this.Type, this.Damage);
                bool isDead = enemy.health.IsDead();
                if (damage > 0) {
                    if (isDead) {
                        //print("Explode");
                    } else {
                        Vector3 forward = Player.Instance.camera.transform.forward;
                        forward.y = 0.0f;
                        forward = forward.normalized;
                        enemy.Knockback(forward * Knockback);
                    }
                }
            }
        }
    }
}
