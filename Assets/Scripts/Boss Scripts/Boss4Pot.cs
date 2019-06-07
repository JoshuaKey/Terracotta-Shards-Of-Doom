using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using System.Collections.Generic;

public class Boss4Pot : Pot
{
    [Header("Values")]
    public Enemy enemy;
    public Collider arenaCollider;

    [Header("Boss Stuff")]
    public GameObject MeshAndCollider = null;
    public BoxCollider MovementBoundaries = null;
    public Transform Spawnpoint = null;
    public GameObject ProjectilePool = null;
    public BoxCollider ProjectileSpawnArea = null;
    public GameObject Waypoint = null;
    public GameObject ShieldObject = null;

    [Header("Phase 2")]
    public TargetProjectile TrackingMeteorPrefab;
    public Transform ProjectileSpawnpoint = null;
    public TargetReceiver Reciever;
    public List<GameObject> Shields;
    public GameObject Aura;
    public float ProjectileImpulse = 10.0f;
    public TargetProjectile currProjectile;
    public float SpawnDelay = 1.0f;

    [Header("Phase")]
    public int Phase1Pots = 50;
    public int ReceiveCount = 0;

    [Range(1, 20)]
    public byte MaxSpawnedPots = 1;
    [HideInInspector]
    public byte NumberOfSpawnedPots = 0;

    [Range(1.0f, 10.0f)]
    public float BossSpeed = 1.0f;
    [Range(40.0f, 360.0f)]
    public float FloatSpeed = 1.0f;
    private bool IsFloating = false;
    [HideInInspector]
    public byte phase = 1;

    void Start()
    {
        this.stateMachine = new StateMachine();
        stateMachine.needAgent = false;
        stateMachine.Init(this.gameObject,
            new Boss4_Idle(),
            new Boss4_Shooting(),
            new Boss4_TargetMove(),
            new Boss4_Target(),
            new Boss4_MeteorShower());

        StartCoroutine(RotateShields());
        enemy.health.OnDamage += ChangeHealthUI;
        enemy.health.OnDamage += PlayClang;
        enemy.health.OnDeath += OnDeath;
        Reciever.OnReceive += ReceiveTarget;
    }

    void Update()
    {
        stateMachine.Update();
    }

    public void PlayClang(float damage)
    {
        if (!health.IsDead())
        {
            AudioManager.Instance.PlaySoundWithParent("clang", ESoundChannel.SFX, gameObject);
        }
    }

    private void ChangeHealthUI(float val)
    {
        PlayerHud.Instance.SetBossHealthBar(enemy.health.CurrentHealth / enemy.health.MaxHealth);
    }

    private void OnDeath()
    {
        StopFloating();
        PlayerHud.Instance.DisableBossHealthBar();
    }

    public void SpawnProjectile()
    {
        if (currProjectile != null)
        {
            return;
        }
        TargetProjectile meteor = Instantiate(TrackingMeteorPrefab,
            ProjectileSpawnpoint.position, ProjectileSpawnpoint.rotation, this.transform);

        Vector3 dir = Vector3.up;
        dir *= ProjectileImpulse;

        meteor.OnHit += HitTarget;
        meteor.OnMiss += MissTarget;
        currProjectile = meteor;

        ReceiveCount = 3 - Shields.Count;

        switch (Shields.Count)
        {
            case 3:
                meteor.SpeedOverTime = .2f;
                break;
            case 2:
                meteor.SpeedOverTime = .05f;
                break;
            case 1:
                meteor.SpeedOverTime = .03f;
                break;
            default:
                meteor.SpeedOverTime = .2f;
                break;
        }

        meteor.Fire(this.gameObject, Player.Instance.camera.gameObject, dir);
    }

    private void ReceiveTarget(TargetReceiver receiver, GameObject meteor)
    {
        if (currProjectile == null) { return; }
        print("Received " + meteor.name);

        if (ReceiveCount == 0)
        {
            currProjectile.OnHit -= HitTarget;
            currProjectile.OnMiss -= MissTarget;
            currProjectile = null;

            GameObject shield = Shields[0];
            Shields.RemoveAt(0);
            Destroy(shield.gameObject);
            Destroy(meteor.gameObject);

            if (Shields.Count == 0)
            {
                Aura.SetActive(false);
                stateMachine.ChangeState("Boss4_MeteorShower");
            }
            else
            {
                StartCoroutine(SpawnProjectileDelay());
            }
        }
        else
        {
            ReceiveCount--;

            Vector3 dir = Vector3.up;
            dir *= ProjectileImpulse;

            //currProjectile.SpeedOverTime -= .01f;

            currProjectile.Hit(this.gameObject, dir);
        }
    }

