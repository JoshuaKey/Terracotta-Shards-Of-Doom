using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Pot : MonoBehaviour
{
    [SerializeField] float waddleAmplitude = 12.5f;
    [SerializeField] float hopHeight = 0.5f;
    [Space]

    protected StateMachine stateMachine;
    protected NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void Waddle()
    {
        Vector3 euler = transform.rotation.eulerAngles;
        euler.z = waddleAmplitude * Mathf.Sin(Mathf.PI * agent.speed * Time.time);
        euler.y += euler.z * -0.1f;

        transform.rotation = Quaternion.Euler(euler);
    }

    public void Hop()
    {
        Vector3 euler = transform.rotation.eulerAngles;
        euler.x = waddleAmplitude * Mathf.Sin(Mathf.PI * agent.speed * Time.time);

        transform.rotation = Quaternion.Euler(euler);

        Vector3 newPos = transform.position;
        newPos.y += Mathf.Max(hopHeight * Mathf.Cos(Mathf.PI * agent.speed * Time.time + Mathf.PI * 0.5f), 0);

        transform.position = newPos;
    }
}
