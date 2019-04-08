using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerPot : Pot
{
    [SerializeField] public float aggroRadius = 5;

    private void Start()
    {
        stateMachine = new StateMachine();
        stateMachine.Init(gameObject, 
            new Charger_Idle(), 
            new Charger_Charge());
    }

    //Calls hop when Animate is called. This looks bad but it's the most efficient way to do it
    public override void Animate()
    {
        Hop();
    }
}

public class Charger_Idle : State
{
    GameObject player;

    //if player is null it sets player
    public override void Enter()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    public override void Exit()
    { }

    //if the distance to player is less than aggroRadius, start running at player
    public override string Update()
    {
        if(Vector3.Distance(owner.transform.position, player.transform.position) < owner.GetComponent<ChargerPot>().aggroRadius)
        {
            return "Charger_Charge";
        }

        return null;
    }
}

public class Charger_Charge : State
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

    //if the distance to player is greater than aggroRadius, stop running at player
    public override string Update()
    {
        if (Vector3.Distance(owner.transform.position, player.transform.position) > owner.GetComponent<ChargerPot>().aggroRadius)
        {
            return "Charger_Idle";
        }

        agent.SetDestination(player.transform.position);

        return null;
    }
}

