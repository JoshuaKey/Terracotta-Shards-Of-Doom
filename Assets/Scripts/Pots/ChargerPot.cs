using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerPot : Pot
{
    [SerializeField] public float aggroRadius = 5f;
    [SerializeField] public float attackRadius = 2.5f;
    [SerializeField] public float attackDuration = 0.25f;
    [SerializeField] public float knockback = 20f;

    [HideInInspector] public bool isAttacking = false;
    [HideInInspector] public bool hasHitPlayer = false;

    //delete this later
    public static bool debugged = false;
    //delete this later

    private void Start()
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

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(Game.Instance.PlayerTag) && isAttacking && !hasHitPlayer) {
            hasHitPlayer = true;
            Player.Instance.health.TakeDamage(DamageType.BASIC, 1);
            Player.Instance.Knockback(this.transform.forward * knockback);
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
        if ((Physics.Raycast(owner.transform.position, towardPlayer, out hit, chargerPot.aggroRadius, ~LayerMask.GetMask("Enemy"))
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
        if(Vector3.Distance(owner.transform.position, Player.Instance.transform.position) < chargerPot.attackRadius)
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

    public Charger_Attack(float seconds)
        : base(seconds)
    { }

    public override void Init(GameObject owner)
    {
        base.Init(owner);
        animator = owner.GetComponentInChildren<Animator>();
        Debug.Log(animator != null);
    }

    public override void Enter()
    {
        base.Enter();
        animator.SetTrigger("Attack");
    }

    public override void Exit() { }

    public override string Update()
    {
        base.Update();

        if(timer >= seconds)
        {
            return "Charger_Charge";
        }

        return null;
    }
}
