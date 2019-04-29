using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Boss2AI : Pot
{
    NavMeshAgent navMeshAgent = null;

    Waypoint currentWaypoint = null;

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

    [SerializeField]
    GameObject Barrier = null;

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

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        barrierWaypoints = Barrier.GetComponentsInChildren<Waypoint>();
        barrierPots = new List<BarrierPot>(8);
        stateMachine = new StateMachine();
        stateMachine.Init(this.gameObject,
            new Moving(),
            new Animating(),
            new DoNothing());
    }

    void Update()
    {
        stateMachine.Update();
    }

    #region Boss States

    private class Running : State
    {

        public override void Enter()
        {
        }

        public override void Exit()
        {
        }

        public override string Update()
        {
            return null;
        }
    }

    private class Moving : State
    {
        NavMeshAgent navMeshAgent = null;
        List<GameObject> waypoints = null;
        GameObject target = null;
        Boss2AI boss2AI = null;
        //The state needs any monobehaviour to start coroutines
        MonoBehaviour monoBehaviour = null;

        bool running = false;
        bool reachedDestination = false;
        public override void Enter()
        {
            running = false;
            if (navMeshAgent == null)
            {
                navMeshAgent = owner.GetComponent<NavMeshAgent>();
            }

            if (waypoints == null)
            {
                waypoints = new List<GameObject>(GameObject.FindGameObjectsWithTag("Waypoint"));
            }

            if (boss2AI == null)
            {
                boss2AI = owner.GetComponent<Boss2AI>();
            }

            if (monoBehaviour == null)
            {
                monoBehaviour = owner.GetComponent<MonoBehaviour>();
            }

            if (waypoints.Where(w => w.GetComponent<Waypoint>().Visited).Count() == waypoints.Count)
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
            if (!running && !reachedDestination)
            {
                monoBehaviour.StartCoroutine(Run());
            }
            else if (reachedDestination)
            {
                return "Boss2AI+Animating";
            }
            return null;
        }

        IEnumerator Run()
        {
            running = true;
            navMeshAgent.SetDestination(target.transform.position);
            while ((navMeshAgent.destination - owner.transform.position).magnitude > .1f)
            {
                yield return null;
            }
            target.GetComponent<Waypoint>().Visited = true;
            running = false;
            reachedDestination = true;
        }
    }

    private class Animating : State
    {
        Boss2AI boss2AI = null;
        List<Pot> Pots = null;

        private bool animating = false;
        private bool doneAnimating = false;

        Vector3 offset = new Vector3(0.0f, .5f, 0.0f);

        Vector3 halfExtents = new Vector3(10.0f, .5f, 10.5f);

        public override void Enter()
        {
            if (boss2AI == null)
            {
                boss2AI = owner.GetComponent<Boss2AI>();
            }

            animating = false;
            doneAnimating = false;

            Pots = new List<Pot>();
            Collider[] colliders = Physics.OverlapBox(owner.transform.position + offset, halfExtents);
            Pot p;
            foreach (Collider c in colliders)
            {
                if ((p = c.GetComponent<Pot>()) && (c.gameObject != owner))
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
            if (!animating)
            {
                boss2AI.StartCoroutine(AnimatePots());
            }
            else if (doneAnimating)
            {
                return "Boss2AI+DoNothing";
            }
            return null;
        }

        IEnumerator AnimatePots()
        {
            animating = true;
            BarrierPot bp = null;
            foreach (Pot p in Pots)
            {
                if (!p.enabled)
                {

                    p.enabled = true;
                    bp = p as BarrierPot;
                    if (bp != null)
                    {
                        boss2AI.BarrierPots.Add(bp);
                        Waypoint w = FindBestEmptyBarrierWaypoint(bp.transform.position);
                        if (w != null)
                        {
                            w.Visited = true;
                            bp.Waypoint = w;
                            bp.GetStateMachine().ChangeState("BarrierPot+EnterFormation");
                        }
                    }
                }
            }
            while(boss2AI.BarrierPots.Exists(p => !p.InPosition))
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

            foreach (Waypoint w in boss2AI.BarrierWaypoints)
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
            if (waypoint == null)
            {
                Debug.Log("All waypoints are occupied or something went wrong");

            }
            return waypoint;
        }
    }

    public class DoNothing : State
    {
        public override void Enter()
        {
        }

        public override void Exit()
        {
        }

        public override string Update()
        {
            return null;
        }
    }
    #endregion
}
