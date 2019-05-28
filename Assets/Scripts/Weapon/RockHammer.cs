using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockHammer : AdvancedWeapon {

    [Header("Slam")]
    public Transform SlamCenter;
    public float SlamRadius;
    public ParticleSystem DustEffect;
    public float PlayerJump = 10;

    private Animator animator;

    private void Awake() {
        CanCharge = false;
        Type = DamageType.EARTH;
        OldWeaponName = "Hammer";

        this.name = "Rock Hammer";

        animator = GetComponent<Animator>();
    }

    private void OnDisable() {
        StopAllCoroutines();
    }

    public override void Attack() {
        if (!CanAttack()) { return; }

        base.Attack();

        Player.Instance.CanWalk = false;
        animator.SetTrigger("Attack");
    }

    private void SlamAttack() {
        Player.Instance.CanWalk = true;
        Player.Instance.velocity.y = PlayerJump;

        DustEffect.Play();
        AudioManager.Instance.PlaySoundWithParent("hammer", ESoundChannel.SFX, gameObject);

        int layermask = PhysicsCollisionMatrix.Instance.MaskForLayer(this.gameObject.layer);
        Collider[] colliders = Physics.OverlapSphere(Player.Instance.transform.position, SlamRadius, layermask);
        //Collider[] colliders = Physics.OverlapSphere(SlamCenter.position, SlamRadius, layermask);
        foreach (Collider c in colliders) {
            TargetProjectile targetProj = c.GetComponentInChildren<TargetProjectile>();
            if (targetProj != null) {
                Vector3 dir = targetProj.transform.position - Player.Instance.transform.position;
                dir.y = 0.0f;
                dir = dir.normalized;
                targetProj.Hit(this.gameObject, dir * Knockback);
                continue;
            }

            Enemy enemy = c.GetComponentInChildren<Enemy>();
            if (enemy == null) { enemy = c.GetComponentInParent<Enemy>(); }
            if (enemy != null) {

                // Damage
                float damage = enemy.health.TakeDamage(this.Type, this.Damage);
                bool isDead = enemy.health.IsDead();

                // Knockback
                if (damage > 0) {
                    if (isDead) {
                        //Vector3 dir = c.transform.position - SlamCenter.position;
                        Vector3 dir = c.transform.position - Player.Instance.transform.position;
                        //dir.y = 0.0f;
                        dir = dir.normalized;
                        enemy.Explode(dir * RigidbodyKnockback, SlamCenter.position);
                    } else {
                        //Vector3 dir = c.transform.position - SlamCenter.position;
                        Vector3 dir = c.transform.position - Player.Instance.transform.position;
                        dir.y = 0.0f;
                        dir = dir.normalized;
                        enemy.Knockback(dir * Knockback, KnockbackDuration);
                    }
                }
            } else {
                Rigidbody rb = c.GetComponentInChildren<Rigidbody>();
                if (rb == null) { rb = c.GetComponentInParent<Rigidbody>(); }

                if (rb != null) {
                    //Vector3 dir = c.transform.position - SlamCenter.position;
                    Vector3 dir = c.transform.position - Player.Instance.transform.position;
                    //dir.y = 0.0f;
                    dir = dir.normalized;
                    rb.AddForceAtPosition(dir * RigidbodyKnockback, SlamCenter.position, ForceMode.Impulse);
                }
            }
        }
    }
}
