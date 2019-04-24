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

// Sword
//  Swings (animation)
//  hits multiple enemies
//  Deals damage on contact
//  has Attack Speed 

// Hammer
//  Swings (animation)
//  hits no enemies (aoe)
//  deals no damage (spawns earthquake)
//  has Attack Speed 

// Spear
//  Lunge (animation)
//  hits multple enemies
//  Deals damage on contact
//  has Attack Speed 

// Bow
//  Charges (animation)
//  hits no enemies (1 per arrow)
//  deals no damage (spawns arrow)
//  basically instant fire (charge)

// Uzi
//  Knockback (animation)
//  hits no enemies (1 per bullet)
//  deals no damage (spawns bullet)
//  has Attack Speed (delay)

// The weapon can damage on Contact, Charge, and Spawn. Any Combination
// Decorator Pattern (?)
// 
