using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmoredPot : Pot
{
    new Transform transform;
    private int numArmorPieces;

    public int NumArmorPieces
    {
        get
        {
            return numArmorPieces;
        }

        set
        {
            if(numArmorPieces - value == 1)
            {
                numArmorPieces = value;
            }
        }
    }


    void Start()
    {
        transform = GetComponent<Transform>();
        gameObject.tag = "Boss";
        stateMachine = new StateMachine();
        //stateMachine.Init(gameObject, 
        //    new Armored_Spawning(), 
        //    new Armored_Shooting(), 
        //    new Armored_Charging());
        stateMachine.Init(gameObject, new Armored_Shooting());
    }

    void Update()
    {
        stateMachine.Update();
    }
    #region Boss States
    public class Armored_Shooting : State
    {
        private GameObject boss = null;
        private GameObject target = null;

        public override void Enter()
        {
            if(boss == null)
            {
                boss = GameObject.FindGameObjectWithTag("Boss");
            }

            if(target == null)
            {
                target = GameObject.FindGameObjectWithTag("Player");
            }
        }

        public override void Exit()
        {
            //Any Cleanup
        }

        public override string Update()
        {
            
            Player player = target.GetComponent<Player>();
            Vector3 targetPosition = Vector3.zero;

            float targetX = player.transform.position.x + player.velocity.x;
            float targetZ = player.transform.position.z + player.velocity.z;

            targetPosition = new Vector3(targetX, player.transform.position.y, targetZ);
           
            Debug.DrawLine(owner.transform.position, targetPosition, Color.red);

            return null;
        }
    }

    public class Armored_Spawning : State
    {
        private GameObject boss = null;
        public override void Enter()
        {
            if(boss == null)
            {
                boss = GameObject.FindGameObjectWithTag("Boss");
            }
        }

        public override void Exit()
        {
            
        }

        public override string Update()
        {
            if(boss.GetComponent<ArmoredPot>().NumArmorPieces != 0)
            {
                Utility
            }
            return null;
        }
    }

    public class Armored_Charging : State
    {
        private GameObject boss = null;
        private GameObject target = null;

        public override void Enter()
        {
            if(boss == null)
            {
                boss = GameObject.FindGameObjectWithTag("Boss");
            }

            if (target == null)
            {
                target = GameObject.FindGameObjectWithTag("Player");
            }
        }

        public override void Exit()
        {
            throw new System.NotImplementedException();
        }

        public override string Update()
        {
            return null;
        }
    }
    #endregion
}