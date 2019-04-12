using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    [HideInInspector]
    public bool CanCharge = false;
    [HideInInspector]
    public DamageType Type;

    public float AttackSpeed;
    public float Damage;
    public LayerMask AttackLayer;

    public delegate void EnemyAction(Enemy enemy);
    public EnemyAction OnEnemyHit;

    protected new Rigidbody rigidbody;
    protected new Collider collider;

    protected float nextAttackTime = 0.0f;

    void Start() {
        collider = GetComponent<Collider>();
        if (collider == null) { collider = GetComponentInChildren<Collider>(); }

        rigidbody = GetComponent<Rigidbody>();
        if (rigidbody == null) { rigidbody = GetComponentInChildren<Rigidbody>(); }

        collider.enabled = false;
    }

    public virtual void Charge() { }
    public virtual void Attack() { nextAttackTime = Time.time + AttackSpeed;  }

    public bool CanAttack() {
        return Time.time <= nextAttackTime;
    }

    // TEMP -------------------------------
    //public string EnemyTag = "Enemy";

    //private new Collider collider;

    //public delegate void EnemyAction(Enemy enemy);
    //public event EnemyAction OnEnemyHit;

    //private bool isSwinging = false;
    //private List<GameObject> enemiesHit = new List<GameObject>();

    // Collision
    //private int playerLayerMask;

    //void Start() {
    //    collider = GetComponent<Collider>();
    //    if (collider == null) { collider = GetComponentInChildren<Collider>(); }

    //    collider.enabled = false;

    //    playerLayerMask = 1 << LayerMask.NameToLayer("Player");
    //}

    //private void Update() {
    //    if (isSwinging) {
    //        Collider[] colliders = Physics.OverlapBox(collider.bounds.center, collider.bounds.extents, this.transform.rotation, ~playerLayerMask);

    //        foreach(Collider hit in colliders) {
    //            print("Weapon hit " + hit.name);
    //            if (hit.CompareTag(EnemyTag)) {
    //                if (!enemiesHit.Contains(hit.gameObject)) {
    //                    Enemy enemy = hit.GetComponent<Enemy>();
    //                    if (enemy != null && !enemiesHit.Contains(hit.gameObject)) {
    //                        //print("Dealing Damage to " + hit.name);
    //                        enemiesHit.Add(hit.gameObject);
    //                        OnEnemyHit?.Invoke(enemy);
    //                    }
    //                } else {
    //                    //print("Already Dealt Damage to " + hit.name);
    //                }
    //            }
    //        }
    //    }
    //}

    //public IEnumerator Swing(float attackSpeed) {
    //    if (isSwinging) {
    //        yield break;
    //    }

    //    // Now attacking
    //    isSwinging = true;
    //    collider.enabled = true;

    //    // (.5, 1, 1) (-45, 0, 0) -> (0, 0, 1) (45, -45, 0)
    //    //Vector3 startPos = new Vector3(0.5f, 1, 1);
    //    //Vector3 endPos = new Vector3(0, 0, 1);
    //    //Quaternion startRot = Quaternion.Euler(new Vector3(-45, 0, 0));
    //    //Quaternion endRot = Quaternion.Euler(new Vector3(45, -45, 0));
    //    Vector3 startPos = new Vector3(0.5f, 0.5f, 0.777f);
    //    Vector3 endPos = new Vector3(-0.2f, 0.2f, 0.777f);
    //    Quaternion startRot = Quaternion.Euler(new Vector3(0, 45, -10));
    //    Quaternion endRot = Quaternion.Euler(new Vector3(10, 45, 120));

    //    // Swing Blade Down
    //    float startTime = Time.time;
    //    float length = attackSpeed * 0.4f; // (40%)
    //    while (Time.time < startTime + length) {
    //        float t = (Time.time - startTime) / length;

    //        Vector3 pos = this.transform.localPosition;
    //        Quaternion rot = this.transform.localRotation;

    //        pos.x = Mathf.Lerp(startPos.x, endPos.x, t);
    //        pos.y = Mathf.Lerp(startPos.y, endPos.y, t);
    //        pos.z = Mathf.Lerp(startPos.z, endPos.z, t);

    //        rot.x = Mathf.Lerp(startRot.x, endRot.x, t);
    //        rot.y = Mathf.Lerp(startRot.y, endRot.y, t);
    //        rot.z = Mathf.Lerp(startRot.z, endRot.z, t);
    //        rot.w = Mathf.Lerp(startRot.w, endRot.w, t);

    //        this.transform.localPosition = pos;
    //        this.transform.localRotation = rot;
    //        yield return null;
    //    }
    //    this.transform.localPosition = endPos;
    //    this.transform.localRotation = endRot;

    //    // Move to Start Pos
    //    startTime = Time.time;
    //    length = attackSpeed * 0.6f; // (60%)
    //    while (Time.time < startTime + length) {
    //        float t = (Time.time - startTime) / length;

    //        Vector3 pos = this.transform.localPosition;
    //        Quaternion rot = this.transform.localRotation;

    //        pos.x = Mathf.Lerp(endPos.x, startPos.x, t);
    //        pos.y = Mathf.Lerp(endPos.y, startPos.y, t);
    //        pos.z = Mathf.Lerp(endPos.z, startPos.z, t);

    //        rot.x = Mathf.Lerp(endRot.x, startRot.x, t);
    //        rot.y = Mathf.Lerp(endRot.y, startRot.y, t);
    //        rot.z = Mathf.Lerp(endRot.z, startRot.z, t);
    //        rot.w = Mathf.Lerp(endRot.w, startRot.w, t);

    //        this.transform.localPosition = pos;
    //        this.transform.localRotation = rot;
    //        yield return null;
    //    }
    //    this.transform.localPosition = startPos;
    //    this.transform.localRotation = startRot;

    //    // No longer Attacking
    //    collider.enabled = false;
    //    isSwinging = false;
    //    enemiesHit.Clear();
    //}
}

// Sword - Simple Swing
// Bow - 
//      - Fire a Projectile, Arrow drop off
//      - 
// Hammer - Ground pound
// Fire Sword - Simple Swing 
// Spear - Lunge

// A Weapon should represent an object that that can attack 
// Each 

// CanCharge
// Attack Speed
// Damage
// Charge - Generate Charge based off Delta Time (Power...)
// Attack - Start Animation (?), Collider / Physics

// Each weapon has it's own specific data and logic...
// Pool Manager (?)

// Can we do Boss Attack with Layer?