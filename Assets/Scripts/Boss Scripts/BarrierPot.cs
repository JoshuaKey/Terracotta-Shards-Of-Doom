using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BarrierPot : ChargerPot
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

    private StateMachine ChargerPotStateMachine = null;

    private void OnEnable()
    {
        stateMachine = new StateMachine();
        stateMachine.Init(this.gameObject,
            new BarrierPot_EnterFormation(),
            new BarrierPot_DoNothing());

        ChargerPotStateMachine = new StateMachine();
        ChargerPotStateMachine.Init(gameObject,
            new Charger_Idle(),
            new Charger_Charge(),
            new Charger_Attack(attackDuration));

        agent.enabled = false;
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
    }

    public void ChangeStateMachine()
    {
        stateMachine = ChargerPotStateMachine;
        agent.enabled = true;
        transform.parent = EnemyManager.Instance.transform;
    }

}
public class BarrierPot_EnterFormation : State
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

        if (!moving && !barrierPot.InPosition)
        {
            barrierPot.StartCoroutine(MoveToWaypoint());
        }
        else if (barrierPot.InPosition)
        {
            return "BarrierPot_DoNothing";
        }
        return null;
    }

    IEnumerator MoveToWaypoint()
    {
        if (waypoint != null)
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
        }
    }
}

public class BarrierPot_DoNothing : State
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