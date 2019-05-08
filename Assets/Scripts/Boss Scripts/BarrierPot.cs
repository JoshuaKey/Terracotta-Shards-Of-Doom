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
            new EnterFormation());
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
     
            return null;
        }

        IEnumerator MoveToWaypoint()
        {
            moving = true;

            Transform transform = owner.transform;
            Vector3 waypointPosition = waypoint.transform.position;

            while ((transform.position - waypointPosition).magnitude > .01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, waypointPosition, Time.deltaTime * 2.0f);
                yield return null;
            }

            barrierPot.InPosition = true;
            barrierPot.transform.parent = waypoint.transform;
            barrierPot.enabled = false;
        }
    }


}