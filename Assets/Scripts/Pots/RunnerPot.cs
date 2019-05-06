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
    //if player is within aggro radius and visible
    //or pot is damaged
    //move to runner_run

    RunnerPot runnerPot;
    Health health;
    bool isDamaged;

    public override void Init(GameObject owner)
    {
        base.Init(owner);
        runnerPot = owner.GetComponent<RunnerPot>();
        health = owner.GetComponent<Health>();
    }

    public override void Enter()
    {
        isDamaged = false;
        health.OnDamage += OnDamage;
    }

    public override void Exit()
    {
        health.OnDamage -= OnDamage;
    }

    public override string Update()
    {
        Vector3 towardPlayer = Player.Instance.transform.position - owner.transform.position;

        RaycastHit hit;
        if ((Physics.Raycast(owner.transform.position, towardPlayer, out hit, runnerPot.cowardRadius, ~LayerMask.GetMask("Enemy"))
            && hit.collider.tag == "Player")
            || isDamaged)
        {
            return "Runner_Run";
        }

        return null;
    }

    public void OnDamage(float damage)
    {
        isDamaged = true;
    }
}

public class Runner_Run : State
{
    //run away from player

    RunnerPot runnerPot;

    public override void Init(GameObject owner)
    {
        base.Init(owner);
        runnerPot = owner.GetComponent<RunnerPot>();
    }

    public override void Enter() { }

    public override void Exit() { }

    public override string Update()
    {
        Debug.DrawLine(owner.transform.position, Player.Instance.transform.position, Color.red);
        Debug.DrawLine(owner.transform.position, agent.destination, Color.cyan);

        if (!runnerPot.stunned) {
          Vector3 awayFromPlayer = owner.transform.position * 2 - Player.Instance.transform.position;
          agent.SetDestination(awayFromPlayer);
        }
        
        return null;
    }
}
