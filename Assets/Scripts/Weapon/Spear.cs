using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : Weapon {

	[Header("Damage")]
	public float MinDamage = .5f;
	public float MinCharge = .25f;

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

	[SerializeField] Transform spearChargePos = null;
	private float charge;
	private float currentDamage;
	[SerializeField] GameObject spearModel = null;
	[SerializeField] float MinKnockback = 0.0f;
	private float currentKnockback;

    protected void Start() {
        if (collider == null) { collider = GetComponentInChildren<Collider>(true); }
        if (rigidbody == null) { rigidbody = GetComponentInChildren<Rigidbody>(true); }

        collider.enabled = false;

        CanCharge = true;
        Type = DamageType.BASIC;

        this.name = "Spear";
    }

    private void OnEnable() {
        spearModel.transform.localPosition = StartPos;
        spearModel.transform.localRotation = Quaternion.Euler(StartRot);
    }
    private void OnDisable() {
        StopAllCoroutines();
        if (collider) {
            collider.enabled = false;
        }
    }

	public override void Charge(){
		charge += Time.deltaTime / AttackSpeed;
		charge = Mathf.Min(charge, 1.0f);

		float t = Interpolation.CubicOut(charge);
		Vector3 spearPos = Interpolation.BezierCurve(StartPos, spearChargePos.localPosition, t);
		spearModel.transform.localPosition = spearPos;
        this.currentDamage = Mathf.Lerp(MinDamage, Damage, charge);//charge == 1 ? Damage : MinDamage; 
        this.currentKnockback = charge == 1 ? Knockback : MinKnockback;
	}

	public override void Attack() {
        if (!CanAttack() || charge == 0) {
			return;
		}
		//Debug.Log("OH GOD WHY AM I JABBING WITH THIS SPEAR");

		//base.Attack();
		StartCoroutine(Stab());
		charge = 0.0f;
		return;
	}

    private IEnumerator Stab() {
		//Debug.Log("BIG STAB");
        collider.enabled = true;
        Quaternion StartRotQuat = Quaternion.Euler(StartRot);
        Quaternion EndRotQuat = Quaternion.Euler(EndRot);

        // Swing
        float startTime = Time.time;
        while (Time.time < startTime + AnimationSwingTime) {
            float t = (Time.time - startTime) / AnimationSwingTime;
            t = Interpolation.ExpoIn(t);
            spearModel.transform.localPosition = Vector3.LerpUnclamped(spearChargePos.localPosition, EndPos, t);
            spearModel.transform.localRotation = Quaternion.SlerpUnclamped(StartRotQuat, EndRotQuat, t);
            yield return null;
        }
        spearModel.transform.localPosition = EndPos;
        spearModel.transform.localRotation = EndRotQuat;

        collider.enabled = false;
        enemiesHit.Clear();

        // Recoil
        startTime = Time.time;
        while (Time.time < startTime + AnimationRecoilTime) {
            float t = (Time.time - startTime) / AnimationRecoilTime;

            spearModel.transform.localPosition = Vector3.LerpUnclamped(EndPos, StartPos, t);
            spearModel.transform.localRotation = Quaternion.SlerpUnclamped(EndRotQuat, StartRotQuat, t);
            yield return null;
        }

        spearModel.transform.localPosition = StartPos;
        spearModel.transform.localRotation = StartRotQuat;
    }


    protected void OnTriggerEnter(Collider other) {
        TargetProjectile targetProj = other.GetComponentInChildren<TargetProjectile>();
        if (targetProj != null) {
            Vector3 dir = targetProj.transform.position - Player.Instance.transform.position;
            dir.y = 0.0f;
            dir = dir.normalized;
            targetProj.Hit(Player.Instance.gameObject, dir * Knockback);
            return;
        }

        if (!enemiesHit.Contains(other.gameObject)) {
            Enemy enemy = other.GetComponentInChildren<Enemy>();
            if (enemy == null) { enemy = other.GetComponentInParent<Enemy>(); }
            if (enemy != null) {
                enemiesHit.Add(other.gameObject);

                float damage = enemy.health.TakeDamage(this.Type, this.currentDamage);
                bool isDead = enemy.health.IsDead();
                if (damage > 0) {
                    if (isDead) {
                        Vector3 forward = this.transform.forward;
                        //forward.y = 0.0f;
                        //forward = forward.normalized;
                        enemy.Explode(forward * RigidbodyKnockback, spearModel.transform.position);
                    } else {
                        Vector3 forward = Player.Instance.camera.transform.forward;
                        forward.y = 0.0f;
                        forward = forward.normalized;
                        enemy.Knockback(forward * Knockback, KnockbackDuration);
                    }
                }
            } else {
                Rigidbody rb = other.GetComponentInChildren<Rigidbody>();
                if (rb == null) { rb = other.GetComponentInParent<Rigidbody>(); }
                if (rb != null) {
                    Vector3 forward = this.transform.forward;
                    //forward.y = 0.0f;
                    //forward = forward.normalized;
                    rb.AddForce(forward * RigidbodyKnockback, ForceMode.Impulse);
                }
            }
        }
    }
}
