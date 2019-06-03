using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using Luminosity.IO;

public class Boss2Pot : Pot
{
    [SerializeField] public float knockback = 50f;

    [HideInInspector]
    public Enemy enemy;
    NavMeshAgent navMeshAgent = null;
    public new Collider collider = null;

    [HideInInspector]
    public Waypoint currentWaypoint = null;

    public GameObject Barrier = null;

    [HideInInspector]
    public Waypoint[] barrierWaypoints = null;

    [HideInInspector]
    public List<BarrierPot> barrierPots = null;

    private byte phase = 1;

    public byte Phase
    {
        get { return phase; }
        set { phase = value; }
    }

    public float speed = 10.0f;

    public GameObject ParticlePrefab = null;

    public BoxCollider startTrigger;

    void Start()
    {
        if (enemy == null) { enemy = GetComponentInChildren<Enemy>(true); }
        if (collider == null) { collider = GetComponentInChildren<Collider>(true); }
        navMeshAgent = GetComponent<NavMeshAgent>();

        barrierWaypoints = Barrier.GetComponentsInChildren<Waypoint>();
        barrierPots = new List<BarrierPot>(8);
        stateMachine = new StateMachine();
        stateMachine.Init(this.gameObject,
            new Boss2Pot_Idle(),
            new Boss2Pot_ChangingRooms(),
            new Boss2Pot_Animating(),
            new Boss2Pot_Running());

        stateMachine.needAgent = false;

        enemy.health.OnDamage += ChangeHealthUI;
        enemy.health.OnDamage += PlayClang;
        enemy.health.OnDeath += OnDeath;
    }

    void Update()
    {
        stateMachine.Update();
        if(InputManager.GetKeyDown(KeyCode.I))
        {
            health.TakeDamage(DamageType.TRUE, 1.0f);
        }
    }

    public void ChangeHealthUI(float damage)
    {
        PlayerHud.Instance.SetBossHealthBar(enemy.health.CurrentHealth / enemy.health.MaxHealth);
    }

    public void OnDeath()
    {
        StopAllCoroutines();
        Destroy(gameObject.GetComponent<Rigidbody>());
        PlayerHud.Instance.DisableBossHealthBar();
    
        foreach (Wall w in currentWaypoint.arena.walls)
        {
            w.Open();
        }
        currentWaypoint.arena.gameObject.SetActive(false);

    }


    public void PlayClang(float damage)
    {
        if (!health.IsDead())
        {
            AudioManager.Instance.PlaySoundWithParent("clang", ESoundChannel.SFX, gameObject);
        }
    }

    public void ConvertBarrierPots()
    {
        for (int i = 0; i < barrierWaypoints.Length; i++)
        {
            barrierWaypoints[i].Visited = false;
        }

        foreach (BarrierPot bp in barrierPots)
        {
            if (bp != null)
            {
                bp.ChangeStateMachine();
            }
        }
        barrierPots.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Game.Instance.PlayerTag))
        {
            if (startTrigger.enabled)
            {
                startTrigger.enabled = false;
                PlayerHud.Instance.SetBossHealthBar(enemy.health.CurrentHealth / enemy.health.MaxHealth, true);
            }
            else
            {
                Player.Instance.health.TakeDamage(DamageType.BASIC, 3);
                Vector3 dir = Player.Instance.transform.position - this.transform.position;
                dir.y = 0.0f;
                dir = dir.normalized;
                Player.Instance.Knockback(dir * knockback);
                // Attack
            }
        }
    }
}
#region Boss States

public class Boss2Pot_Idle : State
{

    Boss2Pot boss = null;

    public override void Init(GameObject owner)
    {
        base.Init(owner);
        boss = owner.GetComponent<Boss2Pot>();
    }

    public override void Enter()
    {
        boss.enemy.health.Resistance = DamageType.BASIC;
    }

    public override void Exit()
    {

    }

    public override string Update()
    {
        if (!boss.startTrigger.enabled)
        {
            return "Boss2Pot_ChangingRooms";
        }

        return null;
    }
}

