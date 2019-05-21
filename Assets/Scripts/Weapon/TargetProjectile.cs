using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetProjectile : MonoBehaviour {

    public float TargetSpeed = 5;

    [Header("Components")]
    public Attack attack;
    public new Rigidbody rigidbody;
    public new Collider collider;

    public Action<TargetProjectile> OnFire;
    public Action<TargetProjectile, GameObject> OnHit;
    public Action<TargetProjectile> OnMiss;

    private bool hasFired;
    private GameObject sender;
    private GameObject target;

    // Start is called before the first frame update
    void Start() {
        if (collider == null) { collider = GetComponentInChildren<Collider>(true); }
        if (rigidbody == null) { rigidbody = GetComponentInChildren<Rigidbody>(true); }
        if (attack == null) { attack = GetComponentInChildren<Attack>(true); }

        //collider.enabled = false;
        //rigidbody.isKinematic = true;
        //attack.isAttacking = false;
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (!rigidbody.isKinematic && target != null) {
            Vector3 dir = target.transform.position - this.transform.position;
            dir = dir.normalized * TargetSpeed;
            rigidbody.AddForce(dir, ForceMode.Impulse);
        }
    }

    private void OnDisable() {
        OnMiss?.Invoke(this);
        hasFired = false;
        sender = null;
        target = null;
    }

    public void Fire(GameObject _sender, GameObject _target, Vector3 impulse) {
        sender = _sender;
        target = _target;
        hasFired = true;

        collider.enabled = true;
        rigidbody.isKinematic = false;
        attack.isAttacking = true;

        rigidbody.velocity = Vector3.zero;
        rigidbody.AddForce(impulse, ForceMode.Impulse);
        this.transform.forward = impulse.normalized;

        this.transform.parent = null;
        LevelManager.Instance.MoveToScene(this.gameObject);

        OnFire?.Invoke(this);
    }

    public void Hit(GameObject _sender, Vector3 impulse) {
        target = sender;
        sender = _sender;

        rigidbody.velocity = Vector3.zero;
        rigidbody.AddForce(impulse, ForceMode.Impulse);
        this.transform.forward = impulse.normalized;

        OnFire?.Invoke(this);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject == target) {
            OnHit?.Invoke(this, other.gameObject);
        } else if (other.CompareTag("TargetBlock")) {
            OnMiss?.Invoke(this);
        }
    }

    public GameObject GetTarget() { return target; }
    public GameObject GetSender() { return sender; }
    public bool HasFired() { return hasFired; }
}