    private void HitTarget(TargetProjectile projectile, GameObject hitObj)
    {
        if (currProjectile != null)
        {
            print("Hit");
            Destroy(projectile.gameObject);
            currProjectile = null;
            StartCoroutine(SpawnProjectileDelay());
        }
    }

    private void MissTarget(TargetProjectile projectile)
    {
        if (currProjectile != null)
        {
            print("Missed");
            Destroy(projectile.gameObject);
            currProjectile = null;
            StartCoroutine(SpawnProjectileDelay());
        }
    }

    private IEnumerator SpawnProjectileDelay()
    {
        yield return new WaitForSeconds(SpawnDelay);

        stateMachine.ChangeState("Boss4_Target");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Game.Instance.PlayerTag))
        {
            if (arenaCollider.enabled)
            {
                arenaCollider.enabled = false;
                PlayerHud.Instance.EnableBossHealthBar();
                PlayerHud.Instance.SetBossHealthBar(enemy.health.CurrentHealth / enemy.health.MaxHealth, true);
            }
        }
    }

    float angle = 0.0f;
    Coroutine floatCoroutine;

    public void StartFloating()
    {
        if (!IsFloating)
        {
            floatCoroutine = StartCoroutine(Float());
        }
    }

    public void StopFloating()
    {
        if (floatCoroutine != null)
        {
            IsFloating = false;
            StopCoroutine(floatCoroutine);
        }
    }
    IEnumerator Float()
    {
        IsFloating = true;
        while (true)
        {

            angle += Time.deltaTime * this.FloatSpeed;
            angle %= 360.0f;

            //TODO: add shake to the floating
            float xOffset = 0.0f;
            float yOffset = Mathf.Sin(angle * Mathf.Deg2Rad) * .5f;
            float zOffset = 0.0f;

            Vector3 offsets = new Vector3(xOffset, yOffset, zOffset);
            MeshAndCollider.transform.position = this.gameObject.transform.position + offsets;
            yield return null;
        }
    }

    float rotationAngle = 0.0f;
    IEnumerator RotateShields()
    {
        while (true)
        {
            rotationAngle += Time.deltaTime * -30.0f;
            rotationAngle %= 360.0f;
            ShieldObject.transform.rotation = Quaternion.Euler(0.0f, rotationAngle, 0.0f);
            yield return null;
        }
    }
}


#region Boss States
public class Boss4_Idle : State
{

    Boss4Pot boss = null;
    bool teleporting = false;
    bool inPosition = false;

    public override void Init(GameObject owner)
    {
        base.Init(owner);
        boss = owner.GetComponent<Boss4Pot>();
    }

    public override void Enter()
    {
        boss.enemy.health.Resistance = DamageType.BASIC | DamageType.EXPLOSIVE | DamageType.FIRE | DamageType.ICE | DamageType.LIGHTNING | DamageType.EARTH | DamageType.TRUE;
        boss.StartFloating();
    }

    public override void Exit()
    {
        boss.StartFloating();
        inPosition = false;
        teleporting = false;
    }

    public override string Update()
    {
        if (!boss.arenaCollider.enabled)
        {
            if (inPosition)
            {
                return "Boss4_Shooting";
            }
            if (!teleporting)
            {
                boss.StartCoroutine(Teleport());
            }
        }

        return null;
    }