public class Boss2Pot_ChangingRooms : State
{
    List<Waypoint> waypoints = null;
    GameObject target = null;
    Boss2Pot boss = null;

    bool running = false;
    bool reachedDestination = false;

    public override void Init(GameObject owner)
    {
        if (waypoints == null)
        {
            waypoints = new List<Waypoint>(GameObject.FindGameObjectsWithTag("Waypoint").Select(x => x.GetComponent<Waypoint>()));
        }

        if (boss == null)
        {
            boss = owner.GetComponent<Boss2Pot>();
        }
        base.Init(owner);
    }

    public override void Enter()
    {

        if (boss.currentWaypoint != null)
        {
            boss.currentWaypoint.arena.gameObject.SetActive(false);
        }

        running = false;
        reachedDestination = false;

        List<Waypoint> possibleWaypoints = null;
        possibleWaypoints = waypoints.Where(w => !w.Visited).ToList();

        int randomIndex = Random.Range(0, possibleWaypoints.Count - 1);
        target = possibleWaypoints[randomIndex].gameObject;
        boss.currentWaypoint = possibleWaypoints[randomIndex];

        boss.enemy.health.Resistance = DamageType.BASIC;
    }

    public override void Exit()
    {
        running = false;
        reachedDestination = false;

        boss.enemy.health.Resistance = 0;
    }

    public override string Update()
    {
        if (!running && !reachedDestination)
        {
            boss.StartCoroutine(Run());
        }
        else if (reachedDestination)
        {
            boss.currentWaypoint.arena.gameObject.SetActive(true);
            foreach (Wall w in boss.currentWaypoint.arena.walls)
            {
                w.gameObject.SetActive(true);
            }
            return "Boss2Pot_Animating";
        }

        return null;
    }

    IEnumerator Run()
    {
        running = true;

        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(boss.transform.position, target.transform.position, NavMesh.AllAreas, path);
   
        boss.agent.SetDestination(target.transform.position);

        while ((boss.transform.position - boss.agent.destination).magnitude > boss.agent.stoppingDistance)
        {
            yield return null;
        }

        target.GetComponent<Waypoint>().Visited = true;
        running = false;
        reachedDestination = true;
    }
}

public class Boss2Pot_Animating : State
{
    Boss2Pot boss = null;
    Health healthComponent = null;
    List<Pot> Pots = null;

    private bool animating = false;
    private bool doneAnimating = false;

    Vector3 offset = new Vector3(0.0f, .5f, 0.0f);
    Vector3 halfExtents = new Vector3(12.0f, 0.0f, 12.0f);

    public override void Init(GameObject owner)
    {
        boss = owner.GetComponent<Boss2Pot>();
        healthComponent = owner.GetComponent<Health>();
        base.Init(owner);
    }

    public override void Enter()
    {
        animating = false;
        doneAnimating = false;
        Pots = new List<Pot>();
        Collider[] colliders = Physics.OverlapBox(owner.transform.position + offset, halfExtents);

        Pot p;
        foreach (Collider c in colliders)
        {
            if ((p = c.GetComponentInParent<Pot>()) && (c.gameObject != owner))
            {
                Pots.Add(p);
            }
        }

        boss.enemy.health.Resistance = DamageType.BASIC;
    }

    public override void Exit()
    {
        Pots = null;
        animating = false;
        doneAnimating = false;

        boss.enemy.health.Resistance = 0;
    }

    public override string Update()
    {

        if (!animating && boss.currentWaypoint.arena.playerEnteredArena)
        {
            boss.StartCoroutine(AnimatePots());
        }
        else if (doneAnimating)
        {
            return "Boss2Pot_Running";
        }
        return null;
    }

