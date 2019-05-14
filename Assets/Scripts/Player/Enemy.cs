using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {

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

    }

    private void Start() {
        this.gameObject.tag = "Enemy";
        this.gameObject.layer = LayerMask.NameToLayer("Enemy");

        rigidbody.isKinematic = true;

        health.OnDeath += this.Die;
    }

    //private void Update() {
    //    //print(agent.isOnNavMesh);
    //    if (!agent.isOnNavMesh) {
    //        rigidbody.constraints = RigidbodyConstraints.None;
    //    } else {
    //        rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    //    }
    //}

    public void Knockback(Vector3 force) {
        if (CanBeKnockedBack) {
            StartCoroutine(KnockbackRoutine(force));
        }     
    }

    public void Explode(Vector3 force, Vector3 pos) {
        if (brokenPot.gameObject.activeInHierarchy) {
            brokenPot.Explode(force, pos);
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
        brokenPot.gameObject.SetActive(true);
        brokenPot.transform.parent = null;

        health.OnDeath -= this.Die;

        Destroy(this.gameObject);
    }

    protected IEnumerator KnockbackRoutine(Vector3 force) {
        if (agent != null) {
            agent.SetDestination(this.transform.position + force);
        }
        if (pot != null) {
            pot.stunned = true;
        }

        yield return new WaitForSeconds(1f);

        if (pot != null) {
            pot.stunned = false;
        }
    }
}


