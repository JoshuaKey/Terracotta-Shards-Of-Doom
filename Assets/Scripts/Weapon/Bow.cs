using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : Weapon {

    [Header("Damage")]
    public float MinDamage = .5f;
    public float MinCharge = .25f;

    [Header("Aim")]
    public float MinDistance = 5f;
    public float MaxDistance = 100f;
    public LayerMask AimLayer;

    [Header("Force")]
    public float MinSpeed = 0f;
    public float MaxSpeed = 5f;

    [Header("Visuals")]
    public Arrow ArrowPrefab;
    public Transform ChargedArrowPos;
    public ArrowPool arrowPool;

    // Value between 0 and 1;
    private float charge;
    private Arrow currArrow = null;

    // Debug
    private float dist = 0.0f;

    protected void Start() {
        CanCharge = true;
        Type = DamageType.BASIC;

        this.name = "Bow";
    }

    private void Update() {
        Player player = Player.Instance;
        Camera camera = player.camera;
        Vector3 forward = camera.transform.forward;

        Ray ray = new Ray(camera.transform.position + forward * MinDistance, forward);
        RaycastHit hit;
        Vector3 aimPoint = Vector3.zero;
        if (Physics.Raycast(ray, out hit, MaxDistance, AimLayer)) {
            aimPoint = hit.point;
        } else {
            aimPoint = camera.transform.position + forward * MaxDistance;
        }

        // Debug
        dist = (aimPoint - this.transform.position).magnitude;

        Quaternion newRot = Quaternion.LookRotation((aimPoint - this.transform.position) / dist, Vector3.up);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, newRot, 0.1f);

        // Debug
        Debug.DrawLine(this.transform.position, aimPoint, Color.blue);
        Debug.DrawLine(camera.transform.position, aimPoint, Color.blue);
    }

    private void OnEnable() {
        if(Player.Instance && Player.Instance.GetCurrentWeapon() == this) {
            PlayerHud.Instance.EnableCrosshair();
        }
    }

    private void OnDisable() {
        if (Player.Instance && Player.Instance.GetCurrentWeapon() == this) {
            PlayerHud.Instance.DisableCrosshair();
        }

        StopAllCoroutines();
        if(currArrow != null) {
            Destroy(currArrow.gameObject);
        }
        charge = 0.0f;
    }

    public override void Charge() {
        if (!currArrow) {
            currArrow = GameObject.Instantiate(ArrowPrefab, this.transform);
            //currArrow = arrowPool.Create();
            //currArrow.transform.SetParent(this.transform);
        }
        
        charge += Time.deltaTime / AttackSpeed;
        charge = Mathf.Min(charge, 1.0f);

        float t = Interpolation.CubicOut(charge);
        Vector3 arrowPos = Interpolation.BezierCurve(this.transform.position, ChargedArrowPos.position, t);
        currArrow.transform.position = arrowPos;
    }

    public override void Attack() {
        if (!CanAttack() || !currArrow) { return; }

        if(charge < MinCharge) {
            Destroy(currArrow.gameObject);
            charge = 0.0f;
            currArrow = null;
            return;
        }

        float t = Interpolation.QuadraticIn(charge);
        currArrow.Impulse = Mathf.Lerp(MinSpeed, MaxSpeed, t);
        currArrow.LifeTime = 20f;
        currArrow.Damage = charge == 1 ? Damage : MinDamage;
        currArrow.Type = this.Type;
        currArrow.Knockback = this.Knockback;
        currArrow.RigidbodyKnockback = this.RigidbodyKnockback;
        currArrow.Fire();

        charge = 0.0f;
        currArrow = null;
    }
}