    IEnumerator AnimatePots()
    {
        animating = true;
        boss.animator.SetTrigger("Animate_Pots");
        BarrierPot bp = null;
        foreach (Pot p in Pots)
        {
            if (p == null) { continue; }

            if (!p.enabled)
            {
                p.animator.SetTrigger("Awake");

                bp = p as BarrierPot;
                if (bp != null)
                {
                    p.enabled = true;
                    bp.owningBoss = boss;
                    boss.barrierPots.Add(bp);
                }
            }
        }

        while (boss.animator.GetCurrentAnimatorStateInfo(0).IsName("Boss2_Idle"))
        {
            yield return null;
        }

        GameObject particleSystem = GameObject.Instantiate(boss.ParticlePrefab, boss.transform.position, boss.ParticlePrefab.transform.rotation);

        while (boss.animator.GetCurrentAnimatorStateInfo(0).IsName("Boss2_Animate_Pots"))
        {
            yield return null;
        }

        foreach (BarrierPot barrierPot in boss.barrierPots)
        {
            Waypoint w = FindBestEmptyBarrierWaypoint(barrierPot.transform.position);
            if (w != null)
            {
                w.Visited = true;
                barrierPot.Waypoint = w;
                barrierPot.GetStateMachine().ChangeState("BarrierPot_EnterFormation");
            }
        }

        while (boss.barrierPots.Exists(p => !p.InPosition))
        {
            yield return null;
        }

        foreach (Pot p in Pots)
        {
            if (p != null && !p.enabled)
            {
                p.enabled = true;
            }
        }

        yield return new WaitForSeconds(.2f);
        doneAnimating = true;
    }

    public Waypoint FindBestEmptyBarrierWaypoint(Vector3 position)
    {

        Waypoint waypoint = null;
        float shortestDistanceSquared = float.MaxValue;

        foreach (Waypoint w in boss.barrierWaypoints)
        {
            if (!w.Visited)
            {
                float distanceSquared = (position - w.gameObject.transform.position).sqrMagnitude;
                if (distanceSquared < shortestDistanceSquared)
                {
                    shortestDistanceSquared = distanceSquared;
                    waypoint = w;
                }
            }
        }
        return waypoint;
    }
}

public class Boss2Pot_Running : State
{
    Boss2Pot boss = null;
    Health healthComponent = null;
    Coroutine movingCoroutine = null;

    Bounds bounds;
    Vector3 previousDirection;
    bool moving = false;

    public override void Init(GameObject owner)
    {
        boss = owner.GetComponent<Boss2Pot>();
        healthComponent = owner.GetComponent<Health>();
        base.Init(owner);
    }

    public override void Enter()
    {
        boss.agent.enabled = false;
        previousDirection = Vector3.zero;
        moving = false;
        bounds = new Bounds(boss.currentWaypoint.transform.position, new Vector3(20.0f, 0.0f, 20.0f));
    }

    public override void Exit()
    {
        boss.agent.enabled = true;
        foreach (Wall w in boss.currentWaypoint.arena.walls)
        {
            w.Reset();
        }
        boss.currentWaypoint.arena.gameObject.SetActive(false);
        moving = false;
        boss.StopCoroutine(movingCoroutine);
        boss.Phase++;
    }

    public override string Update()
    {

        if (healthComponent.CurrentHealth - ((6 - boss.Phase) * healthComponent.MaxHealth / 6) <= 0.0f)
        {
            boss.ConvertBarrierPots();
            return "Boss2Pot_ChangingRooms";
        }
        else if (!moving)
        {
            movingCoroutine = boss.StartCoroutine(Run());
        }
        boss.Barrier.transform.Rotate(0.0f, 30.0f * Time.deltaTime * boss.Phase, 0.0f);

        return null;
    }

    IEnumerator Run()
    {
        moving = true;
        Vector3 targetPosition;
        do
        {
            targetPosition = Utility.RandomPointInBounds(bounds);
        } while (!(Vector3.Angle(previousDirection, targetPosition.normalized) > 30.0f) && targetPosition.magnitude < 3.0f);

        previousDirection = targetPosition.normalized;

        while ((owner.transform.position - targetPosition).magnitude > .25f)
        {
            Debug.DrawLine(boss.transform.position, targetPosition, Color.red);
            owner.transform.position = Vector3.MoveTowards(owner.transform.position, targetPosition, Time.deltaTime * boss.speed);
            yield return null;
        }
        moving = false;
    }

}
#endregion
