using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerPot : Pot
{
    [SerializeField] public float aggroRadius = 5f;
    [SerializeField] public float attackRadius = 2.5f;
    [SerializeField] public float attackDuration = 0.25f;

    //delete this later
    public static bool debugged = false;
    //delete this later

    protected virtual void Start()
    {
        stateMachine = new StateMachine();
        stateMachine.Init(gameObject,
            new Charger_Idle(),
            new Charger_Charge(),
            new Charger_Attack(attackDuration));
        //stateMachine.DEBUGGING = true;
    }

    //Calls hop when Animate is called. This looks bad but it's the most efficient way to do it
    public override void Animate()
    {
        Hop();

        if(gameObject.name == "Charger Pot Variant" && stateMachine.DEBUGGING)
        {
            Debug.Log(Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position));
        }
    }
}

public class Charger_Idle : State
{
    //if player is within aggro radius and visible
    //or pot is damaged
    //move to charger_charge

    ChargerPot chargerPot;
    Health health;
    bool isDamaged;

    public override void Init(GameObject owner)
    {
        base.Init(owner);
        chargerPot = owner.GetComponent<ChargerPot>();
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
        if ((Physics.Raycast(owner.transform.position, towardPlayer, out hit, chargerPot.aggroRadius, ~LayerMask.GetMask("Enemy", "Trigger"))
            && hit.collider.tag == "Player")
            || isDamaged)
        {
            return "Charger_Charge";
        }

        return null;
    }

    public void OnDamage(float damage)
    {
        isDamaged = true;
    }
}

public class Charger_Charge : State
{
    //if player is within attack radius
    //move to charger_attack

    ChargerPot chargerPot;

    public override void Init(GameObject owner)
    {
        base.Init(owner);
        chargerPot = owner.GetComponent<ChargerPot>();
    }

    public override void Enter() { }

    public override void Exit() { }

    public override string Update()
    {
        if(Vector3.Distance(owner.transform.position, Player.Instance.transform.position) < chargerPot.attackRadius && 
            !chargerPot.stunned)
        {
            return "Charger_Attack";
        }

        if (agent.isActiveAndEnabled && !chargerPot.stunned)
        {
            agent.SetDestination(Player.Instance.transform.position);
        }


        return null;
    }
}

public class Charger_Attack : TimedState
{
    //trigger animation
    //after a certain amount of time move to charger_charge

    Animator animator;
    Attack attack;
    ChargerPot chargerPot;

    public Charger_Attack(float seconds)
        : base(seconds)
    { }

    public override void Init(GameObject owner)
    {
        base.Init(owner);
        animator = owner.GetComponentInChildren<Animator>();
        attack = owner.GetComponentInChildren<Attack>();
        chargerPot = owner.GetComponent<ChargerPot>();
    }

    public override void Enter()
    {
        base.Enter();
        animator.SetTrigger("Attack");
        attack.isAttacking = true;
    }

    public override void Exit()
    {
        attack.isAttacking = false;
        attack.hasHitPlayer = false;
    }

    public override string Update()
    {
        base.Update();

        //turn pot towards player
        Vector3 newForward = Player.Instance.transform.position - owner.transform.position;
        owner.transform.forward = newForward;

        if (timer >= seconds || chargerPot.stunned)
        {
            return "Charger_Charge";
        }

        return null;
    }
}
