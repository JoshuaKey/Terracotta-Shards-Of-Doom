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

    //public delegate void EnemyAction(Enemy enemy);
    //public EnemyAction OnEnemyHit;

    protected float nextAttackTime = 0.0f;

    public virtual void Charge() { }
    public virtual void Attack() { nextAttackTime = Time.time + AttackSpeed;  }

    public bool CanAttack() {
        return Time.time > nextAttackTime;
    }
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