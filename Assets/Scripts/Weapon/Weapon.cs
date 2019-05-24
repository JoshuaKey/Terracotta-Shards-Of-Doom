using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    [HideInInspector]
    public bool CanCharge = false;
    [HideInInspector]
    public DamageType Type;

    [Header("Speed")]
    public float AttackSpeed;

    [Header("Damage")]
    public float Damage;
    public float Knockback;
    public float KnockbackDuration = 1.0f;
    public float RigidbodyKnockback;

    //public delegate void EnemyAction(Enemy enemy);
    //public EnemyAction OnEnemyHit;

    protected float nextAttackTime = 0.0f;

    public virtual void Charge() { }
    public virtual void Attack() { nextAttackTime = Time.time + AttackSpeed;  }

    public virtual bool CanAttack() {
        return Time.time > nextAttackTime;
    }
    public virtual bool CanSwap() { return CanAttack(); }
}