    IEnumerator Teleport()
    {
        boss.StopFloating();
        teleporting = true;

        Transform target = boss.Waypoint.transform.GetChild(0);

        Vector3 teleportLocation = Vector3.zero;
        Vector2 randomDirection2D = Random.insideUnitCircle;

        randomDirection2D.y = Mathf.Abs(randomDirection2D.y);

        Player player = Player.Instance;

        teleportLocation = new Vector3(randomDirection2D.x, 0.0f, randomDirection2D.y) * 15.0f;
        teleportLocation.y = boss.transform.position.y;


        Vector3 originalScale = boss.transform.localScale;
        float shrinkTime = 0.0f;

        while (boss.transform.localScale.sqrMagnitude != 0)
        {
            shrinkTime += Time.deltaTime;
            shrinkTime = Mathf.Clamp(shrinkTime, 0.0f, 1.0f);
            boss.transform.localScale = originalScale - (originalScale * (shrinkTime / 1.0f));
            yield return null;
        }

        boss.Waypoint.transform.position = new Vector3(player.transform.position.x, 0.0f, player.transform.position.z);

        target.localPosition = teleportLocation;
        boss.transform.position = target.position;

        boss.transform.localScale = Vector3.zero;
        float growthTime = 0.0f;

        while (boss.transform.localScale.sqrMagnitude < originalScale.sqrMagnitude)
        {
            growthTime += Time.deltaTime;
            growthTime = Mathf.Clamp(growthTime, 0.0f, 1.0f);
            boss.transform.localScale = originalScale * (growthTime / 1.0f);
            boss.Waypoint.transform.position = new Vector3(player.transform.position.x, 0.0f, player.transform.position.z);
            boss.transform.position = target.position;
            Debug.DrawLine(player.transform.position, target.position, Color.cyan);
            yield return null;
        }
        boss.StartFloating();
        teleporting = false;
        inPosition = true;
    }

}

class Boss4_Shooting : State
{
    Boss4Pot boss = null;
    Coroutine movingCoroutine;
    Coroutine spawningCoroutine;
    byte killedPots = 0;
    float nextSpawnTime = 0;

    public override void Init(GameObject owner)
    {
        boss = owner.GetComponent<Boss4Pot>();
        base.Init(owner);
    }

    public override void Enter()
    {
        boss.StartFloating();
        movingCoroutine = boss.StartCoroutine(Moving());
        spawningCoroutine = boss.StartCoroutine(Spawning());
        boss.enemy.health.Resistance = DamageType.BASIC | DamageType.EXPLOSIVE | DamageType.FIRE | DamageType.ICE | DamageType.LIGHTNING | DamageType.EARTH | DamageType.TRUE;
    }

    public override void Exit()
    {
        boss.StopFloating();
        boss.StopCoroutine(movingCoroutine);
        boss.StopCoroutine(spawningCoroutine);
    }

    public override string Update()
    {
        if (killedPots >= boss.Phase1Pots)
        {
            if (boss.NumberOfSpawnedPots == 0)
            {
                return "Boss4_TargetMove";
            }
        }
        else
        {
            if (Time.time >= nextSpawnTime && boss.NumberOfSpawnedPots < boss.MaxSpawnedPots && killedPots < boss.Phase1Pots)
            {
                boss.StartCoroutine(Shooting());
            }
        }

        return null;
    }

    IEnumerator Shooting()
    {
        nextSpawnTime = Time.time + 2f;

        Enemy enemy = EnemyManager.Instance.SpawnChargerPot();
        if (enemy == null)
        {
            yield break;
        }
        boss.NumberOfSpawnedPots++;
        enemy.health.OnDeath += PotDied;

        Pot pot = enemy.GetComponent<Pot>();
        if (pot != null)
        {

            pot.enabled = false;
            if (pot.agent != null)
            {

                pot.agent.enabled = false;
            }
        }

        Vector2 randomDirection = Random.insideUnitCircle;
        Vector3 direction = new Vector3(randomDirection.x, 0.0f, randomDirection.y);
        Vector3 spawnpointPosition = boss.transform.position + (direction * 2.0f);

        enemy.transform.position = spawnpointPosition;
        boss.Spawnpoint.position = spawnpointPosition;
        enemy.transform.parent = boss.Spawnpoint;

        Vector3 originalScale = enemy.transform.localScale;
        enemy.transform.localScale = Vector3.zero;
        float growthTime = 0.0f;

        while (enemy.transform.localScale.sqrMagnitude < originalScale.sqrMagnitude)
        {
            growthTime += Time.deltaTime;
            growthTime = Mathf.Clamp(growthTime, 0.0f, 1.5f);
            enemy.transform.localScale = originalScale * (growthTime / 1.5f);
            yield return null;
        }
        enemy.transform.parent = null;

        Player player = Player.Instance;
        Vector3 playerHorizontalVelocity = new Vector3(player.velocity.x, 0.0f, player.velocity.z);
        Vector3 playerPosition = player.transform.position + playerHorizontalVelocity;

        Vector3 targetPosition = playerPosition;

        // Hit is false...
        NavMeshHit hit;
        NavMesh.SamplePosition(playerPosition, out hit, 8.0f, NavMesh.AllAreas);

        if (NavMesh.SamplePosition(targetPosition, out hit, 25.0f, NavMesh.AllAreas))
        {
            targetPosition = hit.position;
        }

        do
        {
            Vector3 pos = Vector3.MoveTowards(enemy.transform.position, targetPosition, Time.deltaTime * 12.0f);
            enemy.transform.position = pos;
            yield return null;
            if (enemy == null)
            {
                yield break;
            }
        } while ((enemy.transform.position - targetPosition).magnitude > .1f);

        if (pot != null)
        {
            pot.enabled = true;
            if (pot.agent != null)
            {
                pot.agent.enabled = true;
            }
        }
    }

