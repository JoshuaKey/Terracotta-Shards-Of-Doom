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

    private new Collider collider;

    void Start() {
        collider = GetComponent<Collider>();
        if (collider == null) { collider = GetComponentInChildren<Collider>(); }

        pot = GetComponent<Pot>();
        if (pot == null) { pot = GetComponentInChildren<Pot>(); }

        brokenPot = GetComponent<BrokenPot>();
        if (brokenPot == null) { brokenPot = GetComponentInChildren<BrokenPot>(); }

        health = GetComponent<Health>();
        if (health == null) { health = GetComponentInChildren<Health>(); }

        this.gameObject.tag = "Enemy";
        this.gameObject.layer = LayerMask.NameToLayer("Enemy");

        health.OnEnemyDeath += brokenPot.Activate;
    }

    public void Attack() {

    }
}
