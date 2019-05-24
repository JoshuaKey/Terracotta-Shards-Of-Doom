using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {

    //[Header("Coins")]
    //public Vector2Int CoinDropRange;
    //public int BaseValue;
    //public bool UseBigCoins = false;

    //[HideInInspector]
    public Health health;
    [HideInInspector]
    public Pot pot;
    [HideInInspector]
    public BrokenPot brokenPot;
    [HideInInspector]
    public NavMeshAgent agent;
    [HideInInspector]
    public new MeshRenderer renderer;

    public bool CanBeKnockedBack = true;

    [HideInInspector]
    public Animator animator;

    private new Rigidbody rigidbody;
    private new Collider collider;

    void Awake() {
        collider = GetComponent<Collider>();
        if (collider == null) { collider = GetComponentInChildren<Collider>(true); }

        rigidbody = GetComponent<Rigidbody>();
        if (rigidbody == null) { rigidbody = GetComponentInChildren<Rigidbody>(true); }

        pot = GetComponent<Pot>();
        if (pot == null) { pot = GetComponentInChildren<Pot>(true); }

        brokenPot = GetComponent<BrokenPot>();
        if (brokenPot == null) { brokenPot = GetComponentInChildren<BrokenPot>(true); }

        health = GetComponent<Health>();
        if (health == null) { health = GetComponentInChildren<Health>(true); }

        if (agent == null) { agent = GetComponentInChildren<NavMeshAgent>(true); }
        
        if(renderer == null) { renderer = GetComponentInChildren<MeshRenderer>(true); }

        animator = GetComponentInChildren<Animator>();
    }

    private void Start() {
        this.gameObject.tag = "Enemy";
        this.gameObject.layer = LayerMask.NameToLayer("Enemy");

        rigidbody.isKinematic = true;

        health.OnDeath += this.Die;
    }

    public void Knockback(Vector3 force, float duration) {
        if (CanBeKnockedBack) {
            StartCoroutine(KnockbackRoutine(force, duration));
        }
    }

    public void Explode(Vector3 force, Vector3 pos) {
        if (brokenPot != null && brokenPot.gameObject.activeInHierarchy) {
            brokenPot.Explode(force, pos);
        }
    }

    public void Slow(float speedMultiplier, float duration) {
        if (CanBeKnockedBack) {
            StartCoroutine(SlowRoutine(speedMultiplier, duration));
        }
    }

    private void Die() {
        string[] sounds = {
            "ceramic_shatter1",
            "ceramic_shatter2",
            "ceramic_shatter3",
        };
        AudioManager.Instance.PlaySoundAtLocation(sounds[Random.Range(0, sounds.Length)], ESoundChannel.SFX, transform.position);

        this.gameObject.SetActive(false);
        if(brokenPot != null) {
            brokenPot.gameObject.SetActive(true);
            brokenPot.transform.parent = null;
        }


        health.OnDeath -= this.Die;

        Destroy(this.gameObject);
    }

    public void SetMaterial(Material m) {
        pot.SetMaterial(m);
        brokenPot.SetMaterial(m);
    }

    protected IEnumerator KnockbackRoutine(Vector3 force, float duration)
    {
        float angularSpeed = 0;
        if (agent != null)
        {
            agent.SetDestination(transform.position + force);
            agent.speed *= 2;
            angularSpeed = agent.angularSpeed;
            agent.angularSpeed = 0;
            agent.acceleration *= 2;
        }
        if (pot != null)
        {
            pot.stunned = true;
        }
        if(animator != null)
        {
            animator.SetTrigger("Knockback");
        }

        yield return new WaitForSeconds(duration);           

        if(agent != null)
        {
            agent.speed *= 0.5f;
            agent.angularSpeed = angularSpeed;
            agent.acceleration *= 0.5f;
        }
        if (pot != null) {
            pot.stunned = false;
        }
    }

    protected IEnumerator SlowRoutine(float speedMultiplier, float duration) {
        if (agent != null) {
            agent.speed *= speedMultiplier;
        }

        yield return new WaitForSeconds(duration);

        if (agent != null) {
            agent.speed *= 1 / speedMultiplier;
        }
    }
}
