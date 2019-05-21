using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss3Pot : Pot {

    [Header("Snowball")]
    public TargetProjectile SnowballPrefab;
    public Transform SpawnPosition;
    public TargetReceiver Reciever;

    [Header("Snowball Values")]
    public float SnowballSpawnTime = 5.0f;
    public float SnowballImpulse = 10.0f;
    private List<TargetProjectile> snowballs = new List<TargetProjectile>();

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
        Reciever.OnReceive += ReceiveSnowball;
    }

    void Update() {
        stateMachine.Update();
        print(stateMachine.GetCurrState().ToString());
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

    public void SpawnSnowball() {
        TargetProjectile snowball = Instantiate(SnowballPrefab, SpawnPosition.position, SpawnPosition.rotation);
        Vector3 dir = Player.Instance.transform.position - snowball.transform.position;
        dir.y = 0.0f;
        dir = dir.normalized;
        snowball.Fire(this.gameObject, Player.Instance.gameObject, dir);
        snowball.OnHit += HitSnowball;
        snowball.OnMiss += MissSnowball;
        snowballs.Add(snowball);
        // Add coroutine
    }

    private void ReceiveSnowball(TargetReceiver receiver, GameObject snowballObj) {
        print("Received " + snowballObj.name);

        stateMachine.ChangeState("Boss3_Vulnerable");

        for (int i = 0; i < snowballs.Count;) {
            TargetProjectile snowball = snowballs[i];
            DestroySnowball(snowball);
        }      
    }

    private void HitSnowball(TargetProjectile snowball, GameObject hitObj) {
        print("Hit");

        DestroySnowball(snowball);
    }

    private void MissSnowball(TargetProjectile snowball) {
        print("Missed");

        DestroySnowball(snowball);
    }

    private void DestroySnowball(TargetProjectile snowball) {
        snowballs.Remove(snowball);
        Destroy(snowball.gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(Game.Instance.PlayerTag)) {
            if (arenaCollider.enabled) {
                arenaCollider.enabled = false;
                PlayerHud.Instance.EnableBossHealthBar();
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

        //Vector3 euler = boss3Pot.transform.rotation.eulerAngles;
        //euler.z = 12.5 * Mathf.Sin(Mathf.PI * 1 * Time.time);
        //euler.y += euler.z * -0.1f;

        //boss3Pot.transform.rotation = Quaternion.Euler(euler);

        return null;
    }
}

public class Boss3_Snowball : State {

    Boss3Pot boss3Pot = null;
    float nextSpawn;

    public override void Init(GameObject owner) {
        base.Init(owner);
        boss3Pot = owner.GetComponent<Boss3Pot>();       
    }

    public override void Enter() {
       boss3Pot.enemy.health.Resistance = DamageType.BASIC | DamageType.EXPLOSIVE | DamageType.FIRE | DamageType.ICE | DamageType.LIGHTNING | DamageType.TRUE;
        nextSpawn = 0.0f;
    }

    public override void Exit() {

    }

    public override string Update() {
        if(Time.time > nextSpawn) {
            nextSpawn = Time.time + boss3Pot.SnowballSpawnTime;
            boss3Pot.SpawnSnowball();
        }
        return null;
    }
}

public class Boss3_Vulnerable : State {

    Boss3Pot boss3Pot = null;
    float healthThreshold; 
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
// Things to Do
// Boss3_Vulnerable needs animation (Maybe add an in between state for jumping down?)
// Bos3_Snowball needs animation and logic

// Add Colliders with TargetBlock Tag
// Add Vulnerable Animation and Particle
// TEST



// Snowball needs to move around the ground (default)
// Snowball needs to hit Player (player)
// Snowball needs to hit TargetReceiver
// Snowball shoulnd not hit Snowball
// Arrow can hit Snowball 
// Arrow can hit Enemy (Enemy)
// Arrow can not hit TargetReceiver
// Arrow is PlayerProjectile
// Arrow becomes Default
// Snowball is (?)
// TargetReciever is (?)

// Snowball Speed is weird...
// Kind of hard to hit with Hammer and Spear...
// Add Wind?

