using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicPot : Pot
{

    [SerializeField] public float aggroRadius = 5f; // this is when the pot first wakes up
    [SerializeField] public float chaseRadius = 10.0f; // this is after the pot is awake
    //This is all atack stuff and i currently dont wanna change it
    [SerializeField] public float attackRadius = 2.5f;
    [SerializeField] public float attackDuration = 0.25f;
    [SerializeField] public float attackAngle = 70f;

    [HideInInspector] public bool isAttacking = false;
    [HideInInspector] public bool hasHitPlayer = false;


    void Start()
    {
        stateMachine = new StateMachine();
        stateMachine.Init(gameObject,
            new Mimic_Idle(),
            new Mimic_Charge(),
            new Mimic_Attack());
    }

    public override void Animate()
    {
        // I have to come up with a cute way to do this
        Hop();
        //but for now i Hop
    }

    public void OnTriggerEnter(Collider other)
    {
        //This is just the base Charger Pot code idk if i need to do anything special with this
        if (other.CompareTag(Game.Instance.PlayerTag) && isAttacking && !hasHitPlayer)
        {
            hasHitPlayer = true;
            Player.Instance.health.TakeDamage(DamageType.BASIC, 1);
        }
    }

}



public class Mimic_Idle : State
{
    GameObject player;

    public override void Init(GameObject owner)
    {
        base.Init(owner);
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public override void Enter()
    {
    }

    public override void Exit()
    {
    }

    public override string Update()
    {
        //This will check if it can wake up or not
        if (Vector3.Distance(owner.transform.position, player.transform.position) < owner.GetComponent<MimicPot>().aggroRadius)
        {
            return "Mimic_Charge";
        }

        //This will check if the player is in the chase radius and has hit the pot 
        //(so if they attack it from range it will charge them)
        if (Vector3.Distance(owner.transform.position, player.transform.position) < owner.GetComponent<MimicPot>().chaseRadius
            //this is awful and i hate it
            && owner.GetComponent<Health>().CurrentHealth != owner.GetComponent<Health>().MaxHealth)
        {
            return "Mimic_Charge";
        }

        return null;
    }
}

public class Mimic_Charge : State
{
    GameObject player;

    public override void Init(GameObject owner)
    {
        base.Init(owner);
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public override void Enter()
    {
    }

    public override void Exit()
    {// this might actually be useful because we can tell it to like close its mouth
    }

    public override string Update()
    {
        float distance = Vector3.Distance(owner.transform.position, player.transform.position);
        MimicPot mp = owner.GetComponent<MimicPot>();

        if (distance > mp.chaseRadius)
        {
            return "Mimic_Idle";
        }

        if (distance < mp.attackRadius)
        {
            return "PUSH.Mimic_Attack";
        }

        if (agent.isActiveAndEnabled)
        {
            agent.SetDestination(player.transform.position);
        }

        return null;
    }
}

public class Mimic_Attack : TimedState
{
    GameObject player;
    MimicPot mp;

    public override void Init(GameObject owner)
    {
        //why do i need this if im just checking for it in the Enter function?
        base.Init(owner);
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public override void Enter()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        if (mp == null)
        {
            mp = owner.GetComponent<MimicPot>();
        }
        timer = 0;
        mp.isAttacking = true;
        mp.hasHitPlayer = false;
        if (agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }
    }

    public override void Exit()
    {
        mp.isAttacking = false;
        mp.hasHitPlayer = false;
        if (agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.isStopped = false;
        }
    }

    public override string Update()
    {
        base.Update();

        //This is for the headbutt ... we are eventually going to change this with an animation so i dont think i care enough to change this code
        Vector3 newRotation = owner.transform.rotation.eulerAngles;

        newRotation.x = timer * (timer - mp.attackDuration) * (-4 * mp.attackAngle) / Mathf.Pow(mp.attackDuration, 2);
        newRotation.y = Quaternion.LookRotation(player.transform.position - owner.transform.position).eulerAngles.y;

        owner.transform.rotation = Quaternion.Euler(newRotation);

        if (timer >= mp.attackDuration)
        {
            return "POP";
        }

        return null;
    }
}
