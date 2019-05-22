using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss3Pot : Pot {

    [Header("Snowball")]
    public TargetProjectile SnowballPrefab;
    public Transform SpawnPosition;
    public BoxCollider SpawnArea;
    public TargetReceiver Reciever;

    [Header("Snowball Values")]
    public float SnowballSpawnTime = 5.0f;
    public float SnowballThrowTime = 1.0f;
    public float SnowballImpulse = 10.0f;
    private List<TargetProjectile> snowballs = new List<TargetProjectile>();

    [Header("Blocks")]
    public float BlockHeight = 5.0f;
    public float BlockMoveTime = 2.0f;
    public float BlockMoveDistance = 10.0f;
    public List<GameObject> Blocks;
    public List<Transform> BlockPositions;

    [Header("Vulnerable")]
    public float FallTime = .5f;
    public Transform VulnerablePosition;

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
            new Boss3_Stop(),
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

    public void SpawnSnowball() {
        TargetProjectile snowball = Instantiate(SnowballPrefab, SpawnPosition.position, SpawnPosition.rotation);

        snowball.OnHit += HitSnowball;
        snowball.OnMiss += MissSnowball;
        snowballs.Add(snowball);

        Vector3 dest = Vector3.zero;
        dest.x = SpawnArea.bounds.center.x + SpawnArea.bounds.extents.x * Random.Range(-1, 1);
        dest.y = SpawnArea.bounds.center.y + SpawnArea.bounds.extents.y * Random.Range(-1, 1);
        dest.z = SpawnArea.bounds.center.z + SpawnArea.bounds.extents.z * Random.Range(-1, 1);

        Vector3 peak = Utility.CreatePeak(SpawnPosition.position, dest, 15);

        StartCoroutine(ThrowSnowball(snowball, dest, peak));
    }

    private IEnumerator ThrowSnowball(TargetProjectile snowball, Vector3 dest, Vector3 peak) {
        Vector3 startPos = snowball.transform.position;
        float startTime = Time.time;
        while (Time.time < startTime + SnowballThrowTime) {
            float t = (Time.time - startTime) / SnowballThrowTime;

            Vector3 pos = Utility.BezierCurve(startPos, peak, dest, t);

            snowball.transform.position = pos;

            yield return null;
        }
        snowball.transform.position = dest;

        Vector3 dir = Player.Instance.transform.position - snowball.transform.position;
        dir.y = 0.0f;
        dir = dir.normalized;
        snowball.Fire(this.gameObject, Player.Instance.gameObject, dir);
    }

    private void ReceiveSnowball(TargetReceiver receiver, GameObject snowballObj) {
        print("Received " + snowballObj.name);

        stateMachine.ChangeState("Boss3_Stop");
        //

        for (int i = 0; i < snowballs.Count;) {
            TargetProjectile snowball = snowballs[i];
            DestroySnowball(snowball);
        }

        StartCoroutine(FallDown());
    }

    private IEnumerator FallDown() {
        // Variables
        Vector3[] startPos = new Vector3[Blocks.Count];
        Vector3[] endPos = new Vector3[Blocks.Count];

        Transform block = Blocks[0].transform;
        startPos[0] = block.position;
        endPos[0] = block.position + block.right * BlockMoveDistance;
        print("Start " + startPos[0]);
        print("End " + endPos[0]);

        for (int i = 1; i < Blocks.Count; i++) {
            startPos[i] = Blocks[i].transform.position;
            endPos[i] = BlockPositions[i - 1].position;
            print("Start " + startPos[i]);
            print("End " + endPos[i]);
        }

        Vector3 potStartPos = this.transform.position;
        Vector3 potEndPos = BlockPositions[Blocks.Count - 1].position + Vector3.up * -BlockHeight / 2.0f;

        // Move Pots and Block to new Destination
        float startTime = Time.time;
        while (Time.time < startTime + BlockMoveTime) {
            float t = (Time.time - startTime) / BlockMoveTime;

            for (int i = 0; i < Blocks.Count; i++) {
                Vector3 pos = Vector3.Lerp(startPos[i], endPos[i], t);
                Blocks[i].transform.position = pos;
            }

            Vector3 potPos = Vector3.Lerp(potStartPos, potEndPos, t);

            this.transform.position = potPos;

            yield return null;
        }

        for (int i = 1; i < Blocks.Count; i++) {
            Blocks[i].transform.position = endPos[i];
        }
        this.transform.position = potEndPos;
        Blocks.RemoveAt(0);

        // Pot Stun Movement
        potStartPos = this.transform.position;
        potEndPos = VulnerablePosition.position;
        Vector3 potPeakPos = Utility.CreatePeak(potStartPos, potEndPos, Blocks.Count * 3 + 2);

        startTime = Time.time;
        while (Time.time < startTime + FallTime) {
            float t = (Time.time - startTime) / FallTime;

            Vector3 potPos = Interpolation.BezierCurve(potStartPos, potEndPos, potPeakPos, t);

            this.transform.position = potPos;

            yield return null;
        }

        this.transform.position = potEndPos;

        stateMachine.ChangeState("Boss3_Vulnerable");
    }

    public IEnumerator Jump() {
        Vector3 potStartPos = this.transform.position;
        Vector3 potEndPos = BlockPositions[Blocks.Count - 1].position + Vector3.up * -BlockHeight / 2.0f;
        Vector3 potPeakPos = Utility.CreatePeak(potStartPos, potEndPos, Blocks.Count * 3 + 2);

        float startTime = Time.time;
        while (Time.time < startTime + FallTime) {
            float t = (Time.time - startTime) / FallTime;

            Vector3 potPos = Interpolation.BezierCurve(potStartPos, potEndPos, potPeakPos, t);

            this.transform.position = potPos;

            yield return null;
        }

        this.transform.position = potEndPos;
        stateMachine.ChangeState("Boss3_Snowball");
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

public class Boss3_Stop : State {

    public override void Init(GameObject owner) {
        base.Init(owner);
    }

    public override void Enter() { }

    public override void Exit() {}

    public override string Update() { return null; }
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
        boss3Pot.StartCoroutine(boss3Pot.Jump());
    }

    public override string Update() {
        if (boss3Pot.enemy.health.CurrentHealth <= nextThreshold) {
            return "Boss3_Stop";
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

