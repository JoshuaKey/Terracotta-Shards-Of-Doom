using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon {

    protected new Rigidbody rigidbody;
    protected new Collider collider;

    //private bool isSwinging = false;
    private List<GameObject> enemiesHit = new List<GameObject>();

    protected void Start() {
        collider = GetComponent<Collider>();
        if (collider == null) { collider = GetComponentInChildren<Collider>(true); }

        rigidbody = GetComponent<Rigidbody>();
        if (rigidbody == null) { rigidbody = GetComponentInChildren<Rigidbody>(true); }

        collider.enabled = false;

        CanCharge = false;
        Type = DamageType.BASIC;

        //Vector3 p0;
        //Vector3 p2;
        //float t;
        //float maxDist = 50;
        //float dist = maxDist - (p2 - p0).magnitude;
        //Vector3 p1 = Utility.CreatePeak(p0, p2, 0.5f, dist);
    }
    public override void Attack() {
        if (!CanAttack()) { return; }

        base.Attack();
        StartCoroutine(Swing());
    }

    private IEnumerator Swing() {
        collider.enabled = true;

        Vector3 startPos = this.transform.localPosition; // new Vector3(0.5f, -0.25f, 0.777f);
        Vector3 endPos = new Vector3(-0.2f, -0.55f, 0.777f);
        Quaternion startRot = this.transform.localRotation; //  Quaternion.Euler(new Vector3(0, 45, -10));
        Quaternion endRot = Quaternion.Euler(new Vector3(10, 45, 120));

        // Swing Blade Down
        float startTime = Time.time;
        float length = AttackSpeed * 0.4f; // (40%)
        while (Time.time < startTime + length) {
            float t = (Time.time - startTime) / length;

            Vector3 pos = this.transform.localPosition;
            Quaternion rot = this.transform.localRotation;

            pos.x = Mathf.Lerp(startPos.x, endPos.x, t);
            pos.y = Mathf.Lerp(startPos.y, endPos.y, t);
            pos.z = Mathf.Lerp(startPos.z, endPos.z, t);

            rot.x = Mathf.Lerp(startRot.x, endRot.x, t);
            rot.y = Mathf.Lerp(startRot.y, endRot.y, t);
            rot.z = Mathf.Lerp(startRot.z, endRot.z, t);
            rot.w = Mathf.Lerp(startRot.w, endRot.w, t);

            this.transform.localPosition = pos;
            this.transform.localRotation = rot;
            yield return null;
        }
        this.transform.localPosition = endPos;
        this.transform.localRotation = endRot;

        collider.enabled = false;
        enemiesHit.Clear();

        // Move to Start Pos
        startTime = Time.time;
        length = AttackSpeed * 0.6f; // (60%)
        while (Time.time < startTime + length) {
            float t = (Time.time - startTime) / length;

            Vector3 pos = this.transform.localPosition;
            Quaternion rot = this.transform.localRotation;

            pos.x = Mathf.Lerp(endPos.x, startPos.x, t);
            pos.y = Mathf.Lerp(endPos.y, startPos.y, t);
            pos.z = Mathf.Lerp(endPos.z, startPos.z, t);

            rot.x = Mathf.Lerp(endRot.x, startRot.x, t);
            rot.y = Mathf.Lerp(endRot.y, startRot.y, t);
            rot.z = Mathf.Lerp(endRot.z, startRot.z, t);
            rot.w = Mathf.Lerp(endRot.w, startRot.w, t);

            this.transform.localPosition = pos;
            this.transform.localRotation = rot;
            yield return null;
        }
        this.transform.localPosition = startPos;
        this.transform.localRotation = startRot;
    }

    protected void OnTriggerEnter(Collider other) {
        if (!enemiesHit.Contains(other.gameObject)) {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null) {
                enemiesHit.Add(other.gameObject);

                enemy.health.TakeDamage(this.Type, this.Damage);

                //OnEnemyHit?.Invoke(enemy);
            }
        }
    }
}
