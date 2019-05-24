using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss3Pot : Pot {

    [Header("Snowball")]
    public Snowball SnowballPrefab;
    public Transform SpawnPosition;
    public BoxCollider SpawnArea;
    public TargetReceiver Reciever;

    [Header("Snowball Values")]
    public float SnowballSpawnTime = 5.0f;
    public float SnowballThrowTime = 1.0f;
    public float SnowballAcceleration = 100;
    private List<Snowball> snowballs = new List<Snowball>();

    [Header("Blocks")]
    public float BlockHeight = 5.0f;
    public float BlockMoveTime = 2.0f;
    public float BlockMoveDistance = 10.0f;
    public List<GameObject> Blocks;
    public List<Transform> BlockPositions;

    [Header("Vulnerable")]
    public float FallTime = .5f;
    public Transform VulnerablePosition;
    public ParticleSystem VulnerableParticle;

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
        //print(stateMachine.GetCurrState().ToString());
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
        Snowball snowball = Instantiate(SnowballPrefab, SpawnPosition.position, SpawnPosition.rotation, this.transform);

        snowball.Acceleration = SnowballAcceleration;
        snowball.projectile.OnHit += HitSnowball;
        snowball.projectile.OnMiss += MissSnowball;
        snowballs.Add(snowball);

        Vector3 dest = Vector3.zero;
        dest.x = SpawnArea.bounds.center.x + SpawnArea.bounds.extents.x * Random.Range(-1f, 1f);
        dest.y = SpawnArea.bounds.center.y + SpawnArea.bounds.extents.y * Random.Range(-1f, 1f);
        dest.z = SpawnArea.bounds.center.z + SpawnArea.bounds.extents.z * Random.Range(-1f, 1f);

        Vector3 peak = Utility.CreatePeak(SpawnPosition.position, dest, 20);

        StartCoroutine(ThrowSnowball(snowball, dest, peak));
    }

    private IEnumerator ThrowSnowball(Snowball snowball, Vector3 dest, Vector3 peak) {
        // 0 -> -17 -> 30
        Quaternion baseRot = this.transform.rotation;
        if (snowball == null) { // We are Falling...
            this.transform.rotation = baseRot;
            yield break;
        }

        // Windup
        Quaternion startRot = baseRot;
        Quaternion endRot = Quaternion.Euler(baseRot.eulerAngles + Vector3.right * -17);

        float startTime = Time.time;
        float length = SnowballThrowTime * .45f; // 45%
        while (Time.time < startTime + length) {
            if (snowball == null) { // We are Falling...
                this.transform.rotation = baseRot;
                yield break;
            }

            float t = (Time.time - startTime) / length;

            Quaternion rot = Quaternion.Slerp(startRot, endRot, t);

            this.transform.rotation = rot;

            yield return null;
        }
        if (snowball == null) { // We are Falling...
            this.transform.rotation = baseRot;
            yield break;
        }
        this.transform.rotation = endRot;

        // Throw
        startRot = this.transform.rotation;
        endRot = Quaternion.Euler(baseRot.eulerAngles + Vector3.right * 30);
        Vector3 startPos = snowball.transform.position;

        startTime = Time.time;
        length = SnowballThrowTime * .1f; // 10%
        while (Time.time < startTime + SnowballThrowTime) {
            if (snowball == null) { // We are Falling...
                this.transform.rotation = baseRot;
                yield break;
            }

            float t = (Time.time - startTime) / SnowballThrowTime;

            Quaternion rot = Quaternion.Slerp(startRot, endRot, t);
            Vector3 pos = Utility.BezierCurve(startPos, peak, dest, t);

            snowball.transform.position = pos;
            this.transform.rotation = rot;

            yield return null;
        }
        if (snowball == null) { // We are Falling...
            this.transform.rotation = baseRot;
            yield break;
        }
        snowball.transform.position = dest;
        this.transform.rotation = endRot;

        Vector3 dir = Player.Instance.transform.position - snowball.transform.position;
        dir.y = 0.0f;
        dir = dir.normalized;
        snowball.projectile.Fire(this.gameObject, Player.Instance.gameObject, dir);

        // Recoil
        startRot = this.transform.rotation;
        endRot = baseRot;

        startTime = Time.time;
        length = SnowballThrowTime * .5f; // 45%
        while (Time.time < startTime + length) {
            if (snowball == null) { // We are Falling...
                this.transform.rotation = baseRot;
                yield break;
            }

            float t = (Time.time - startTime) / length;

            Quaternion rot = Quaternion.Slerp(startRot, endRot, t);

            this.transform.rotation = rot;

            yield return null;
        }
        if (snowball == null) { // We are Falling...
            this.transform.rotation = baseRot;
            yield break;
        }
        this.transform.rotation = endRot;
    }

    private void ReceiveSnowball(TargetReceiver receiver, GameObject snowballObj) {
        stateMachine.ChangeState("Boss3_Stop");
        //StopAllCoroutines();

        for (int i = 0; i < snowballs.Count;) {
            Snowball snowball = snowballs[i];
            snowballs.Remove(snowball);
            Destroy(snowball.gameObject);
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

        for (int i = 1; i < Blocks.Count; i++) {
            startPos[i] = Blocks[i].transform.position;
            endPos[i] = BlockPositions[i - 1].position;
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
        Destroy(block.gameObject);

        // Pot Stun Movement
        potStartPos = this.transform.position;
        potEndPos = VulnerablePosition.position;
        Vector3 potPeakPos = Utility.CreatePeak(potStartPos, potEndPos, Blocks.Count * 3 + 4);

        startTime = Time.time;
        while (Time.time < startTime + FallTime) {
            float t = (Time.time - startTime) / FallTime;

            Vector3 potPos = Interpolation.BezierCurve(potStartPos, potPeakPos, potEndPos, t);

            this.transform.position = potPos;

            yield return null;
        }

        this.transform.position = potEndPos;

        stateMachine.ChangeState("Boss3_Vulnerable");
        VulnerableParticle.Play();
        StartCoroutine(Teeter());
    }

    public IEnumerator Jump() {
        VulnerableParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        Vector3 potStartPos = this.transform.position;
        Vector3 potEndPos = BlockPositions[Blocks.Count].position + Vector3.up * -BlockHeight / 2.0f;
        Vector3 potPeakPos = Utility.CreatePeak(potStartPos, potEndPos, Blocks.Count * 3 + 4);

        float startTime = Time.time;
        while (Time.time < startTime + FallTime) {
            float t = (Time.time - startTime) / FallTime;

            Vector3 potPos = Interpolation.BezierCurve(potStartPos, potPeakPos, potEndPos, t);

            this.transform.position = potPos;

            yield return null;
        }

        this.transform.position = potEndPos;
        stateMachine.ChangeState("Boss3_Snowball");
    }

    private void HitSnowball(TargetProjectile projectile, GameObject hitObj) {
        //print("Hit");

        Snowball snowball = projectile.GetComponent<Snowball>();

        snowballs.Remove(snowball);
        Destroy(snowball.gameObject);
    }

    private void MissSnowball(TargetProjectile projectile) {
        //print("Missed");

        Snowball snowball = projectile.GetComponent<Snowball>();

        snowballs.Remove(snowball);
        Destroy(projectile.gameObject);
    }

    private IEnumerator Teeter() {
        Vector3 angledUp = Quaternion.Euler(0, 0, 30) * Vector3.up;

        while (true) {

        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(Game.Instance.PlayerTag)) {
            if (arenaCollider.enabled) {
                arenaCollider.enabled = false;
                PlayerHud.Instance.EnableBossHealthBar();
                PlayerHud.Instance.SetBossHealthBar(enemy.health.CurrentHealth / enemy.health.MaxHealth);
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
        boss3Pot.enemy.health.Resistance = DamageType.BASIC | DamageType.EXPLOSIVE | DamageType.FIRE | DamageType.ICE | DamageType.LIGHTNING | DamageType.EARTH | DamageType.TRUE;
    }

    public override void Exit() {
    }

    public override string Update() {
        if (!boss3Pot.arenaCollider.enabled) {
            return "Boss3_Snowball";
        }

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
       boss3Pot.enemy.health.Resistance = DamageType.BASIC | DamageType.EXPLOSIVE | DamageType.FIRE | DamageType.ICE | DamageType.LIGHTNING | DamageType.EARTH | DamageType.TRUE;
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
    int phase = 1;

    public override void Init(GameObject owner) {
        base.Init(owner);
        boss3Pot = owner.GetComponent<Boss3Pot>();
        healthThreshold = boss3Pot.enemy.health.MaxHealth / boss3Pot.Blocks.Count;
        nextThreshold = boss3Pot.enemy.health.MaxHealth - healthThreshold * phase;
    }

    public override void Enter() {
        boss3Pot.enemy.health.Resistance = 0;
    }

    public override void Exit() {
        phase++;
        nextThreshold = boss3Pot.enemy.health.MaxHealth - healthThreshold * phase;
        boss3Pot.StartCoroutine(boss3Pot.Jump());

        switch (phase) {
            case 1:
                boss3Pot.SnowballSpawnTime = 5.0f;
                boss3Pot.SnowballAcceleration = 500;
                break;
            case 2:
                boss3Pot.SnowballSpawnTime = 2.5f;
                boss3Pot.SnowballAcceleration = 1000;
                break;
            case 3:
                boss3Pot.SnowballSpawnTime = 1.0f;
                boss3Pot.SnowballAcceleration = 1000;
                break;
        }

        boss3Pot.enemy.health.Resistance = DamageType.BASIC | DamageType.EXPLOSIVE | DamageType.FIRE | DamageType.ICE | DamageType.LIGHTNING | DamageType.EARTH | DamageType.TRUE;
    }

    public override string Update() {
        if (boss3Pot.enemy.health.CurrentHealth <= nextThreshold) {
            return "Boss3_Stop";
        }
        
        return null;
    }
}

///////////////////////////////////////////////////////////////////////////////
// Things to Do
// Teeter Animation
// Add Wind
// Fix player getting hit, but not destroying Snowball...



