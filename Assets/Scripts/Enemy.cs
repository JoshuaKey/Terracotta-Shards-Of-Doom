using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Enemy : MonoBehaviour {

    [Header("Combat")]
    public float AttackSpeed = .5f;
    public float Damage = 1f;
    public float Health;
    public float MaxHealth;

    private new Collider collider;

    void Start() {
        collider = GetComponent<Collider>();
        if (collider == null) { collider = GetComponentInChildren<Collider>(); }

        this.gameObject.tag = "Enemy";
        this.gameObject.layer = LayerMask.NameToLayer("Enemy");
    }

    public void Attack() {
        // Raycast

        // Attack Collision Animation

        // Box Popup...
    }

    public void Heal(float health) {
        Health = Mathf.Max(MaxHealth, Health + health);

        print(this.name + " (Heal): " + Health + "/" + MaxHealth);
    }

    public void TakeDamage(float damage) {
        Health -= damage;

        print(this.name + " (Damage): " + Health + "/" + MaxHealth);
    }

    public bool IsDead() {
        return Health <= 0.0f;
    }
}
