using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmoredPot : Pot
{
    new Transform transform = null;

    private int numArmorPieces = 3;

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

    public void RunStateCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }

    #region Boss States
    public class Armored_Shooting : State
    {
        private GameObject Boss = null;
        private GameObject Target = null;
        private Transform AISpawnPoint = null;

        private GameObject[] SpawnableObjects = null;

        private byte numberOfShotsFired = 0;
        private byte numberOfShotsLanded = 0;

        private bool firing = false;

        public override void Enter()
        {
            if(Boss == null)
            {
                Boss = GameObject.FindGameObjectWithTag("Boss");
            }

            if(Target == null)
            {
                Target = GameObject.FindGameObjectWithTag("Player");
            }

            if(AISpawnPoint == null)
            {
                AISpawnPoint = GameObject.FindGameObjectWithTag("AISpawnPoint").transform;
            }

            if(SpawnableObjects == null)
            {
                SpawnableObjects = Resources.LoadAll<GameObject>("Boss1_SpawnableObjects"); 
            }
        }

        public override void Exit()
        {
            numberOfShotsFired = 0;
            numberOfShotsLanded = 0;
        }

        public override string Update()
        {
            if(numberOfShotsFired < 3)
            {
                if(!firing)
                {
                    owner.GetComponent<ArmoredPot>().RunStateCoroutine(LaunchPot());
                }
            }
            else if(numberOfShotsLanded < 3)
            {

            }
            return null;
        }

        IEnumerator LaunchPot()
        {
            firing = true;
            numberOfShotsFired++;

            float time = 0;
            GameObject enemy = GameObject.Instantiate<GameObject>(SpawnableObjects[0]);

            Player player = Target.GetComponent<Player>();

            Vector3 targetPosition = Vector3.zero;
            float targetX = player.transform.position.x + player.velocity.x;
            float targetZ = player.transform.position.z + player.velocity.z;
            targetPosition = new Vector3(targetX, player.transform.position.y, targetZ);

            Vector3 startPosition = AISpawnPoint.position;

            Vector3 peak = Utility.CreatePeak(startPosition, targetPosition, 75.0f - (startPosition - targetPosition).magnitude);

            while (time != 1.0f)
            {
            
                time = Mathf.Clamp(time += Time.deltaTime, 0.0f, 1.0f);
                Vector3 newPosition = Utility.BezierCurve(startPosition, peak, targetPosition, time);

                enemy.transform.position = newPosition;

                yield return null;
            }
            numberOfShotsLanded++;
            firing = false;
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