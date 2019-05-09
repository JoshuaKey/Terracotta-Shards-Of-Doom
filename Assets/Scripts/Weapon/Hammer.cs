using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : Weapon {

    [Header("Slam")]
    public Transform SlamCenter;
    public float SlamRadius;
    public ParticleSystem DustEffect;

    [Header("Swing Animation")]
    public Vector3 StartPos = new Vector3(0.4f, -1, 0.77f);
    public Vector3 EndPos = new Vector3(0.4f, -1, 0.77f);
    public float AnimationSwingTime;
    public float PlayerJump = 10;

    [Header("Recoil Animation")]
    public Vector3 StartRot = Vector3.zero;
    public Vector3 EndRot = new Vector3(120, 0, 0);
    public float AnimationRecoilTime;

    void Start() {
        CanCharge = false;
        Type = DamageType.BASIC;

        this.name = "Hammer";
    }

    private void OnEnable() {
        this.transform.localPosition = StartPos;
        this.transform.localRotation = Quaternion.Euler(StartRot);
    }
    private void OnDisable() {
        StopAllCoroutines();
    }

    public override void Attack() {
        if (!CanAttack()) { return; }

        base.Attack();
        StartCoroutine(Slam());
    }

    private IEnumerator Slam() {
        Player.Instance.CanWalk = false;
        Quaternion StartRotQuat = Quaternion.Euler(StartRot);
        Quaternion EndRotQuat = Quaternion.Euler(EndRot);

        // Swing
        float startTime = Time.time;
        while (Time.time < startTime + AnimationSwingTime) {
            float t = (Time.time - startTime) / AnimationSwingTime;
            t = Interpolation.ExpoIn(t);
            this.transform.localPosition = Vector3.LerpUnclamped(StartPos, EndPos, t);
            this.transform.localRotation = Quaternion.SlerpUnclamped(StartRotQuat, EndRotQuat, t);
            yield return null;
        }
        this.transform.localPosition = EndPos;
        this.transform.localRotation = EndRotQuat;

        SlamAttack();
        Player.Instance.CanWalk = true;

        // Recoil
        startTime = Time.time;
        while (Time.time < startTime + AnimationRecoilTime) {
            float t = (Time.time - startTime) / AnimationRecoilTime;

            this.transform.localPosition = Vector3.LerpUnclamped(EndPos, StartPos, t);
            this.transform.localRotation = Quaternion.SlerpUnclamped(EndRotQuat, StartRotQuat, t);
            yield return null;
        }

        this.transform.localPosition = StartPos;
        this.transform.localRotation = StartRotQuat;
    }

    private void SlamAttack() {
        Player.Instance.velocity.y = PlayerJump;

        DustEffect.Play();

        int layermask = PhysicsCollisionMatrix.Instance.MaskForLayer(this.gameObject.layer);
        Collider[] colliders = Physics.OverlapSphere(Player.Instance.transform.position, SlamRadius, layermask);
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
                        Vector3 dir = c.transform.position - SlamCenter.position;
                        //dir.y = 0.0f;
                        dir = dir.normalized;
                        enemy.Explode(dir * RigidbodyKnockback, SlamCenter.position);
                    } else {
                        Vector3 dir = c.transform.position - SlamCenter.position;
                        dir.y = 0.0f;
                        dir = dir.normalized;
                        enemy.Knockback(dir * Knockback);
                    }
                }
            } else {
                Rigidbody rb = c.GetComponentInChildren<Rigidbody>();
                if (rb == null) { rb = c.GetComponentInParent<Rigidbody>(); }

            if (rb != null) {
                    Vector3 dir = c.transform.position - SlamCenter.position;
                    //dir.y = 0.0f;
                    dir = dir.normalized;
                    rb.AddForceAtPosition(dir * RigidbodyKnockback, SlamCenter.position, ForceMode.Impulse);
                }
            }
        }
    }

}
