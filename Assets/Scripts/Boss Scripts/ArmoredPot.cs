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
        stateMachine.DEBUGGING = true;
        stateMachine.Init(gameObject,
            new Armored_Shooting(),
            new Armored_Spawning(),
            new Armored_Charging());
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

            if (armoredPot == null)
            {
                armoredPot = owner.GetComponent<ArmoredPot>();
            }

            if (AISpawnPoint == null)
            {
                AISpawnPoint = armoredPot.AISpawnPoint.transform;
            }

            if (SpawnableObjects == null)
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
            if (armoredPot.numArmorPieces != 0)
            {
                if (numberOfShotsFired < 3)
                {
                    if (!firing)
                    {
                        armoredPot.StartCoroutine(LaunchPot());
                    }
                }
                else if (numberOfShotsLanded == 3)
                {
                    if ((Target.transform.position - owner.transform.position).magnitude <= 50.0f)
                    {
                        numberOfShotsFired = 0;
                        numberOfShotsLanded = 0;
                        return "PUSH.ArmoredPot+Armored_Spawning";
                    }
                    else
                    {
                        armoredPot.StartCoroutine(LaunchPot());
                    }
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

            Collider collider = enemy.GetComponent<Collider>();
            collider.enabled = false;

            Pot pot = enemy.GetComponent<Pot>();
            pot.enabled = false;

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

            collider.enabled = false;

            pot.enabled = true;
            numberOfShotsLanded++;
            firing = false;
        }
    }

    public class Armored_Spawning : State
    {
        private Transform AISpawnPoint = null;
        private GameObject[] SpawnableObjects = null;
        //The state needs any monobehavior it can get to start a coroutine
        private ArmoredPot armoredPot = null;

        private byte numberOfBurstsFired = 0;
        private byte numberOfShotsLanded = 0;
        private bool firing = false;
        public override void Enter()
        {

            if (armoredPot == null)
            {
                armoredPot = owner.GetComponent<ArmoredPot>();
            }

            if (AISpawnPoint == null)
            {
                AISpawnPoint = armoredPot.AISpawnPoint.transform;
            }

            if (SpawnableObjects == null)
            {
                SpawnableObjects = Resources.LoadAll<GameObject>("Boss1_SpawnableObjects");
            }
        }

        public override void Exit()
        {
            numberOfBurstsFired = 0;
            numberOfShotsLanded = 0;
        }

        public override string Update()
        {
            if (!firing && numberOfBurstsFired != 3)
            {
                armoredPot.StartCoroutine(LaunchBurst());
            }
            else if (numberOfBurstsFired == 3)
            {
                if(armoredPot.numArmorPieces == 0)
                {

                }
                return "POP";
            }
            return null;
        }

        IEnumerator LaunchBurst()
        {
            firing = true;
            for (int i = 0; i < 3; i++)
            {
                armoredPot.StartCoroutine(LaunchPot());
            }

            while (numberOfShotsLanded < 3)
            {
                yield return null;
            }

            firing = false;
          
            numberOfShotsLanded = 0;
            numberOfBurstsFired++;
        }
        IEnumerator LaunchPot()
        {
            float time = 0.0f;

            GameObject enemy = GameObject.Instantiate<GameObject>(SpawnableObjects[0]);

            Collider collider = enemy.GetComponent<Collider>();
            collider.enabled = false;

            Pot pot = enemy.GetComponent<Pot>();
            pot.enabled = false;

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

            collider.enabled = true;

            pot.enabled = true;

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