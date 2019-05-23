using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Boss4Pot : Pot
{
    [Header("Boss Stuff")]
    public GameObject MeshAndCollider = null;
    public BoxCollider MovementBoundaries = null;
    public Transform Spawnpoint = null;
    public GameObject ProjectilePool = null;
    public BoxCollider ProjectileTargetArea = null;
    public BoxCollider ProjectileSpawnArea = null;

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
            new Boss_MeteorShower());
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

        NavMeshHit hit;
        NavMesh.SamplePosition(playerPosition, out hit, 4.0f, NavMesh.AllAreas);
        Vector3 targetPosition = hit.position;


        do
        {
            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, targetPosition, Time.deltaTime * 12.0f);
            yield return null;
            if (enemy == null)
            {
                shooting = false;
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

        shooting = false;
    }

    IEnumerator Float()
    {
        float distance = 0.0f;
        Vector3 targetPosition = Utility.RandomPointInBounds(boss.MovementBoundaries.bounds);
        float angle = 0.0f;
        while (true)
        {
            distance = (boss.transform.position - targetPosition).magnitude;
            if (distance <= .1f)
            {
                targetPosition = Utility.RandomPointInBounds(boss.MovementBoundaries.bounds);
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
            boss.MeshAndCollider.transform.position = boss.gameObject.transform.position + offsets;
            yield return null;
        }
    }

    void PotDied()
    {
        boss.NumberOfSpawnedPots--;
        killedPots++;
    }
}

class Boss_MeteorShower : State
{
    Boss4Pot boss = null;
    Meteor[] projectiles = null;

    bool firing = false;

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
    }

    public override void Exit()
    {
        firing = false;
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

        //Wait for each projectile to land before we exit the coroutine and start again
        while (projectiles.Any(m => !m.Landed))
        {
            yield return null;
        }
        firing = false;
    }
}
#endregion