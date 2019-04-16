using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

    public delegate void PublicAction();

    [HideInInspector]
    public float CurrentHealth;
    public float MaxHealth = 3f;

    public Action OnEnemyDeath;
    public Action OnEnemyHeal;
    public Action OnEnemyDamage;

    private void Start() {
        Reset();
    }

    public void Reset() {
        CurrentHealth = MaxHealth;
    }

    public void Heal(float health) {
        CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + health);

        print(this.name + " (Heal): " + CurrentHealth + "/" + MaxHealth);

        OnEnemyHeal?.Invoke();
    }

    public void TakeDamage(DamageType type, float damage) {
        // What do we do with Type ???

        CurrentHealth -= damage;

        print(this.name + " (Damage): " + CurrentHealth + "/" + MaxHealth);

        OnEnemyDamage?.Invoke();

        if (IsDead()) {
            OnEnemyDeath?.Invoke();
        }
    }

    public bool IsDead() {
        return CurrentHealth <= 0.0f;
    }
}
