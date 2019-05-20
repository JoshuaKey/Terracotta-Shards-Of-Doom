using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss3Pot : Pot {

    [Header("Snowball")]
    public GameObject/*Snowball*/ SnowballPrefab;
    public float SnowballSpawnTime = 5.0f;

    [Header("Blocks")]
    public List<GameObject> Blocks;
    public List<Transform> BlockPositions;

    [Header("Components")]
    public Enemy enemy;
    public NavMeshAgent navMeshAgent;
    public Collider arenaCollider;

    // Start is called before the first frame update
    void Start() {
        if (enemy == null) { enemy = GetComponentInChildren<Enemy>(true); }
        if (navMeshAgent == null) { navMeshAgent = GetComponentInChildren<NavMeshAgent>(true); }

        gameObject.tag = "Boss";

        stateMachine = new StateMachine();
        stateMachine.Init(gameObject,
            new Boss3_Idle(),
            new Boss3_Vulnerable(),
            new Boss3_Snowball());

        enemy.health.OnDamage += ChangeHealthUI;
        enemy.health.OnDamage += PlayClang;
        enemy.health.OnDeath += OnDeath;
        ChangeHealthUI(0);
    }

    void Update() {
        stateMachine.Update();
    }

    public void PlayClang(float damage) {
        if (!health.IsDead()) {
            AudioManager.Instance.PlaySoundWithParent("clang", ESoundChannel.SFX, gameObject);
        }
    }

    private void ChangeHealthUI(float val) {
        PlayerHud.Instance.SetBossHealthBar(enemy.health.CurrentHealth / enemy.health.MaxHealth);
    }

    private void OnDeath() {
        PlayerHud.Instance.DisableBossHealthBar();
    }

    public void RemoveBlock() {
        Blocks.RemoveAt(0);
        for(int i = 0; i < Blocks.Count; i++) {
            Blocks[i].transform.position = BlockPositions[i].position;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(Game.Instance.PlayerTag)) {
            if (arenaCollider.enabled) {
                arenaCollider.enabled = false;
            } 
        }
    }
}

public class Boss3_Idle : State {
    Boss3Pot boss3Pot = null;
    public override void Init(GameObject owner) {
        base.Init(owner);
        boss3Pot = owner.GetComponent<Boss3Pot>();       
    }

    public override void Enter() {
        boss3Pot.enemy.health.Resistance = DamageType.BASIC | DamageType.EXPLOSIVE | DamageType.FIRE | DamageType.ICE | DamageType.LIGHTNING | DamageType.TRUE;
    }

    public override void Exit() {

    }

    public override string Update() {
        if (!boss3Pot.arenaCollider.enabled) {
            return "Boss3_Snowball";
        }
        boss3Pot.Waddle();
        return null;
    }
}

public class Boss3_Snowball : State {

    Boss3Pot boss3Pot = null;


    public override void Init(GameObject owner) {
        base.Init(owner);
        boss3Pot = owner.GetComponent<Boss3Pot>();
    }

    public override void Enter() {
       boss3Pot.enemy.health.Resistance = DamageType.BASIC | DamageType.EXPLOSIVE | DamageType.FIRE | DamageType.ICE | DamageType.LIGHTNING | DamageType.TRUE;
    }

    public override void Exit() {

    }

    public override string Update() {
        //if (!boss3Pot.arenaCollider.enabled) {
        //    return "Boss3_Snowball";
        //}
        return null;
    }
}

public class Boss3_Vulnerable : State {

    Boss3Pot boss3Pot = null;
    float healthThreshold; // 
    float nextThreshold;

    public override void Init(GameObject owner) {
        base.Init(owner);
        boss3Pot = owner.GetComponent<Boss3Pot>();
        healthThreshold = boss3Pot.enemy.health.MaxHealth / boss3Pot.Blocks.Count;
        nextThreshold = boss3Pot.enemy.health.MaxHealth - healthThreshold;
    }

    public override void Enter() {
        boss3Pot.enemy.health.Resistance = 0;
    }

    public override void Exit() {
        nextThreshold = boss3Pot.enemy.health.MaxHealth - healthThreshold;
    }

    public override string Update() {
        if (boss3Pot.enemy.health.CurrentHealth <= nextThreshold) {
            return "Boss3_Snowball";
        }

        // Visual Logic...

        return null;
    }
}

// Idle Phase
// Waits and does nothing (animates?)
// Switch to Snowball phase when trigger is hit

// Vulnerable Phase
// Teeters around on ground
// Bird / star particle effect
// Waits until health threshold
// Goes to Snowball phase when below threshold

// Spawn Snowball Phase
// Invulnerable
// Spawns Snowballs at player
// When Snowball hits block, block breaks, boss falls to ground, enter Vulnerable Phase

// Snowball
// Spawns in front of the blocks
// When it is ready, the boss calculates the direction from snowball to player and FIRES
// Snowball moves forward while gaining speed (?) from angle and terrain. (small bumps?)
// If the snowball hits the player, it deals damage, knockback (?), and dissipates
// If the player hits the snowball with a melee weapon, it moves towards the boss / block
// When the snowball hits the boss / block, the next block is destroyed and the Boss becomes vulnerable
// We should have 


///////////////////////////////////////////////////////////////////////////////
// TargetProjectileReceiver
// Has a collider, if Target Projectile enters collider, send the Target Projectile back...

// Snowball
// TargetSpeed = 0

// In Sword.cs
// OnTriggerEnter()
// TargetProjectile targetPro = GetComponent<TargetProjectile>();
// var dir = ...
// targetPro.Hit(this.gameObject, dir);

// OnHit for TargetProjectile...


// Things to Do
// Target projectile is Good
// Boss3_Idle is Good
// Boss3_Vulnerable needs animation (Maybe add an in between state for jumping down?)
// Bos3_Snowball needs animation and logic
// Add ThrowSnowball() to Boss3 
// Add Colliders with TargetBlock Tag
// Add Vulnerable Animation and Particle
// Set Snowball TargetSpeed = 0
// Add Code to Melee weapons for hitting Target Projectile
// Add Snowball.OnHit += DamagePlayer (+ knockback)
// TEST

