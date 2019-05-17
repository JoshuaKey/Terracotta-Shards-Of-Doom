using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Boss2Pot : Pot
{
    [SerializeField] public float knockback = 50f;

    [HideInInspector]
    public Enemy enemy;
    NavMeshAgent navMeshAgent = null;

    [HideInInspector]
    public Waypoint currentWaypoint = null;

    public GameObject Barrier = null;

    Waypoint[] barrierWaypoints = null;

    public Waypoint[] BarrierWaypoints
    {
        get
        {
            return barrierWaypoints;
        }
    }

    public Arena[] Arenas;

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

    void Start()
    {
        if (enemy == null) { enemy = GetComponentInChildren<Enemy>(true); }
        navMeshAgent = GetComponent<NavMeshAgent>();

        barrierWaypoints = Barrier.GetComponentsInChildren<Waypoint>();
        barrierPots = new List<BarrierPot>(8);
        stateMachine = new StateMachine();
        stateMachine.Init(this.gameObject,
            new Boss2Pot_ChangingRooms(),
            new Boss2Pot_Animating(),
            new Boss2Pot_Running());

        enemy.health.OnDamage += ChangeHealthUI;
        enemy.health.OnDamage += PlayClang;
        enemy.health.OnDeath += OnDeath;
        ChangeHealthUI(0);
    }

    void Update()
    {
        stateMachine.Update();
    }

    public void ChangeHealthUI(float damage) 
    {
        PlayerHud.Instance.SetBossHealthBar(enemy.health.CurrentHealth / enemy.health.MaxHealth);
    }

    public void OnDeath() 
    {
        PlayerHud.Instance.DisableBossHealthBar();
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
            if(bp != null)
            {
                bp.ChangeStateMachine();
            }
        }
        barrierPots.Clear();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(Game.Instance.PlayerTag)) {
            Player.Instance.health.TakeDamage(DamageType.BASIC, 1);
            Vector3 dir = Player.Instance.transform.position - this.transform.position;
            dir.y = 0.0f;
            dir = dir.normalized;
            Player.Instance.Knockback(dir * knockback);
        }
    }
}
#region Boss States

public class Boss2Pot_ChangingRooms : State
{
    List<Waypoint> waypoints = null;
    GameObject target = null;
    Boss2Pot boss = null;

    bool running = false;
    bool reachedDestination = false;

    public override void Init(GameObject owner)
    {
        if(waypoints == null)
        {
            waypoints = new List<Waypoint>(
                GameObject.FindGameObjectsWithTag("Waypoint").Select(x => x.GetComponent<Waypoint>()));
        }

        if(boss == null)
        {
            boss = owner.GetComponent<Boss2Pot>();
        }
        base.Init(owner);
    }

    public override void Enter()
    {
        boss.agent.enabled = true;

        running = false;
        reachedDestination = false;

        List<Waypoint> possibleWaypoints = null;
        possibleWaypoints = waypoints.Where(w => !w.Visited).ToList();

        int randomIndex = Random.Range(0, possibleWaypoints.Count - 1);
        target = possibleWaypoints[randomIndex].gameObject;
        boss.currentWaypoint = possibleWaypoints[randomIndex];
        base.Init(owner);
    }

    public override void Exit()
    {
        running = false;
        reachedDestination = false;
        boss.agent.enabled = false;

    }

    public override string Update()
    {
        if(!running && !reachedDestination)
        {
            boss.StartCoroutine(Run());
        }
        else if(reachedDestination)
        {
            Debug.Log("Reached Destination");
            //boss.currentWaypoint.arena.gameObject.SetActive(true);
            return "Boss2Pot_Animating";
        }

        return null;
    }

    IEnumerator Run()
    {
        running = true;
        boss.agent.SetDestination(target.transform.position);
        while ((boss.agent.destination - owner.transform.position).magnitude > .1f)
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
            if((p = c.GetComponentInParent<Pot>()) && (c.gameObject != owner))
            {
                Pots.Add(p);
            }
        }
    }

    public override void Exit()
    {
        Pots = null;
        animating = false;
        doneAnimating = false;
    }

    public override string Update()
    {
        if(!animating)
        {
            boss.StartCoroutine(AnimatePots());
        }
        else if(doneAnimating)
        {
            if(healthComponent.CurrentHealth - ((6 - boss.Phase) * healthComponent.MaxHealth / 6) <= 0.0f)
            {
                Debug.Log("Reseting _Animating");
                boss.ConvertBarrierPots();
                //boss.currentWaypoint.arena.walls.ForEach(X => X.Open());
                //boss.currentWaypoint.arena.gameObject.SetActive(false);
                return "Boss2Pot_ChangingRooms";
            }
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
            if(!p.enabled)
            {           
                p.animator.SetTrigger("Awake");

                bp = p as BarrierPot;
                if(bp != null)
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

        foreach(BarrierPot barrierPot in boss.barrierPots)
        {
            Waypoint w = FindBestEmptyBarrierWaypoint(barrierPot.transform.position);
            if (w != null)
            {

                w.Visited = true;
                barrierPot.Waypoint = w;
                barrierPot.GetStateMachine().ChangeState("BarrierPot_EnterFormation");
            }
        }

        //Found problem. If a barrier pot dies during the animation phase, it is null and will never enter position.
        //This causes this check to always fail.
        //TODO: ^

        while (boss.barrierPots.Exists(p => !p.InPosition))
        {
            yield return null;
        }

        foreach(Pot p in Pots)
        {
            if(p != null && !p.enabled)
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

        foreach (Waypoint w in boss.BarrierWaypoints)
        {
            if(!w.Visited)
            {
                float distanceSquared = (position - w.gameObject.transform.position).sqrMagnitude;
                if(distanceSquared < shortestDistanceSquared)
                {
                    shortestDistanceSquared = distanceSquared;
                    waypoint = w;
                }
            }
        }
        if(waypoint == null)
        {
            Debug.Log("All waypoints are occupied or something went wrong");

        }
        return waypoint;
    }
}

public class Boss2Pot_Running : State
{
    Boss2Pot boss = null;
    Health healthComponent = null;

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
        previousDirection = Vector3.zero;
        moving = false;
        bounds = new Bounds(boss.currentWaypoint.transform.position, new Vector3(20.0f, 0.0f, 20.0f));
    }

    public override void Exit()
    {
        moving = false;
        boss.Phase++;
    }

    public override string Update()
    {
        if(healthComponent.CurrentHealth - ((6 - boss.Phase) * healthComponent.MaxHealth / 6) <= 0.0f)
        {
            boss.ConvertBarrierPots();
            //boss.currentWaypoint.arena.walls.ForEach(X => X.Open());
            //boss.currentWaypoint.arena.gameObject.SetActive(false);
            return "Boss2Pot_ChangingRooms";
        }
        else if(!moving)
        {
            boss.StartCoroutine(Run());
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
            targetPosition = RandomPointInBounds(bounds);
        } while (!(Vector3.Angle(previousDirection, targetPosition.normalized) > 30.0f) && targetPosition.magnitude < 3.0f);

        previousDirection = targetPosition.normalized;

        while ((owner.transform.position - targetPosition).magnitude > .25f)
        {
            owner.transform.position = Vector3.MoveTowards(owner.transform.position, targetPosition, Time.deltaTime * boss.speed);
            yield return null;
        }
        moving = false;
    }

    Vector3 RandomPointInBounds(Bounds bounds)
    {
        Vector3 result = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z));
        return result;
    }
}
#endregion
