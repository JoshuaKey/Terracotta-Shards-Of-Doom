using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss4Pot : Pot
{
    public GameObject EnemyGameobject = null;
    public BoxCollider MovementBoundaries = null;
    public Transform Spawnpoint = null;
    [Range(1, 20)]
    public byte MaxSpawnedPots = 1;
    [HideInInspector]
    public byte NumberOfSpawnedPots = 0;

    [Range(1.0f, 10.0f)]
    public float BossSpeed = 1.0f;
    [Range(40.0f, 360.0f)]
    public float FloatSpeed = 1.0f;

    [HideInInspector]
    public byte phase = 1;

    void Start()
    {
        this.stateMachine = new StateMachine();
        stateMachine.Init(this.gameObject,
            new Boss4_Shooting());
    }

    void Update()
    {
        stateMachine.Update();
    }
}
#region Boss States
class Boss4_Shooting : State
{
    Boss4Pot boss = null;
    Player target = null;

    Coroutine floatCoroutine = null;

    byte killedPots = 0;
    bool shooting = false;

    public override void Init(GameObject owner)
    {
        boss = owner.GetComponent<Boss4Pot>();
        target = Player.Instance;

        base.Init(owner);
    }
    public override void Enter()
    {
        shooting = false;
        floatCoroutine = boss.StartCoroutine(Float());
    }

    public override void Exit()
    {
        shooting = false;
        boss.StopCoroutine(floatCoroutine);
    }

    public override string Update()
    {
        if (!shooting && boss.NumberOfSpawnedPots < boss.MaxSpawnedPots)
        {
            boss.StartCoroutine(Shooting());
        }
        return null;
    }

    IEnumerator Shooting()
    {
        shooting = true;
        boss.NumberOfSpawnedPots++;

        Enemy enemy = EnemyManager.Instance.SpawnChargerPot();
        if (enemy == null)
        {
            yield break;
        }
        enemy.health.OnDeath += PotDied;

        NavMeshAgent navAgent = null;
        Pot pot = enemy.GetComponent<Pot>();
        if (pot != null)
        {
            
            pot.enabled = false;
            if (navAgent != null)
            {
                navAgent.enabled = false;
            }
        }

        Player player = Player.Instance;
        Vector3 playerHorizontalVelocity = new Vector3(player.velocity.x, 0.0f, player.velocity.z);
        Vector3 playerPosition = player.transform.position + playerHorizontalVelocity * 1.2f;

        NavMeshHit hit;
        NavMesh.SamplePosition(playerPosition, out hit, 4.0f, NavMesh.AllAreas);
        Vector3 targetPosition = hit.position;
        
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


        do
        {
            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, targetPosition, Time.deltaTime * 10.0f);
            yield return null;
            if (enemy == null)
            {
                shooting = false;
                yield break;
            }
        } while ((enemy.transform.position - targetPosition).magnitude > .1f);

        if (navAgent != null)
        {
            navAgent.enabled = true;
        }

        if (pot != null)
        {
            pot.enabled = true;
        }

        shooting = false;
    }

    IEnumerator Float()
    {
        float distance = 0.0f;
        Vector3 targetPosition = RandomPointInBounds(boss.MovementBoundaries.bounds);
        float angle = 0.0f;
        while (true)
        {
            distance = (boss.transform.position - targetPosition).magnitude;
            if (distance <= .1f)
            {
                targetPosition = RandomPointInBounds(boss.MovementBoundaries.bounds);
            }
            //TODO: Change to target radius around player
            boss.transform.position = Vector3.MoveTowards(boss.transform.position, targetPosition, Time.deltaTime * boss.BossSpeed);

            angle += Time.deltaTime * boss.FloatSpeed;
            if (angle > 360.0f)
            {
                angle -= 360.0f;
            }

            //TODO: add shake to the floating
            float xOffset = 0.0f;
            float yOffset = Mathf.Sin(angle * Mathf.Deg2Rad) * .5f;
            float zOffset = 0.0f;

            Vector3 offsets = new Vector3(xOffset, yOffset, zOffset);
            boss.EnemyGameobject.transform.position = boss.gameObject.transform.position + offsets;
            yield return null;
        }
    }

    void PotDied()
    {
        boss.NumberOfSpawnedPots--;
        killedPots++;
    }

    public Vector3 RandomPointInBounds(Bounds bounds)
    {
        return new Vector3(Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z));
    }
}
#endregion