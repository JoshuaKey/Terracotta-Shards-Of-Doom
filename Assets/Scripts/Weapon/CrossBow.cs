using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossBow : Weapon {

    [Header("Distance")]
    public float MinDistance = 5f;
    public float MaxDistance = 100f;
    public float Impulse = 5f;
    public LayerMask AimLayer;

    [Header("Visuals")]
    public Arrow ArrowPrefab;
    public Transform ArrowPos;
    public Transform ChargedArrowPos;
    public ArrowPool arrowPool;

    [Header("Animation")]
    public GameObject drawString;
    private Vector3 drawStringDefaultPos;

    private Arrow currArrow = null;

    void Start() {
        CanCharge = false;
        Type = DamageType.BASIC;

        this.name = "Crossbow";

        drawStringDefaultPos = drawString.transform.localPosition;
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

        AnimateDrawString();

        // Debug
        Debug.DrawLine(this.transform.position, aimPoint, Color.blue);
        Debug.DrawLine(player.transform.position, aimPoint, Color.blue);
        Debug.DrawLine(camera.transform.position, aimPoint, Color.blue);
    }

    private void OnEnable() {
        if (Player.Instance && Player.Instance.GetCurrentWeapon() == this) {
            PlayerHud.Instance.EnableCrosshair();
        }
        if (!currArrow) {
            currArrow = GameObject.Instantiate(ArrowPrefab, this.transform);
            currArrow.transform.position = ChargedArrowPos.position;
        }       
    }
    private void OnDisable() {
        if (Player.Instance && Player.Instance.GetCurrentWeapon() == this) {
            PlayerHud.Instance.DisableCrosshair();
        }
        StopAllCoroutines();
    }

    public void AnimateDrawString()
    {
        if(currArrow == null)
        {
            drawString.transform.localPosition = drawStringDefaultPos;
        }
        else
        {
            drawString.transform.position = currArrow.transform.position - transform.forward * 0.225f;
        }
    }

    public override void Attack() {
        if (!CanAttack()) { return; }

        base.Attack();
        Shoot();
        StartCoroutine(Reload());
    }

    private void Shoot() {
        currArrow.Impulse = Impulse;
        currArrow.LifeTime = 20f;
        currArrow.Damage = this.Damage;
        currArrow.Type = this.Type;
        currArrow.Knockback = this.Knockback;
        currArrow.RigidbodyKnockback = this.RigidbodyKnockback;
        currArrow.Fire();

        currArrow = null;
    }

    private IEnumerator Reload() {
        if (!currArrow) {
            currArrow = GameObject.Instantiate(ArrowPrefab, this.transform);
            currArrow.transform.position = ArrowPos.position;
        }

        float startTime = Time.time;
        while(Time.time < startTime + AttackSpeed) {
            float t = (Time.time - startTime) / AttackSpeed;

            Vector3 arrowPos = Interpolation.BezierCurve(ArrowPos.position, ChargedArrowPos.position, t);
            currArrow.transform.position = arrowPos;

            yield return null;
        }

        currArrow.transform.position = ChargedArrowPos.position;
    }
}