    IEnumerator Spawning()
    {
        Enemy enemy = null;
        Vector3 targetPosition = Vector3.zero;
        Vector2 randomDirection2D = Vector2.zero;
        Vector3 randomDirection3D = Vector3.zero;
        Vector3 playerPosition = Vector3.zero;
        Vector3 playerPositionHorizontal = Vector3.zero;
        Vector3 lookDirection = Vector3.zero;
        Quaternion lookRotation = Quaternion.identity;
        NavMeshHit hit;

        while (true)
        {
            if (boss.NumberOfSpawnedPots < boss.MaxSpawnedPots && killedPots <= boss.Phase1Pots)
            {
                playerPosition = Player.Instance.transform.position;
                playerPositionHorizontal = new Vector3(playerPosition.x, 0.0f, playerPosition.z);

                enemy = EnemyManager.Instance.SpawnArmorPot();
                enemy.GetComponent<Pot>().enabled = false;
                boss.NumberOfSpawnedPots++;
                enemy.health.OnDeath += PotDied;
                do
                {
                    randomDirection2D = Random.insideUnitCircle;
                    randomDirection3D = new Vector3(randomDirection2D.x, 0.0f, randomDirection2D.y);
                    targetPosition = playerPosition + randomDirection3D * 7;
                } while (!NavMesh.SamplePosition(targetPosition, out hit, 10.0f, NavMesh.AllAreas));
                targetPosition = hit.position;
                enemy.transform.position = hit.position;

                lookRotation = Quaternion.LookRotation((playerPositionHorizontal - targetPosition).normalized, Vector3.up);
                enemy.transform.rotation = lookRotation;

                boss.StartCoroutine(ShrinkAndGrow(enemy.gameObject));

                enemy = EnemyManager.Instance.SpawnArmorPot();
                boss.NumberOfSpawnedPots++;
                enemy.health.OnDeath += PotDied;
                do {
                    randomDirection3D = Quaternion.Euler(0.0f, 120.0f + Random.Range(0.0f, 30.0f), 0.0f) * randomDirection3D;
                    targetPosition = playerPosition + randomDirection3D * 7;
                } while (!NavMesh.SamplePosition(targetPosition, out hit, 10.0f, NavMesh.AllAreas));

                targetPosition = hit.position;
                enemy.transform.position = targetPosition;

                lookRotation = Quaternion.LookRotation((playerPositionHorizontal - targetPosition).normalized, Vector3.up);
                enemy.transform.rotation = lookRotation;

                boss.StartCoroutine(ShrinkAndGrow(enemy.gameObject));

                //boss.NumberOfSpawnedPots += 2;
                yield return new WaitForSeconds(15.0f);
            }
            else
            {
                yield return null;
            }
        }
    }

    IEnumerator ShrinkAndGrow(GameObject obj)
    {
        Pot pot = obj.GetComponent<Pot>();
        pot.enabled = false;
        Vector3 originalScale = obj.transform.localScale;

        obj.transform.localScale = Vector3.zero;

        float growthTime = 0.0f;

        while (obj.transform.localScale.sqrMagnitude < originalScale.sqrMagnitude)
        {
            growthTime += Time.deltaTime;
            growthTime = Mathf.Clamp(growthTime, 0.0f, 1.0f);
            obj.transform.localScale = originalScale * (growthTime / 1.0f);
            yield return null;
        }
        pot.enabled = true;

    }

