using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Pot : MonoBehaviour
{
    //these variables are for the animations
    [SerializeField] float waddleAmplitude = 12.5f; //in degrees
    [SerializeField] float hopHeight = 0.5f; //in meters
    [Space]
    [SerializeField] public bool stunned = false;


    public StateMachine stateMachine;
    public NavMeshAgent agent;
    
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        stateMachine.Update();
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
        Vector3 euler = transform.rotation.eulerAngles;
        euler.z = waddleAmplitude * Mathf.Sin(Mathf.PI * agent.speed * Time.time);
        euler.y += euler.z * -0.1f;

        transform.rotation = Quaternion.Euler(euler);
    }

    //Hop Animation
    public void Hop()
    {
        Vector3 euler = transform.rotation.eulerAngles;
        euler.x = waddleAmplitude * Mathf.Sin(Mathf.PI * agent.speed * Time.time);

        transform.rotation = Quaternion.Euler(euler);

        Vector3 newPos = transform.position;
        newPos.y += Mathf.Max(hopHeight * Mathf.Cos(Mathf.PI * agent.speed * Time.time + Mathf.PI * 0.5f), 0);

        transform.position = newPos;
    }

    public NavMeshAgent GetAgent() {
        return agent;
    }
    public StateMachine GetStateMachine() {
        return stateMachine;
    }
}
