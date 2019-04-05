using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerPot : Pot
{
    [SerializeField] public float cowardRadius = 5;

    private void Start()
    {
        stateMachine = new StateMachine();
        stateMachine.Init(gameObject,
            new Runner_Idle(),
            new Runner_Run());
    }

    private void Update()
    {
        stateMachine.Update();
        if (agent.desiredVelocity.magnitude > 0)
        {
            Waddle();
        }
    }
}

public class Runner_Idle : State
{
    GameObject player;

    public override void Enter()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    public override void Exit()
    { }

    public override string Update()
    {
        if (Vector3.Distance(owner.transform.position, player.transform.position) < owner.GetComponent<RunnerPot>().cowardRadius)
        {
            return "Runner_Run";
        }

        return null;
    }
}
public class Runner_Run : State
{
    GameObject player;

    public override void Enter()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    public override void Exit()
    { }

    public override string Update()
    {
        if (Vector3.Distance(owner.transform.position, player.transform.position) > owner.GetComponent<RunnerPot>().cowardRadius)
        {
            return "Runner_Idle";
        }

        agent.SetDestination(owner.transform.position * 2 - player.transform.position);

        return null;
    }
}