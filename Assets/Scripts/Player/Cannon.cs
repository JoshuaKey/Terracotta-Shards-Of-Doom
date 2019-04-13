using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour {

    // A cannon can be interacted with
    // When Interacted, the Player will align with the Barrel
    // Then the player will shoot, in a specific direction and velocit

    [Header("Launch")]
    [HideInInspector]
    public float BaseAngle;
    [HideInInspector]
    public float BarrelAngle;
    public Transform Target;
    public Transform Peak; // Aka , how high will we get?
    public Transform BarrelChargePos; // z = -.5

    [Header("Damage")]
    public float Damage;
    public float RadiusSize;
    public LayerMask EnemyLayer;

    [Header("Time")]
    public float ChargeTime = 2.5f;
    public float LeapTime = 10.0f;

    [Header("Object")]
    public GameObject Barrel;
    public GameObject Base;

    private Interactable interactable;

    private WaitForSeconds chargedWait;
    private WaitForSeconds leapWait;
    private bool aligning = false;

    // Start is called before the first frame update
    void Start() {
        interactable = GetComponent<Interactable>();
        if (interactable == null) { interactable = GetComponentInChildren<Interactable>(); }

        interactable.Subscribe(FirePlayer);

        BarrelAngle = Barrel.transform.root.localEulerAngles.x;
        BaseAngle = Base.transform.root.localEulerAngles.y;
        Align();

        float max = 50;
        for (int i = 0; i <= max; i++) {
            float t = i / max;

            float t2 = Interpolation.QuinticInOut(t);

            float delta = Interpolation.QuinticInOut(t);
            t = Interpolation.Inverse(t, delta);

            print("Lerp: " + (i / max) + " - " + t + " - " + t2);
        }
    }

    public void Change(float baseAngle, float barrelAngle, Transform target, Transform peak, 
        float chargeTime, float leapTime) {

        //BaseAngle = baseAngle;
        //BarrelAngle = barrelAngle;
        //Target = target;
        //Peak = peak;
        //ChargeTime = chargeTime;
        //LeapTime = leapTime;

        //StartCoroutine(Align(1.0f, .5f));
    }

    //private IEnumerator Align(float baseTime, float barrelTime) {
        //Quaternion startRot = Base.transform.localRotation;
        //Quaternion endRot = startRot * Quaternion.AngleAxis(;
        //float startTime = Time.time;
        //while (Time.time < startTime + baseTime) {
        //    float t = (Time.time - startTime) / baseTime;

        //    Quaternion rot = Quaternion.identity;

        //    pos = Quaternion.SlerpUnclamped(startRot, )

        //    player.transform.position = pos;
        //    yield return null;
        //}
    //}
    [ContextMenu("Align")]
    public void Align() {
        Vector3 baseDir = Target.transform.position - Base.transform.position;
        baseDir.y = 0.0f;
        baseDir = baseDir.normalized;

        Vector3 barrelDir = Peak.position - Barrel.transform.position;
        barrelDir.x = 0.0f;
        barrelDir = barrelDir.normalized;

        Base.transform.forward = baseDir; // Angle on Y Axis
        Barrel.transform.forward = barrelDir; // Angle on X Axis
    }

    public void FirePlayer() {
        Player player = Player.Instance;
        StartCoroutine(Shoot(player));
    }

    public IEnumerator Shoot(Player player) {
        Transform oldParent = player.transform.parent;
        Quaternion oldRot = player.transform.localRotation;

        player.transform.position = Barrel.transform.position;
        player.transform.SetParent(Barrel.transform, true);
        player.transform.up = Barrel.transform.forward;
        player.Orient(Barrel.transform.forward);

        player.CanWalk = false;
        player.CanMove = false;
        player.CanRotate = false;

        while (aligning) {
            yield return null;
        }

        // Charge Animation
        {
            Vector3 startPos = Barrel.transform.position;
            Vector3 endPos = BarrelChargePos.position;
            float startTime = Time.time;
            while (Time.time < startTime + ChargeTime) {
                float t = (Time.time - startTime) / ChargeTime;

                t = Interpolation.BounceOut(t);

                Vector3 pos = Vector3.Lerp(startPos, endPos, t);

                player.transform.position = pos;
                yield return null;
            }
        }

        player.CanRotate = true;

        // Launch Animation
        {
            Vector3 startPos = BarrelChargePos.position;
            float startTime = Time.time;
            while (Time.time < startTime + LeapTime) {
                float t = (Time.time - startTime) / LeapTime;

                //t = Interpolation.SmoothStep(t);
                //t = Interpolation.SmoothStep(t);
                //t = Interpolation.SmoothStep(t);


                //t = Interpolation.InverseSmoothStep(t);
                //t = Interpolation.InverseSmoothStep(t);
                //t = Interpolation.InverseSmoothStep(t);

                //t = Interpolation.QuinticIn(t);
                //t = Interpolation.QuinticOut(t);
                t = t + (t - Interpolation.QuinticInOut(t));

                //float delta = Interpolation.QuinticInOut(t);
                //t = Interpolation.Inverse(t, delta);

                Vector3 pos = Utility.BezierCurve(startPos, Peak.position, Target.position, t);

                player.transform.position = pos;
                yield return null;
            }
            player.transform.position = Target.position;
        }

        Explosion(player.transform.position);

        player.CanWalk = true;
        player.CanMove = true;
        player.transform.SetParent(oldParent, true);
        player.transform.localRotation = oldRot;
    }

    [ContextMenu("Find Mid Point")]
    private void FindMidPoint() {
        Vector3 mid = Barrel.transform.position + .5f * (Target.position - Barrel.transform.position);
        Peak.position = mid;
        Peak.forward = (Target.position - Barrel.transform.position).normalized;
    }

    public void Explosion(Vector3 pos) {
        Collider[] colliders = Physics.OverlapSphere(pos, RadiusSize, EnemyLayer);
        foreach(Collider other in colliders) {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null) {
                enemy.health.TakeDamage(DamageType.EXPLOSIVE, this.Damage);
                //OnEnemyHit?.Invoke(enemy);
            }
        }
    }

}
