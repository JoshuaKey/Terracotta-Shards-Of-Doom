using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierPot : Pot
{
    private Waypoint waypoint;

    public Waypoint Waypoint
    {
        get { return waypoint; }
        set { waypoint = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        stateMachine = new StateMachine();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    class EnterFormation : State
    {
        public override void Enter()
        {
        }

        public override void Exit()
        {
        }

        public override string Update()
        {
            return null;
        }
    }
}