using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : Weapon {

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
        //this.transform.localPosition = new Vector3(0.5f, -0.25f, 0.777f);
        //this.transform.localRotation = Quaternion.Euler(new Vector3(0, 45, -10));
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
            Enemy enemy = other.GetComponent<Enemy>();
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

                //OnEnemyHit?.Invoke(enemy);
            }
        }
    }
}
