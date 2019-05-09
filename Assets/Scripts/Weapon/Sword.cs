using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon {

    [Header("Swing Animation")]
    public Vector3 StartPos = new Vector3(0.5f, -0.25f, 0.777f);
    public Vector3 EndPos = new Vector3(-0.2f, -0.55f, 0.777f);
    public float AnimationSwingTime = 0.2f;

    [Header("Recoil Animation")]
    public Vector3 StartRot = new Vector3(0, 45, -10);
    public Vector3 EndRot = new Vector3(10, 45, 120);
    public float AnimationRecoilTime = 0.3f;

    protected new Rigidbody rigidbody;
    protected new Collider collider;
    protected Animator animator;

    private List<GameObject> enemiesHit = new List<GameObject>();

    protected void Start() {
        if (collider == null) { collider = GetComponentInChildren<Collider>(true); }
        if (rigidbody == null) { rigidbody = GetComponentInChildren<Rigidbody>(true); }
        if (animator == null) { animator = GetComponentInChildren<Animator>(true); }

        collider.enabled = false;

        CanCharge = false;
        Type = DamageType.BASIC;

        this.name = "Sword";
    }

    private void OnEnable() {
        this.transform.localPosition = StartPos;
        this.transform.localRotation = Quaternion.Euler(StartRot);
    }
    private void OnDisable() {
        StopAllCoroutines();
        if (collider) {
            collider.enabled = false;
        }
    }

    public override void Attack() {
        if (!CanAttack()) { return; }

        base.Attack();
        //StartCoroutine(Swing());
        animator.SetTrigger("Swing");
    }

    public void StartSwing()
    {
        collider.enabled = true;
        AudioManager.Instance.PlaySoundWithParent("swoosh", ESoundChannel.SFX, gameObject);
    }

    public void EndSwing()
    {
        collider.enabled = false;
        enemiesHit.Clear();
    }

    private IEnumerator Swing() {
        Quaternion StartRotQuat = Quaternion.Euler(StartRot);
        Quaternion EndRotQuat = Quaternion.Euler(EndRot);

        collider.enabled = true;

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

        collider.enabled = false;
        enemiesHit.Clear();

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

    protected void OnTriggerEnter(Collider other) {
        if (!enemiesHit.Contains(other.gameObject)) {
            Enemy enemy = other.GetComponentInChildren<Enemy>();
            if(enemy == null) { enemy = other.GetComponentInParent<Enemy>(); }
            if (enemy != null) {
                enemiesHit.Add(other.gameObject);

                float damage = enemy.health.TakeDamage(this.Type, this.Damage);
                bool isDead = enemy.health.IsDead();
                if (damage > 0) {
                    if (isDead) {
                        Vector3 forward = Player.Instance.camera.transform.forward;
                        //forward = forward.normalized;
                        enemy.Explode(forward * RigidbodyKnockback, Player.Instance.camera.transform.position);
                    } else {
                        Vector3 forward = Player.Instance.camera.transform.forward;
                        forward.y = 0.0f;
                        forward = forward.normalized;
                        enemy.Knockback(forward * Knockback);
                    }
                }
            } else {
                Rigidbody rb = other.GetComponentInChildren<Rigidbody>();
                if (rb == null) { rb = other.GetComponentInParent<Rigidbody>(); }
                if (rb != null) {
                    Vector3 forward = Player.Instance.camera.transform.forward;
                    //forward = forward.normalized;
                    rb.AddForce(forward * RigidbodyKnockback, ForceMode.Impulse);
                }
            }
        }
    }
}