    IEnumerator Moving()
    {
        float distance = 0.0f;
        float distanceFromPlayer = 0.0f;
        float yPosition = boss.transform.position.y;

        Player player = Player.Instance;
        Transform target = boss.Waypoint.transform.GetChild(0);
        Vector3 waypointPosition = new Vector3(player.transform.position.x, 0.0f, player.transform.position.z);
        boss.Waypoint.transform.position = waypointPosition;

        Vector3 playerPositionHorizontal = Vector3.zero;
        Vector3 directionToPlayer = Vector3.zero;

        Vector2 randomDirection2D = Random.insideUnitCircle;
        Vector3 randomDirection3D = new Vector3(randomDirection2D.x, 0.0f, randomDirection2D.y);

        // Player should always see the boss...
        //Vector3 randomDirection3D = player.camera.transform.forward;
        //randomDirection3D.y = 0.0f;
        //randomDirection3D = randomDirection3D.normalized;

        Vector3 targetPosition = randomDirection3D * 15.0f;//Random.Range(10.0f, 12.0f);
        targetPosition.y = yPosition;

        target.localPosition = targetPosition;

        while (true)
        {
            waypointPosition.x = player.transform.position.x;
            waypointPosition.z = player.transform.position.z;
            boss.Waypoint.transform.position = waypointPosition;

            distance = (boss.transform.position - target.transform.position).magnitude;

            if (distance <= .1f)
            {
                randomDirection2D = Random.insideUnitCircle;
                randomDirection3D = new Vector3(randomDirection2D.x, 0.0f, randomDirection2D.y);

                //randomDirection3D = player.camera.transform.forward;
                //randomDirection3D.y = 0.0f;
                //randomDirection3D = randomDirection3D.normalized;

                targetPosition = randomDirection3D * 15.0f;//Random.Range(10.0f, 12.0f);
                targetPosition.y = yPosition;
                target.localPosition = targetPosition;
            }

            playerPositionHorizontal.x = player.transform.position.x;
            playerPositionHorizontal.y = yPosition;
            playerPositionHorizontal.z = player.transform.position.z;

            directionToPlayer = (playerPositionHorizontal - boss.transform.position);
            distanceFromPlayer = directionToPlayer.magnitude;

            if (distanceFromPlayer > 15.5f)
            {
                boss.transform.position += (directionToPlayer.normalized * (distanceFromPlayer - 10));
            }

            boss.transform.position = Vector3.MoveTowards(boss.transform.position, target.position, Time.deltaTime * boss.BossSpeed);
            Quaternion rotation = Quaternion.identity;
            rotation.SetLookRotation(directionToPlayer.normalized);
            boss.transform.rotation = rotation;
            yield return null;
        }
    }

    void PotDied()
    {
        boss.NumberOfSpawnedPots--;
        killedPots++;
    }
}

public class Boss4_TargetMove : State {

    Boss4Pot boss4Pot = null;
    bool moving = false;

    public override void Init(GameObject owner) {
        base.Init(owner);
        boss4Pot = owner.GetComponent<Boss4Pot>();
    }

    public override void Enter() {
        boss4Pot.enemy.health.Resistance = DamageType.BASIC | DamageType.EXPLOSIVE | DamageType.FIRE | DamageType.ICE | DamageType.LIGHTNING | DamageType.EARTH | DamageType.TRUE;
        boss4Pot.StartFloating();

        boss4Pot.StartCoroutine(Move());
    }

    public override void Exit() {
        boss4Pot.StopFloating();
    }

    public override string Update() {
        if (!moving) {
           return "Boss4_Target"; 
        }
        return null;
    }

    private IEnumerator Move() {
        moving = true;
        var MovementBoundaries = boss4Pot.MovementBoundaries;

        Vector2 dir = Random.insideUnitCircle;
        Vector3 targetPos = MovementBoundaries.transform.position + MovementBoundaries.center + (MovementBoundaries.size.x * new Vector3(dir.x, 0.0f, dir.y));

        do {
            Vector3 pos = Vector3.MoveTowards(boss4Pot.transform.position, targetPos, Time.deltaTime * 3.0f);
            boss4Pot.transform.position = pos;
            yield return null;
        } while ((boss4Pot.transform.position - targetPos).sqrMagnitude > .1f);

        yield return new WaitForSeconds(2.0f);

        moving = false;
    }
}

public class Boss4_Target : State
{

    Boss4Pot boss4Pot = null;

    public override void Init(GameObject owner)
    {
        base.Init(owner);
        boss4Pot = owner.GetComponent<Boss4Pot>();
    }

    public override void Enter()
    {
        boss4Pot.enemy.health.Resistance = DamageType.BASIC | DamageType.EXPLOSIVE | DamageType.FIRE | DamageType.ICE | DamageType.LIGHTNING | DamageType.EARTH | DamageType.TRUE;
        boss4Pot.SpawnProjectile();
        boss4Pot.StartFloating();
    }

