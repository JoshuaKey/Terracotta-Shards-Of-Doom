using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Boss2Pot : Pot
{
    NavMeshAgent navMeshAgent = null;

    [HideInInspector]
    public Waypoint currentWaypoint = null;

    public Waypoint CurrentWaypoint
    {
        get
        {
            return currentWaypoint;
        }
        set
        {
            currentWaypoint = value;
        }
    }

    public GameObject Barrier = null;

    Waypoint[] barrierWaypoints = null;

    public Waypoint[] BarrierWaypoints
    {
        get
        {
            return barrierWaypoints;
        }
    }

    private List<BarrierPot> barrierPots = null;

    public List<BarrierPot> BarrierPots
    {
        get
        {
            return barrierPots;
        }
    }

    private byte phase = 1;

    public byte Phase
    {
        get { return phase; }
        set { phase = value; }
    }


    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        barrierWaypoints = Barrier.GetComponentsInChildren<Waypoint>();
        barrierPots = new List<BarrierPot>(8);
        stateMachine = new StateMachine();
        stateMachine.Init(this.gameObject,
            new Boss2Pot_ChangingRooms(),
            new Boss2Pot_Animating(),
            new Boss2Pot_Running());
    }

    void Update()
    {
        stateMachine.Update();
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

}
#region Boss States

public class Boss2Pot_ChangingRooms : State
{
    List<GameObject> waypoints = null;
    GameObject target = null;
    Boss2Pot boss2AI = null;

    bool running = false;
    bool reachedDestination = false;

    public override void Init(GameObject owner)
    {
        if(waypoints == null)
        {
            waypoints = new List<GameObject>(GameObject.FindGameObjectsWithTag("Waypoint"));
        }

        if(boss2AI == null)
        {
            boss2AI = owner.GetComponent<Boss2Pot>();
        }
        base.Init(owner);
    }

    public override void Enter()
    {
        running = false;
        reachedDestination = false;
        //If every waypoint has been visited, reset them  all
        if(waypoints.Where(w => w.GetComponent<Waypoint>().Visited).Count() == waypoints.Count)
        {
            foreach (GameObject waypoint in waypoints)
            {
                waypoint.GetComponent<Waypoint>().Visited = false;
            }
        }

        List<GameObject> possibleWaypoints = null;
        possibleWaypoints = waypoints.Where(w => !w.GetComponent<Waypoint>().Visited).ToList();

        int randomIndex = Random.Range(0, possibleWaypoints.Count - 1);
        target = possibleWaypoints[randomIndex];
        boss2AI.currentWaypoint = target.GetComponent<Waypoint>();

    }

    public override void Exit()
    {
        running = false;
        reachedDestination = false;
    }

    public override string Update()
    {
        if(!running && !reachedDestination)
        {
            boss2AI.StartCoroutine(Run());
        }
        else if(reachedDestination)
        {
            return "Boss2Pot_Animating";
        }

        return null;
    }

    IEnumerator Run()
    {
        running = true;
        boss2AI.agent.SetDestination(target.transform.position);
        while ((boss2AI.agent.destination - owner.transform.position).magnitude > .1f)
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

    Vector3 halfExtents = new Vector3(12.0f, .5f, 12.0f);

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
            if((p = c.GetComponent<Pot>()) && (c.gameObject != owner))
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
                boss.ConvertBarrierPots();
                return "Boss2Pot_ChangingRooms";
            }
            return "Boss2Pot_Running";

        }
        return null;
    }

    IEnumerator AnimatePots()
    {
        animating = true;
        BarrierPot bp = null;
        foreach (Pot p in Pots)
        {
            if(!p.enabled)
            {

                p.enabled = true;
                bp = p as BarrierPot;
                if(bp != null)
                {
                    boss.BarrierPots.Add(bp);
                    Waypoint w = FindBestEmptyBarrierWaypoint(bp.transform.position);
                    if(w != null)
                    {
                        w.Visited = true;
                        bp.Waypoint = w;
                        bp.GetStateMachine().ChangeState("BarrierPot_EnterFormation");
                        boss.BarrierPots.Add(bp);
                    }
                }
            }
        }
        while (boss.BarrierPots.Exists(p => !p.InPosition))
        {
            yield return null;
        }
        doneAnimating = true;
        yield break;
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
            owner.transform.position = Vector3.MoveTowards(owner.transform.position, targetPosition, Time.deltaTime * 5.0f);
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