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

    [Range(0.0f, 20.0f)]
    [SerializeField]
    float PanickedTimer = 0.0f;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        stateMachine = new StateMachine();
        stateMachine.Init(this.gameObject,
            new Boss2AI_Running(),
            new Boss2AI_Panicked(PanickedTimer),
            new Boss2AI_Animating());
    }

    void Update()
    {
        stateMachine.Update();
    }

    #region Boss States
    private class Boss2AI_Panicked : TimedState
    {
        public Boss2AI_Panicked(float seconds) : base(seconds)
        {

        }
        
        public override void Enter()
        {
            base.Enter();
        }

        public override string Update()
        {
            if (timer >= seconds)
            {
                //Exit to some state
            }
            //Call this last if the condition to exit was not met
            return base.Update();
        }

        public override void Exit()
        {

        }
    }

    private class Boss2AI_Running : State
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
            if(navMeshAgent == null)
            {
                navMeshAgent = owner.GetComponent<NavMeshAgent>();
            }

            if(waypoints == null)
            {
                waypoints = new List<GameObject>(GameObject.FindGameObjectsWithTag("Waypoint"));
            }

            if(boss2AI == null)
            {
                boss2AI = owner.GetComponent<Boss2AI>();
            }

            if(monoBehaviour == null)
            {
                monoBehaviour = owner.GetComponent<MonoBehaviour>();
            }

            if(waypoints.Where(w => w.GetComponent<Waypoint>().Visited).Count() == waypoints.Count)
            {
                foreach(GameObject waypoint in waypoints)
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
                monoBehaviour.StartCoroutine(Run());
            }
            else if (reachedDestination)
            {
                return "";
            }
            return null;
        }
        IEnumerator Run()
        {
            running = true;
            navMeshAgent.SetDestination(target.transform.position);
            while((navMeshAgent.destination - owner.transform.position).magnitude > .1f)
            {
                yield return null;
            }
            target.GetComponent<Waypoint>().Visited = true;
            running = false;
            reachedDestination = true;
        }
    }
    
    private class Boss2AI_Animating : State
    {
        List<Pot> Pots = null;
        public override void Enter()
        {
            if(Pots == null)
            {
                Pots = owner.GetComponent<Boss2AI>().currentWaypoint.pots;
            }
        }

        public override void Exit()
        {
            Pots = null;
        }

        public override string Update()
        {

            return null;
        }

        IEnumerator AnimatePots()
        {
            foreach(Pot p in Pots)
            {
                p.enabled = true;
            }
            return null;
        }
    }
    #endregion
}
