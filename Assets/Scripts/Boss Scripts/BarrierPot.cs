using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BarrierPot : Pot
{
    private Waypoint waypoint;

    public Waypoint Waypoint
    {
        get { return waypoint; }
        set { waypoint = value; }
    }

    private bool inPosition;

    public bool InPosition
    {
        get
        {
            return inPosition;
        }
        set
        {
            inPosition = value;
        }
    }


    private void OnEnable()
    {
        stateMachine = new StateMachine();
        stateMachine.Init(this.gameObject,
     new EnterFormation(),
     new FollowWaypoint());
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
    }

    class EnterFormation : State
    {
        BarrierPot barrierPot = null;
        Waypoint waypoint = null;
        bool moving = false;

        public override void Enter()
        {
            moving = false;
            if (barrierPot == null)
            {
                barrierPot = owner.GetComponent<BarrierPot>();
            }
            if (waypoint == null && barrierPot != null)
            {
                waypoint = barrierPot.Waypoint;
            }
        }

        public override void Exit()
        {
            moving = false;
        }

        public override string Update()
        {
            if (!moving)
            {
                barrierPot.StartCoroutine(MoveToWaypoint());
            }
            else if (barrierPot.InPosition)
            {
                return "BarrierPot+FollowWaypoint";
            }
            return null;
        }

        IEnumerator MoveToWaypoint()
        {
            moving = true;

            NavMeshAgent navMeshAgent = barrierPot.agent;
            navMeshAgent.speed = 15.0f;
            navMeshAgent.SetDestination(waypoint.transform.position);

            while ((navMeshAgent.destination - owner.transform.position).magnitude > .1f)
            {
                yield return null;
            }

            barrierPot.InPosition = true;
        }
    }

    public class FollowWaypoint : State
    {
        BarrierPot barrierPot = null;
        NavMeshAgent navMeshAgent = null;

        public override void Enter()
        {
            if(barrierPot == null)
            {
                barrierPot = owner.GetComponent<BarrierPot>();
            }

            if(navMeshAgent == null && barrierPot != null)
            {
                navMeshAgent = barrierPot.agent;
            }
        }

        public override void Exit()
        {

        }

        public override string Update()
        {
            owner.transform.position = barrierPot.Waypoint.transform.position;
            return null;
        }
    }

}