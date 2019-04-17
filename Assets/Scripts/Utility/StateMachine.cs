using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateMachine
{
    Dictionary<string, State> states;
    Stack<State> curState;
    GameObject owner;

    //////////////////////
    public bool DEBUGGING = false;
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
        curState = new Stack<State>();
        AddStates(initStates);
    }

    /// <summary>
    /// updates curState and if the State returns a string changes to the relevant State. Throws an error if
    /// curState is null
    /// </summary>
    public void Update()
    {
        if(curState.Count < 1)
        {
            throw new System.NullReferenceException
                ("The curState in StateMachine is empty. Did you forget to add States?");
        }

        if (DEBUGGING) Debug.Log($"Updated State {curState.Peek()}");

        string nextState = curState.Peek().Update();
        if(nextState != null)
        {
            string[] splitNextState = nextState.Split('.');

            if(splitNextState[0].ToUpper() == "PUSH")
            {
                PushState(splitNextState[1]);
            }
            else if(nextState.ToUpper() == "POP")
            {
                PopState();
            }
            else
            {
                ChangeState(nextState);
            }
        }
    }

    /// <summary>
    /// intializes and adds a State to states. uses the State's name as a string, so a State named RunAway is 
    /// automatically stored with the key "RunAway". If this is the first State being added it automatically 
    /// pusshes state to curState
    /// </summary>
    /// <param name="state"></param>
    public void AddState(State state)
    {
        state.Init(owner);

        if (states.Count < 1)
        {
            curState.Push(state);
            curState.Peek().Enter();
        }

        states.Add(state.ToString(), state);
    }

    /// <summary>
    /// adds multiple States at once to states
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
    /// removes a State from states
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
    public void ChangeState(string stateName)
    {
        if (!states.ContainsKey(stateName))
        {
            throw new System.ArgumentException($"The StateMachine does not contain State '{stateName}'.");
        }

        if (DEBUGGING) Debug.Log($"Exited State {curState.Peek()}. Entered State {stateName}");

        curState.Pop().Exit();
        curState.Push(states[stateName]);
        curState.Peek().Enter();
    }

    /// <summary>
    /// Pushes a State on top of the curState. If no State is found, it throws an error
    /// </summary>
    /// <param name="stateName"></param>
    public void PushState(string stateName)
    {
        if (!states.ContainsKey(stateName))
        {
            throw new System.ArgumentException($"The StateMachine does not contain State '{stateName}'.");
        }

        if (DEBUGGING) Debug.Log($"Exited State {curState.Peek()}. Entered State {stateName} (PUSHED)");

        curState.Peek().Exit();
        curState.Push(states[stateName]);
        curState.Peek().Enter();
    }

    /// <summary>
    /// Pops the current state. If there is no state underneath, it throws an error
    /// </summary>
    public void PopState()
    {
        State oldState = curState.Pop();

        if (DEBUGGING) Debug.Log($"Exited State {oldState}. Entered State {curState.Peek()} (POPPED)");

        oldState.Exit();

        if (curState.Count < 1)
        {
            throw new System.NullReferenceException("The StateMachine does not have any States left in curState.");
        }

        curState.Peek().Enter();
    }

    public State GetCurrState()
    {
        return curState.Peek();
    }
}

public abstract class State
{
    public GameObject owner;
    public NavMeshAgent agent;

    /// <summary>
    /// sets variables based on owner
    /// </summary>
    /// <param name="owner"></param>
    public void Init(GameObject owner)
    {
        this.owner = owner;
        agent = owner.GetComponent<NavMeshAgent>();
    }

    /// <summary>
    /// called the StateMachine update before this State starts being updated
    /// </summary>
    public abstract void Enter();

    /// <summary>
    /// called when this is the curState
    /// </summary>
    /// <returns></returns>
    public abstract string Update();

    /// <summary>
    /// called on the last update before the curState changes to a different State
    /// </summary>
    public abstract void Exit();

    /// <summary>
    /// returns the type of the State in string form. For example a state called DoNothing in code returns 
    /// "DoNothing". this is useful for automatically making a key for states in StateMachine
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return GetType().ToString();
    }
}
