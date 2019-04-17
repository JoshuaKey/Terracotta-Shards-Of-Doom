using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ArmoredPot : Pot
{
    new Transform transform = null;

    private NavMeshAgent navMeshAgent = null;
    public GameObject AISpawnPoint = null;

    private int numArmorPieces = 3;

    public int NumArmorPieces
    {
        get
        {
            return numArmorPieces;
        }

        set
        {
            if (numArmorPieces - value == 1)
            {
                numArmorPieces = value;
            }
        }
    }

    void Start()
    {
        transform = GetComponent<Transform>();
        navMeshAgent = GetComponent<NavMeshAgent>();
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

    #region Boss States
    public class Armored_Shooting : State
    {
        private GameObject Target = null;
        private Transform AISpawnPoint = null;
        //The state needs any monobehavior it can get to start a coroutine
        private ArmoredPot armoredPot = null;
        private GameObject[] SpawnableObjects = null;

        private byte numberOfShotsFired = 0;
        private byte numberOfShotsLanded = 0;

        private bool firing = false;
        public override void Enter()
        {
            if (Target == null)
            {
                Target = GameObject.FindGameObjectWithTag("Player");
            }

            if (AISpawnPoint == null)
            {
                AISpawnPoint = owner.GetComponent<ArmoredPot>().AISpawnPoint.transform;
            }

            if (SpawnableObjects == null)
            {
                SpawnableObjects = Resources.LoadAll<GameObject>("Boss1_SpawnableObjects");
            }
            if (armoredPot == null)
            {
                armoredPot = owner.GetComponent<ArmoredPot>();
            }
        }

        public override void Exit()
        {
            numberOfShotsFired = 0;
            numberOfShotsLanded = 0;
        }

        public override string Update()
        {
            if (armoredPot.numArmorPieces != 0)
            {
                if (numberOfShotsFired < 3)
                {
                    if (!firing)
                    {
                        armoredPot.StartCoroutine(LaunchPot());
                    }
                }
                else if (numberOfShotsLanded < 3)
                {
                    return "Armored_Spawning";
                }
            }
            return null;
        }

        IEnumerator LaunchPot()
        {
            firing = true;
            numberOfShotsFired++;

            float time = 0;
            //TODO: Change to pick a random object
            GameObject enemy = GameObject.Instantiate<GameObject>(SpawnableObjects[0]);

            Player player = Target.GetComponent<Player>();

            Vector3 startPosition = AISpawnPoint.position;

            Vector3 targetPosition = Vector3.zero;
            float targetX = player.transform.position.x + player.velocity.x;
            float targetZ = player.transform.position.z + player.velocity.z;
            targetPosition = new Vector3(targetX, player.transform.position.y, targetZ);

            Debug.Log(targetPosition);
            NavMeshHit hit;
            NavMesh.SamplePosition(owner.transform.position + targetPosition, out hit, 25.0f, NavMesh.AllAreas);
            targetPosition = hit.position;

            Vector3 peak = Utility.CreatePeak(startPosition, targetPosition, 75.0f - (startPosition - targetPosition).magnitude);

            while (time != 1.5f)
            {
                time = Mathf.Clamp(time += Time.deltaTime, 0.0f, 1.5f);
                Vector3 newPosition = Utility.BezierCurve(startPosition, peak, targetPosition, time / 1.5f);
                enemy.transform.position = newPosition;

                yield return null;
            }

            numberOfShotsLanded++;
            firing = false;
        }
    }

    public class Armored_Spawning : State
    {
        private Transform AISpawnPoint = null;
        private GameObject[] SpawnableObjects = null;
        //The state needs any monobehavior it can get to start a coroutine
        private MonoBehaviour monoBehaviour = null;

        private byte numberOfBurstsFired = 0;
        private byte numberOfShotsLanded = 0;
        private bool firing = false;
        public override void Enter()
        {
            if (AISpawnPoint == null)
            {
                AISpawnPoint = owner.GetComponent<ArmoredPot>().AISpawnPoint.transform;
            }

            if (SpawnableObjects == null)
            {
                SpawnableObjects = Resources.LoadAll<GameObject>("Boss1_SpawnableObjects");
            }

            if (monoBehaviour == null)
            {
                monoBehaviour = owner.GetComponent<MonoBehaviour>();
            }
        }

        public override void Exit()
        {
            numberOfBurstsFired = 0;
        }

        public override string Update()
        {
            if (!firing && numberOfBurstsFired != 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    monoBehaviour.StartCoroutine(LaunchPot());
                }
                //monoBehaviour.StartCoroutine(LaunchBurst());
            }
            else if (numberOfBurstsFired == 3)
            {
                //TODO: Change state
            }
            return null;
        }

        IEnumerator LaunchBurst()
        {
            firing = true;

            while (numberOfShotsLanded < 3)
            {
                yield return null;
            }

        }
        IEnumerator LaunchPot()
        {
            float time = 0.0f;

            GameObject enemy = GameObject.Instantiate<GameObject>(SpawnableObjects[1]);

            Vector3 startPosition = AISpawnPoint.position;
            Vector2 direction = Vector2.zero;

            Vector3 targetPosition = Vector3.zero;

            NavMeshPath path = new NavMeshPath();

            do
            {
                direction = Random.insideUnitCircle.normalized * Random.Range(10.0f, 75.0f);
                targetPosition = new Vector3(direction.x, 0.0f, direction.y);

                NavMeshHit hit;
                NavMesh.SamplePosition(owner.transform.position + targetPosition, out hit, 25.0f, NavMesh.AllAreas);
                targetPosition = hit.position;

            } while (!(NavMesh.CalculatePath(Vector3.zero, targetPosition, NavMesh.AllAreas, path)));

            Vector3 peak = Utility.CreatePeak(startPosition, targetPosition, 100.0f - (startPosition - targetPosition).magnitude);

            while (time != 1.5f)
            {
                time = Mathf.Clamp(time += Time.deltaTime, 0.0f, 1.5f);
                Vector3 newPosition = Utility.BezierCurve(startPosition, peak, targetPosition, time / 1.5f);

                enemy.transform.position = newPosition;

                yield return null;
            }
            numberOfShotsLanded++;
        }
    }

    public class Armored_Charging : State
    {
        private GameObject target = null;
        private MonoBehaviour monoBehaviour = null;
        private bool charging = false;

        public override void Enter()
        {
            if (target == null)
            {
                target = GameObject.FindGameObjectWithTag("Player");
            }

            if (monoBehaviour == null)
            {
                monoBehaviour = owner.GetComponent<MonoBehaviour>();
            }
        }

        public override void Exit()
        {

        }

        public override string Update()
        {
            if (!charging)
            {
                monoBehaviour.StartCoroutine(Charge());
            }
            return null;
        }

        IEnumerator Charge()
        {
            charging = true;

            NavMeshAgent navMeshAgent = owner.GetComponent<NavMeshAgent>();
            //NavMesh.CalculatePath();
            Vector3 targetPosition = target.transform.position;
            navMeshAgent.SetDestination(targetPosition);
            while (navMeshAgent.remainingDistance < .01f)
            {
                yield return null;
            }

            charging = false;
        }
    }
    #endregion
}