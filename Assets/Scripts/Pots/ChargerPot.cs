using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerPot : Pot
{
    [SerializeField] public float aggroRadius = 5f;
    [SerializeField] public float attackRadius = 3f;
    [SerializeField] public float attackDuration = 0.25f;
    [SerializeField] public float attackAngle = 70f;

    private void Start()
    {
        stateMachine = new StateMachine();
        stateMachine.Init(gameObject, 
            new Charger_Idle(), 
            new Charger_Charge(),
            new Charger_Attack(attackDuration));
        stateMachine.DEBUGGING = true;
    }

    //Calls hop when Animate is called. This looks bad but it's the most efficient way to do it
    public override void Animate()
    {
        Hop();

        if(gameObject.name == "Charger Pot Variant")
        {
            Debug.Log(Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position));
        }
    }
}

public class Charger_Idle : State
{
    GameObject player;

    public override void Init(GameObject owner)
    {
        base.Init(owner);
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public override void Enter()
    { }

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

    public override void Init(GameObject owner)
    {
        base.Init(owner);
        player = GameObject.FindGameObjectWithTag("Player");
    }

    //if player is null it sets player
    public override void Enter()
    { }

    public override void Exit()
    { }

    //if the distance to player is greater than aggroRadius, stop running at player
    public override string Update()
    {
        float distance = Vector3.Distance(owner.transform.position, player.transform.position);
        ChargerPot cp = owner.GetComponent<ChargerPot>();

        if (distance > cp.aggroRadius)
        {
            return "Charger_Idle";
        }

        if (distance < cp.attackRadius)
        {
            return "PUSH.Charger_Attack";
        }

        if (agent.isActiveAndEnabled) {
            agent.SetDestination(player.transform.position);
        }

        return null;
    }
}

public class Charger_Attack : TimedState
{
    GameObject player;

    public Charger_Attack(float seconds) : base(seconds)
    { }

    public override void Init(GameObject owner)
    {
        base.Init(owner);
        player = GameObject.FindGameObjectWithTag("Player");
    }

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
        base.Update();

        Vector3 newRotation = owner.transform.rotation.eulerAngles;

        ChargerPot cp = owner.GetComponent<ChargerPot>();

        newRotation.x = timer * (timer - cp.attackDuration) * (-4 * cp.attackAngle) / Mathf.Pow(cp.attackDuration, 2);
        newRotation.y = Quaternion.LookRotation(player.transform.position - owner.transform.position).eulerAngles.y;

        owner.transform.rotation = Quaternion.Euler(newRotation);

        if(timer >= cp.attackDuration)
        {
            return "POP";
        }

        return null;
    }
}