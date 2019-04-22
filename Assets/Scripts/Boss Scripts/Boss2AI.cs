using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss2AI : Pot
{
    NavMeshAgent navMeshAgent = null;
    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        stateMachine = new StateMachine();
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
    }

    private class Boss2AI_Panicked : TimedState
    {
        public Boss2AI_Panicked(float seconds) : base(seconds)
        {

        }

        public override void Exit()
        {
            throw new System.NotImplementedException();
        }
    }
}
