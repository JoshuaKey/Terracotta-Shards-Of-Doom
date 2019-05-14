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
            new BarrierPot_EnterFormation());

        ChargerPotStateMachine = new StateMachine();
        ChargerPotStateMachine.Init(gameObject,
            new Charger_Idle(),
            new Charger_Charge(),
            new Charger_Attack(attackDuration));
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
        transform.parent = EnemyManager.Instance.transform;
    }

}
class BarrierPot_EnterFormation : State
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