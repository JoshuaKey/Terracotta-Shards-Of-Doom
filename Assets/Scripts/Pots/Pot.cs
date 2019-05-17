using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Pot : MonoBehaviour
{
    //these variables are for the animations
    [SerializeField] float waddleAmplitude = 12.5f; //in degrees
    [SerializeField] float hopHeight = 0.5f; //in meters
    [Space]
    [SerializeField] public bool stunned = false;

    protected Health health;
    public StateMachine stateMachine;
    [HideInInspector]
    public NavMeshAgent agent;
    [HideInInspector]
    public Animator animator;

    public MeshRenderer[] potRenderers;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        health = GetComponent<Health>();
        health.OnDamage += PlayTink;
    }

    private void Update()
    {
        if (stateMachine != null)
        {
            stateMachine.Update();
        }
        if (agent.desiredVelocity.magnitude > 0)
        {
            Animate();
        }
    }

    //this does nothing but child classes can change that
    public virtual void Animate()
    { }

    //Waddle Animation
    public void Waddle()
    {
        if (!stunned && Time.timeScale != 0)
        {
            Vector3 euler = transform.rotation.eulerAngles;
            euler.z = waddleAmplitude * Mathf.Sin(Mathf.PI * agent.speed * Time.time);
            euler.y += euler.z * -0.1f;

            transform.rotation = Quaternion.Euler(euler);
        }
    }

    //Hop Animation
    public void Hop()
    {
        if (!stunned && Time.timeScale != 0)
        {
            Vector3 euler = transform.rotation.eulerAngles;
            euler.x = waddleAmplitude * Mathf.Sin(Mathf.PI * agent.speed * Time.time);

            transform.rotation = Quaternion.Euler(euler);

            Vector3 newPos = transform.position;
            newPos.y += Mathf.Max(hopHeight * Mathf.Cos(Mathf.PI * agent.speed * Time.time + Mathf.PI * 0.5f), 0);

            transform.position = newPos;
        }
    }

    public NavMeshAgent GetAgent()
    {
        return agent;
    }
    public StateMachine GetStateMachine()
    {
        return stateMachine;
    }

    public void PlayTink(float damage)
    {
        if (!health.IsDead())
        {
            AudioManager.Instance.PlaySoundWithParent("ceramic_tink", ESoundChannel.SFX, gameObject);
        }
    }
<<<<<<< HEAD
=======

    public void SetMaterial(Material m) {
        foreach(MeshRenderer r in potRenderers) {
            r.material = m;
        }
    }
>>>>>>> develop
}
