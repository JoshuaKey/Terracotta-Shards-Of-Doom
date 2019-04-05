using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateMachine
{
    Dictionary<string, State> states;
    State curState;
    GameObject owner;

    //////////////////////
    bool DEBUGGING = false;
    //////////////////////

    /// <summary>
    /// sets owner to owner and adds all states in initStates to states
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="initStates"></param>
    public void Init(GameObject owner, params State[] initStates)
    {
        this.owner = owner;
        states = new Dictionary<string, State>();
        AddStates(initStates);
    }

    /// <summary>
    /// updates curState and if the State returns a string changes to the relevant State. Throws an error if
    /// curState is null
    /// </summary>
    public void Update()
    {
        if(curState == null)
        {
            throw new System.NullReferenceException
                ("The curState in StateMachine is null. Did you forget to add States?");
        }

        if (DEBUGGING) Debug.Log($"Updated State {curState}");

        string nextState = curState.Update();
        if(nextState != null)
        {
            ChangeState(nextState);
        }
    }

    /// <summary>
    /// intializes and adds a State to states. uses the State's name as a string, so a State named RunAway is 
    /// automatically stored with the key "RunAway". If this is the first State being added it automatically 
    /// sets curState to state.
    /// </summary>
    /// <param name="state"></param>
    public void AddState(State state)
    {
        state.Init(owner);

        if (states.Count == 0)
        {
            curState = state;
            curState.Enter();
        }

        states.Add(state.ToString(), state);
    }

    /// <summary>
    /// adds multiple States at once
    /// </summary>
    /// <param name="initStates"></param>
    public void AddStates(IEnumerable<State> initStates)
    {
        foreach (State s in initStates)
        {
            AddState(s);
        }
    }

    /// <summary>
    /// removes a State
    /// </summary>
    /// <param name="state"></param>
    public void RemoveState(State state)
    {
        states.Remove(state.ToString());
    }

    /// <summary>
    /// exits curState then changes it to the State stored with the key stateName then enters that State. If no 
    /// State is found, it throws an error
    /// </summary>
    /// <param name="stateName"></param>
    void ChangeState(string stateName)
    {
        if (!states.ContainsKey(stateName))
        {
            throw new System.ArgumentException($"The StateMachine does not contain State '{stateName}'.");
        }

        if (DEBUGGING) Debug.Log($"Exited State {curState}. Entered State {stateName}");

        curState.Exit();
        curState = states[stateName];
        curState.Enter();
    }
}

public abstract class State
{
    public GameObject owner;
    public NavMeshAgent agent;

    public void Init(GameObject owner)
    {
        this.owner = owner;
        agent = owner.GetComponent<NavMeshAgent>();
    }

    public abstract void Enter();

    public abstract string Update();

    public abstract void Exit();

    public override string ToString()
    {
        return GetType().ToString();
    }
}
