using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    [HideInInspector]
    public Health health;
    [HideInInspector]
    public Pot pot;
    [HideInInspector]
    public BrokenPot brokenPot;

    private new Rigidbody rigidbody;
    private new Collider collider;

    void Start() {
        collider = GetComponent<Collider>();
        if (collider == null) { collider = GetComponentInChildren<Collider>(true); }

        rigidbody = GetComponent<Rigidbody>();
        if (rigidbody == null) { rigidbody = GetComponentInChildren<Rigidbody>(true); }

        pot = GetComponent<Pot>();
        if (pot == null) { pot = GetComponentInChildren<Pot>(true); }

        brokenPot = GetComponent<BrokenPot>();
        if (brokenPot == null) { brokenPot = GetComponentInChildren<BrokenPot>(true); }

        health = GetComponent<Health>();
        if (health == null) { health = GetComponentInChildren<Health>(true); }

        this.gameObject.tag = "Enemy";
        this.gameObject.layer = LayerMask.NameToLayer("Enemy");

        rigidbody.isKinematic = true;

        health.OnDeath += this.Die;
    }

    public void Knockback(Vector3 force) {
        print(force);
        StartCoroutine(KnockbackRoutine(force));
    }

    public void Explode() {

    }

    private void Die() {
        this.gameObject.SetActive(false);
        brokenPot.gameObject.SetActive(true);
        brokenPot.transform.parent = null;

        health.OnDeath -= this.Die;
    }

    protected IEnumerator KnockbackRoutine(Vector3 force) {
        rigidbody.isKinematic = false;
        if (pot != null && pot.GetAgent() != null) {
            pot.GetAgent().enabled = false;
        }

        rigidbody.AddForce(force, ForceMode.Impulse);

        yield return new WaitForSeconds(1f);

        rigidbody.isKinematic = true;
        if (pot != null && pot.GetAgent() != null) {
            pot.GetAgent().enabled = true;
        }
    }
}


