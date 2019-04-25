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
            numArmorPieces = value;
        }
    }

    void Start()
    {
        transform = GetComponent<Transform>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        gameObject.tag = "Boss";
        stateMachine = new StateMachine();
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

        private float timer = 0.0f;
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
            timer = 0.0f;
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
                else if (numberOfShotsLanded >= 3)
                {
                    timer += Time.deltaTime;
                    if ((Target.transform.position - owner.transform.position).magnitude <= 50.0f)
                    {

                        return "ArmoredPot+Armored_Spawning";
                    }
                    else
                    {
                        if(!firing)
                        {
                            armoredPot.StartCoroutine(LaunchPot());
                        }
                    }
                }
            }
            else
            {
                return "ArmoredPot+Armored_Charging";
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

            NavMeshAgent navAgent = enemy.GetComponent<NavMeshAgent>();
            navAgent.enabled = false;

            Pot pot = enemy.GetComponent<Pot>();
            pot.enabled = false;

            Player player = Player.Instance;

            Vector3 startPosition = AISpawnPoint.position;

            Vector3 targetPosition = Vector3.zero;
            float targetX = player.transform.position.x + player.velocity.x;
            float targetZ = player.transform.position.z + player.velocity.z;
            targetPosition = new Vector3(targetX, player.transform.position.y, targetZ);

            NavMeshHit hit;
            NavMesh.SamplePosition(owner.transform.position + targetPosition, out hit, 25.0f, NavMesh.AllAreas);
            targetPosition = hit.position;

            Vector3 peak = Utility.CreatePeak(startPosition, targetPosition, 150.0f - (startPosition - targetPosition).magnitude);

            while (time != 1.5f)
            {
                time = Mathf.Clamp(time += Time.deltaTime, 0.0f, 1.5f);
                Vector3 newPosition = Utility.BezierCurve(startPosition, peak, targetPosition, time / 1.5f);
                enemy.transform.position = newPosition;

                yield return null;
            }

            navAgent.enabled = true;

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

        private float timer = 0;
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
            timer = 0;
        }

        public override string Update()
        {
            if (!firing && numberOfBurstsFired != 3)
            {
                armoredPot.StartCoroutine(LaunchBurst());
            }
            else if (numberOfBurstsFired == 3)
            {
                if(armoredPot.NumArmorPieces == 0)
                {
                    timer += Time.deltaTime;
                    if(timer >= 2.0f)
                    {
                        return "ArmoredPot+Armored_Charging";
                    }
                }
                else if(armoredPot.numArmorPieces != 0)
                {
                    timer += Time.deltaTime;
                    if(timer >= 2.0f)
                    {
                        return "ArmoredPot+Armored_Shooting";
                    }
                }
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

            //TODO: Change to pick a random object
            GameObject enemy = GameObject.Instantiate<GameObject>(SpawnableObjects[0]);

            NavMeshAgent navAgent = enemy.GetComponent<NavMeshAgent>();
            navAgent.enabled = false;

            Pot pot = enemy.GetComponent<Pot>();
            pot.enabled = false;

            Vector3 startPosition = AISpawnPoint.position;
            Vector2 direction = Vector2.zero;

            Vector3 targetPosition = Vector3.zero;

            NavMeshPath path = new NavMeshPath();

            do
            {
                direction = Random.insideUnitCircle.normalized * Random.Range(10.0f, 50.0f);
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

            navAgent.enabled = true;
            
            pot.enabled = true;

            numberOfShotsLanded++;
        }
    }

    public class Armored_Charging : State
    {
        private GameObject target = null;
        private ArmoredPot armoredPot = null;
        private MonoBehaviour monoBehaviour = null;

        private bool charging = false;
        private bool charged = false;
        private float timer = 0.0f;
        //private float rotateSpeed = 30.0f;

        public override void Enter()
        {
            if(armoredPot == null)
            {
                armoredPot = owner.GetComponent<ArmoredPot>();
            }

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
            charged = false;
            timer = 0.0f;
        }

        public override string Update()
        {
            if (!charging)
            {
                //Transform transform = owner.transform;

                //Quaternion currentRotation = transform.rotation;

                //Vector3 targetPosition = target.transform.position;
                //Vector3 targetOffset = (new Vector3(Player.Instance.velocity.x, 0.0f, Player.Instance.velocity.z)) * 1.5f;
                //targetPosition = new Vector3(targetPosition.x, 0.0f, targetPosition.z) + targetOffset;

                //Vector3 targetDirection = (targetPosition - transform.position).normalized;

                //Quaternion desiredRotation = Quaternion.FromToRotation(transform.forward, targetDirection);
                //Quaternion.RotateTowards(currentRotation, desiredRotation, rotateSpeed * Time.deltaTime);
                timer += Time.deltaTime;
                if(timer > 2.5f)
                {
                    monoBehaviour.StartCoroutine(Charge());
                }
            }
            else if(charged)
            {
                return "ArmoredPot+Armored_Spawning";
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
            while ((navMeshAgent.destination - owner.transform.position).magnitude >.1f)
            {
                yield return null;
            }

            charging = false;
            charged = true;
        }
    }
    #endregion
}