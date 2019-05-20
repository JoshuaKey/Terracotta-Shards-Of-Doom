using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetProjectile : MonoBehaviour {

    public float TargetSpeed = 5;

    [Header("Components")]
    public new Rigidbody rigidbody;
    public new Collider collider;

    public Action<GameObject> OnHit;
    public Action OnMiss;

    private GameObject sender;
    private GameObject target;

    // Start is called before the first frame update
    void Start() {
        if (collider == null) { collider = GetComponentInChildren<Collider>(true); }

        if (rigidbody == null) { rigidbody = GetComponentInChildren<Rigidbody>(true); }

        collider.enabled = false;
        rigidbody.isKinematic = true;
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (!rigidbody.isKinematic && target != null) {
            Vector3 dir = target.transform.position - this.transform.position;
            dir = dir.normalized * TargetSpeed;
            rigidbody.AddForce(dir, ForceMode.Impulse);
        }
    }

    public void Fire(GameObject _sender, GameObject _target, Vector3 impulse) {
        sender = _sender;
        target = _target;

        collider.enabled = true;
        rigidbody.isKinematic = false;

        rigidbody.AddForce(impulse, ForceMode.Impulse);

        this.transform.parent = null;
        LevelManager.Instance.MoveToScene(this.gameObject);
    }

    public void Hit(GameObject _sender, Vector3 impulse) {
        target = sender;
        sender = _sender;

        rigidbody.velocity = Vector3.zero;
        rigidbody.AddForce(impulse, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject == target) {
            OnHit?.Invoke(other.gameObject);
        } else if (other.CompareTag("TargetBlock")) {
            OnMiss?.Invoke();
        }
    }
}