    public override void Exit()
    {
        boss4Pot.StopFloating();
    }

    public override string Update()
    {

        return null;
    }
}

class Boss4_MeteorShower : State
{
    Boss4Pot boss = null;
    Meteor[] projectiles = null;

    bool firing = false;
    bool teleporting = false;

    public override void Init(GameObject owner)
    {
        boss = owner.GetComponent<Boss4Pot>();
        projectiles = new Meteor[boss.ProjectilePool.transform.childCount];
        for (int i = 0; i < projectiles.Length; i++)
        {
            projectiles[i] = boss.ProjectilePool.transform.GetChild(i).GetComponent<Meteor>();
        }
        base.Init(owner);
    }

    public override void Enter()
    {
        firing = false;
        teleporting = false;
        boss.enemy.health.Resistance = 0;
    }

    public override void Exit()
    {
        firing = false;
        teleporting = false;

    }

    public override string Update()
    {
        if (!firing)
        {
            boss.StartCoroutine(Firing());
        }
        return null;
    }

    IEnumerator Firing()
    {

        firing = true;

        Bounds spawnArea = boss.ProjectileSpawnArea.bounds;
        Vector3 playerPosition = Player.Instance.transform.position;
        RaycastHit hit;
        Vector3 spawnPosition = new Vector3(playerPosition.x, Random.Range(spawnArea.min.y, spawnArea.max.y), playerPosition.z);

        //The first projectile is set to the players position, so they are forced to move
        projectiles[0].transform.position = spawnPosition;
        Physics.Raycast(spawnPosition, Vector3.down, out hit, float.MaxValue, LayerMask.GetMask("Default"));
        projectiles[0].TargetPosition = hit.point;
        projectiles[0].StartFall();

        bool tooClose = false;
        for (int i = 1; i < projectiles.Length; i++)
        {
            //Loop while we find a position for each meteor to hit that is not too close to each other
            do
            {
                spawnPosition = Utility.RandomPointInBounds(spawnArea);
                Physics.Raycast(spawnPosition, Vector3.down, out hit, float.MaxValue, LayerMask.GetMask("Default"));

                tooClose = false;
                //Iterate through each projectile to see if it's to close to the others set before it
                for (int j = i - 1; (j > -1) && !tooClose; j--)
                {
                    tooClose = ((hit.point - projectiles[j].TargetPosition).magnitude < 4.0f);
                }

            } while (tooClose);

            projectiles[i].TargetPosition = hit.point;
            projectiles[i].transform.position = spawnPosition;
            projectiles[i].StartFall();
        }
        boss.StartCoroutine(Teleport());

        //Wait for each projectile to land before we exit the coroutine and start again
        while (projectiles.Any(m => !m.Landed))
        {
            yield return null;
        }
        firing = false;
    }
    IEnumerator Teleport()
    {
        boss.StopFloating();
        teleporting = true;
        Vector3 teleportLocation = Vector3.zero;
        bool tooClose = false;
        do
        {
            teleportLocation = Utility.RandomPointInBounds(boss.MovementBoundaries.bounds);
            tooClose = false;
            for (int i = 0; i < projectiles.Length && !tooClose; i++)
            {
                //tooClose = (projectiles[i].transform.position - teleportLocation).sqrMagnitude < 16.0f;
                tooClose = (projectiles[i].transform.position - teleportLocation).sqrMagnitude < 4.0f;
            }
        } while (tooClose);
        Vector3 originalScale = boss.transform.localScale;
        float shrinkTime = 0.0f;

        while (boss.transform.localScale.sqrMagnitude != 0)
        {
            shrinkTime += Time.deltaTime;
            shrinkTime = Mathf.Clamp(shrinkTime, 0.0f, 1.0f);
            boss.transform.localScale = originalScale - (originalScale * (shrinkTime / 1.0f));
            yield return null;
        }
        boss.transform.position = teleportLocation;

        boss.transform.localScale = Vector3.zero;
        float growthTime = 0.0f;

        while (boss.transform.localScale.sqrMagnitude < originalScale.sqrMagnitude)
        {
            growthTime += Time.deltaTime;
            growthTime = Mathf.Clamp(growthTime, 0.0f, 1.0f);
            boss.transform.localScale = originalScale * (growthTime / 1.0f);
            yield return null;
        }
        boss.StartFloating();
        teleporting = false;
    }
}
#endregion