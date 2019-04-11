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

    public override void Animate()
    {
        Waddle();
    }
}

public class Runner_Idle : State
{
    GameObject player;

    //if player is null it sets player
    public override void Enter()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    public override void Exit()
    { }

    //if the distance to player is less than cowardRadius, start running away
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

    //if player is null it sets player
    public override void Enter()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    public override void Exit()
    { }

    //if distance to player is greater than cowardRadius, stop running away
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