using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncher : AdvancedWeapon {
    [Header("Distance")]
    public float MinDistance = 5f;
    public float MaxDistance = 100f;
    public float Impulse = 5f;
    public LayerMask AimLayer;

    [Header("Visuals")]
    public Rocket RocketPrefab;
    public Transform RocketPos;
    public Transform ChargedRocketPos;
    //public ArrowPool arrowPool;

    [Header("Animation")]
    //public GameObject drawString;
    //private Vector3 drawStringDefaultPos;

    private Rocket currRocket = null;
    private bool hasReloaded = false;

    private void Awake() {
        CanCharge = false;
        Type = DamageType.EXPLOSIVE;
        OldWeaponName = "Crossbow";

        this.name = "Magic Missile";
    }

    void Start() {
        //drawStringDefaultPos = drawString.transform.localPosition;
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
        float dist = (aimPoint - this.transform.position).magnitude;

        Quaternion newRot = Quaternion.LookRotation((aimPoint - this.transform.position) / dist, Vector3.up);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, newRot, 0.1f);

        //AnimateDrawString();

        //// Debug
        //Debug.DrawLine(this.transform.position, aimPoint, Color.blue);
        //Debug.DrawLine(player.transform.position, aimPoint, Color.blue);
        //Debug.DrawLine(camera.transform.position, aimPoint, Color.blue);
    }

    private void OnEnable() {
        if (Player.Instance && Player.Instance.GetCurrentWeapon() == this) {
            PlayerHud.Instance.EnableCrosshair();
        }
        if (!currRocket) {
            currRocket = GameObject.Instantiate(RocketPrefab, this.transform);
        }
        if (!hasReloaded) {
            StartCoroutine(Reload());
        } else {
            currRocket.transform.position = ChargedRocketPos.position;
        }
    }
    private void OnDisable() {
        if (Player.Instance && Player.Instance.GetCurrentWeapon() == this) {
            PlayerHud.Instance.DisableCrosshair();
        }
        StopAllCoroutines();
    }

    //public void AnimateDrawString() {
    //    if (currRocket == null) {
    //        drawString.transform.localPosition = drawStringDefaultPos;
    //    } else {
    //        drawString.transform.position = currRocket.transform.position - transform.forward * 0.225f;
    //    }
    //}

    public override void Attack() {
        if (!CanAttack() && !hasReloaded) { return; }

        base.Attack();
        Shoot();
        StartCoroutine(Reload());
    }

    private void Shoot() {
        currRocket.Impulse = Impulse;
        currRocket.LifeTime = 20f;
        currRocket.Damage = this.Damage;
        currRocket.Type = this.Type;
        currRocket.Knockback = this.Knockback;
        currRocket.RigidbodyKnockback = this.RigidbodyKnockback;
        currRocket.Fire();

        currRocket = null;
    }

    public override bool CanAttack() {
        return Time.time > nextAttackTime && hasReloaded;
    }

    public override bool CanSwap() {
        return true;
    }

    private IEnumerator Reload() {
        hasReloaded = false;
        if (!currRocket) {
            currRocket = GameObject.Instantiate(RocketPrefab, this.transform);
            currRocket.transform.position = RocketPos.position;
        }

        float startTime = Time.time;
        while (Time.time < startTime + AttackSpeed) {
            float t = (Time.time - startTime) / AttackSpeed;

            Vector3 rocketPos = Interpolation.BezierCurve(RocketPos.position, ChargedRocketPos.position, t);
            currRocket.transform.position = rocketPos;

            yield return null;
        }

        Debug.Log(ChargedRocketPos);
        currRocket.transform.position = ChargedRocketPos.position;
        hasReloaded = true;
        AudioManager.Instance.PlaySound("lever", ESoundChannel.SFX);
    }
}
