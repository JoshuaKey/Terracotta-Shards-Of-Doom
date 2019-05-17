using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss1Pot : Pot
{
    [SerializeField] public float knockback = 50f;

    [Header("Health")]
    public int Phase1Health = 3;
    public DamageType Phase1Resistance = DamageType.BASIC;

    public int Phase2Health = 10;
    public DamageType Phase2Resistance = 0;

    [Header("Spawning")]
    public int SpawnLimit = 50;
    public Vector2Int SpawnCoinDropRange = new Vector2Int(1,5);
    public int MaxSpawnsThatDropCoins = 100;
    [SerializeField] private int totalSpawns;
    [SerializeField] private List<Enemy> currSpawns = new List<Enemy>();

    [HideInInspector]
    public Enemy enemy;
    private NavMeshAgent navMeshAgent = null;

    [Header("AI")]
    public GameObject AISpawnPoint = null;
    #pragma warning disable 0649
    [SerializeField]
    Collider playerTrigger;
    #pragma warning restore 0649
    private bool playerEnteredArena;

    public bool PlayerEnteredArena
    {
        get { return playerEnteredArena; }
        set { playerEnteredArena = value; }
    }

    [Header("Visual")]
    public List<Rigidbody> ArmorPieces;

    void Start()
    {
        if (enemy == null) { enemy = GetComponentInChildren<Enemy>(true); }
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.avoidancePriority = 0;

        gameObject.tag = "Boss";

        stateMachine = new StateMachine();
        stateMachine.Init(gameObject,
            new Armored_Idle(),
            new Armored_Shooting(),
            new Armored_Spawning(),
            new Armored_Charging());

        enemy.health.MaxHealth = Phase1Health + Phase2Health;
        enemy.health.Resistance = Phase1Resistance;
        enemy.health.Reset();

        enemy.health.OnDamage += ChangeHealthUI;
        enemy.health.OnDamage += ChangePhase;
        enemy.health.OnDamage += PlayClang;
        enemy.health.OnDeath += OnDeath;
        ChangeHealthUI(0);
    }

    void Update()
    {
        stateMachine.Update();
    }

    public void PlayClang(float damage)
    {
        if (!health.IsDead())
        {
            AudioManager.Instance.PlaySoundWithParent("clang", ESoundChannel.SFX, gameObject);
        }
    }

    private void ChangeHealthUI(float val)
    {
        PlayerHud.Instance.SetBossHealthBar(enemy.health.CurrentHealth / enemy.health.MaxHealth);
    }

    private void OnDeath()
    {
        PlayerHud.Instance.DisableBossHealthBar();
    }

    private void ChangePhase(float val)
    {
        // Phase 2
        if (GetArmor() <= 0)
        {
            enemy.health.Resistance = Phase2Resistance;
            enemy.health.OnDamage -= ChangePhase;
            enemy.health.OnDamage -= PlayClang;
            enemy.health.OnDamage += PlayTink;
        }

        // Phase 1 - Remove Armor
        while (GetArmor() < ArmorPieces.Count)
        {
            int index = Random.Range(0, ArmorPieces.Count);
            Rigidbody armor = ArmorPieces[index];
            armor.isKinematic = false;
            armor.transform.parent = null;
            ArmorPieces.RemoveAt(index);
        }
    }

    public Enemy SpawnRandomPot(out int type)
    {
        Enemy enemy = null;
        type = -1;

        if (!CanSpawn())
        {
            //currSpawns[0].health.TakeDamage(DamageType.TRUE, currSpawns[0].health.CurrentHealth);
            Destroy(currSpawns[0].gameObject);
            currSpawns.RemoveAt(0);
        }

        Player player = Player.Instance;
        float percent = Random.Range(0.0f, 99.0f);

        if (player.health.CurrentHealth / player.health.MaxHealth <= .40f)
        {
            if (percent <= 9.0f)
            {
                enemy = EnemyManager.Instance.SpawnPot();
                type = 0;
            }
            else if (percent <= 39.0f)
            {
                enemy = EnemyManager.Instance.SpawnHealthPot();
                type = 1;
            }
            else if (percent <= 79.0f)
            {
                enemy = EnemyManager.Instance.SpawnChargerPot();
                type = 2;
            }
            else
            {
                enemy = EnemyManager.Instance.SpawnRunnerPot();
                type = 3;
            }
        }
        else
        {
            if (percent <= 14.0f)
            {
                enemy = EnemyManager.Instance.SpawnPot();
                type = 0;
            }
            else if (percent <= 29.0f)
            {
                enemy = EnemyManager.Instance.SpawnHealthPot();
                type = 1;
            }
            else if (percent <= 79.0f)
            {
                enemy = EnemyManager.Instance.SpawnChargerPot();
                type = 2;
            }
            else
            {
                enemy = EnemyManager.Instance.SpawnRunnerPot();
                type = 3;
            }
        }

        // We could just make another prefab variant with different Variables...
        CoinPowerUp coinPower = enemy.GetComponent<CoinPowerUp>();
        if (coinPower != null) {
            if (totalSpawns > MaxSpawnsThatDropCoins) {
                Destroy(coinPower);
            } else {
                coinPower.CoinDropRange = SpawnCoinDropRange;
            }
        }

        currSpawns.Add(enemy);
        enemy.health.OnDeath += OnSpawnDeath;
        totalSpawns++;

        return enemy;
    }

    private void OnSpawnDeath()
    {
        // ¯\_(ツ)_/¯
        for (int i = 0; i < currSpawns.Count; i++)
        {
            Enemy e = currSpawns[i];
            if (e.gameObject == null || e.health.IsDead())
            {
                currSpawns.RemoveAt(i);
            }
        }
    }

    public float GetArmor()
    {
        return Mathf.Max(enemy.health.CurrentHealth - Phase2Health, 0);
    }

    public bool CanSpawn()
    {
        return currSpawns.Count < SpawnLimit;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Game.Instance.PlayerTag))
        {
            if (playerTrigger.enabled)
            {
                playerTrigger.enabled = false;
                playerEnteredArena = true;
            }
            else
            {
                Player.Instance.health.TakeDamage(DamageType.BASIC, 1);
                Player.Instance.Knockback(this.transform.forward * knockback);
            }
        }
    }
    #region Boss States

    public class Armored_Idle : State
    {
        Boss1Pot boss1Pot = null;
        public override void Init(GameObject owner)
        {
            boss1Pot = owner.GetComponent<Boss1Pot>();
            base.Init(owner);
        }

        public override void Enter()
        {
        }

        public override void Exit()
        {

        }

        public override string Update()
        {
            if (boss1Pot.playerEnteredArena)
            {
                if (boss1Pot.GetArmor() <= 0)
                {
                    return "Boss1Pot+Armored_Charging";
                }
                else /*if (boss1Pot.CanSpawn()) */
                {
                    return "Boss1Pot+Armored_Spawning";
                }
            }
            return null;
        }
    }

    public class Armored_Shooting : State
    {
        private GameObject Target = null;
        private Transform AISpawnPoint = null;
        private Boss1Pot boss = null;

        private byte numberOfShotsFired = 0;
        private byte numberOfShotsLanded = 0;
        private bool firing = false;

        private float timer = 0.0f;

        public override void Init(GameObject owner)
        {
            Target = GameObject.FindGameObjectWithTag("Player");

            boss = owner.GetComponent<Boss1Pot>();

            AISpawnPoint = boss.AISpawnPoint.transform;

            base.Init(owner);

        }

        public override void Enter()
        {
            numberOfShotsFired = 0;
            numberOfShotsLanded = 0;
            timer = 0.0f;
        }

        public override void Exit()
        {
            numberOfShotsFired = 0;
            numberOfShotsLanded = 0;
            timer = 0.0f;
        }

        public override string Update()
        {
            if (boss.GetArmor() > 0)
            {
                //if (!boss.CanSpawn()) {
                //    return "Boss1Pot+Armored_Idle";
                //}

                if (numberOfShotsFired < 3)
                {
                    if (!firing && timer >= 2.0f)
                    {
                        boss.StartCoroutine(LaunchPot());
                        timer = 0;
                    }
                    else if (!firing)
                    {
                        timer += Time.deltaTime;
                    }
                }
                else if (numberOfShotsLanded >= 3)
                {
                    if ((Target.transform.position - owner.transform.position).magnitude <= 50.0f)
                    {
                        return "Boss1Pot+Armored_Spawning";
                    }
                    else
                    {
                        if (!firing && timer >= 2.0f)
                        {
                            boss.StartCoroutine(LaunchPot());
                            timer = 0.0f;
                        }
                        else if (!firing)
                        {
                            timer += Time.deltaTime;
                        }
                    }
                }
            }
            else
            {
                return "Boss1Pot+Armored_Charging";
            }
            return null;
        }

        IEnumerator LaunchPot()
        {
            firing = true;
            numberOfShotsFired++;

            float time = 0;

            Player player = Player.Instance;

            //Enemy enemy = null;
            //float percent = Random.Range(0.0f, 99.0f);

            //int type = -1;

            //if (player.health.CurrentHealth / player.health.MaxHealth <= .40f)
            //{
            //    if (percent <= 9.0f)
            //    {
            //        enemy = EnemyManager.Instance.SpawnPot();
            //        type = 0;
            //    }
            //    else if (percent <= 39.0f)
            //    {
            //        enemy = EnemyManager.Instance.SpawnHealthPot();
            //        type = 1;
            //    }
            //    else if (percent <= 79.0f)
            //    {
            //        enemy = EnemyManager.Instance.SpawnChargerPot();
            //        type = 2;
            //    }
            //    else if (percent <= 99.0f)
            //    {
            //        enemy = EnemyManager.Instance.SpawnRunnerPot();
            //        type = 3;
            //    }
            //}
            //else
            //{
            //    if (percent <= 14.0f)
            //    {
            //        enemy = EnemyManager.Instance.SpawnPot();
            //        type = 0;
            //    }
            //    else if (percent <= 29.0f)
            //    {
            //        enemy = EnemyManager.Instance.SpawnHealthPot();
            //        type = 1;
            //    }
            //    else if (percent <= 79.0f)
            //    {
            //        enemy = EnemyManager.Instance.SpawnChargerPot();
            //        type = 2;
            //    }
            //    else if (percent <= 99.0f)
            //    {
            //        enemy = EnemyManager.Instance.SpawnRunnerPot();
            //        type = 3;
            //    }
            //}
            int type;
            Enemy enemy = boss.SpawnRandomPot(out type);
            if (enemy == null)
            {
                yield break;
            }

            NavMeshAgent navAgent = enemy.GetComponent<NavMeshAgent>();
            if (navAgent != null)
            {
                navAgent.enabled = false;
            }

            Pot pot = enemy.GetComponent<Pot>();
            if (pot != null)
            {
                pot.enabled = false;
            }


            Vector3 startPosition = AISpawnPoint.position;

            Vector3 targetPosition = Vector3.zero;

            Vector2 playerHorizontalVelocity = new Vector2(player.velocity.x, player.velocity.z);

            float targetX = 0.0f;
            float targetZ = 0.0f;

            if (playerHorizontalVelocity.magnitude <= .2f)
            {
                Vector2 randomDirection = Random.insideUnitCircle.normalized;
                randomDirection.y = Mathf.Abs(randomDirection.y);

                Vector3 aimOffset = new Vector3(randomDirection.x, 0.0f, randomDirection.y); ;
                aimOffset = ((Quaternion.Euler(player.rotation.x, 0.0f, player.rotation.z) * aimOffset)) * 2.0f;
                aimOffset += aimOffset * Random.Range(0.0f, 2.0f);

                targetX = player.transform.position.x + aimOffset.x;
                targetZ = player.transform.position.z + aimOffset.z;
            }
            else
            {
                targetX = player.transform.position.x + player.velocity.x;
                targetZ = player.transform.position.z + player.velocity.z;
            }
            targetPosition = new Vector3(targetX, player.transform.position.y, targetZ);

            NavMeshHit hit;
            NavMesh.SamplePosition(owner.transform.position + targetPosition, out hit, 25.0f, NavMesh.AllAreas);
            targetPosition = hit.position;

            // Other Stuff...
            {
                //float heightMulti = 150;
                //float curveTime = 1.5f;

                //heightMulti = heightMulti - (startPosition - targetPosition).magnitude;
                //Vector3 peak = Utility.CreatePeak(startPosition, targetPosition, heightMulti);

                //float startTime = Time.time;
                ////while (time != 1.5f)
                //while (Time.time < startTime + curveTime) {
                //    float t = (startTime - Time.time) / curveTime;
                //    //time = Mathf.Clamp(time += Time.deltaTime, 0.0f, 1.5f);

                //    Vector3 newPosition = Utility.BezierCurve(startPosition, peak, targetPosition, t);
                //    //Vector3 newPosition = Utility.BezierCurve(startPosition, peak, targetPosition, time / 1.5f);

                //    // pos is NaN (?)
                //    enemy.transform.position = newPosition;

                //    yield return null;
                //}
                //enemy.transform.position = targetPosition;
            }

            float heightMulti = Mathf.Max(150.0f - (startPosition - targetPosition).magnitude, 50);
            Vector3 peak = Utility.CreatePeak(startPosition, targetPosition, heightMulti);

            while (time != 1.5f)
            {
                time = Mathf.Clamp(time += Time.deltaTime, 0.0f, 1.5f);
                Vector3 newPosition = Utility.BezierCurve(startPosition, peak, targetPosition, time / 1.5f);
                // pos is NaN
                float xPos = newPosition.x;
                if(float.IsNaN(xPos)) {
                    Debug.Break();
                }
                enemy.transform.position = newPosition;

                yield return null;
            }

            if (navAgent != null)
            {
                navAgent.enabled = true;
            }

            if (pot != null)
            {
                pot.enabled = true;
            }
            if (type == 0)
            {
                enemy.health.TakeDamage(DamageType.BASIC, 1.0f);
            }
            numberOfShotsLanded++;
            firing = false;
        }
    }

    public class Armored_Spawning : State
    {
        private GameObject Target = null;
        private Transform AISpawnPoint = null;
        private Boss1Pot boss = null;

        private byte numberOfBurstsFired = 0;
        private byte numberOfShotsLanded = 0;
        private bool firing = false;

        private float timer = 0;

        public override void Init(GameObject owner)
        {
            Target = GameObject.FindGameObjectWithTag("Player");

            boss = owner.GetComponent<Boss1Pot>();

            AISpawnPoint = boss.AISpawnPoint.transform;

            base.Init(owner);
        }

        public override void Enter()
        {
            numberOfBurstsFired = 0;
            numberOfShotsLanded = 0;
            firing = false;
            timer = 0;
        }

        public override void Exit()
        {
            numberOfBurstsFired = 0;
            numberOfShotsLanded = 0;
            firing = false;
            timer = 0;
        }

        Coroutine c = null;

        public override string Update()
        {
            if (!firing && numberOfBurstsFired != 3)
            {
                if (numberOfBurstsFired == 0)
                {
                    c = boss.StartCoroutine(LaunchBurst());
                }
                else if (!firing && timer >= 1.5f)
                {
                    c = boss.StartCoroutine(LaunchBurst());
                    timer = 0.0f;
                }
                else if (!firing)
                {
                    timer += Time.deltaTime;
                }
            }
            else if (numberOfBurstsFired == 3)
            {
                timer += Time.deltaTime;
                if (timer >= 2.0f)
                {
                    if (boss.GetArmor() <= 0)
                    {
                        timer = 0.0f;
                        return "Boss1Pot+Armored_Charging";
                    }
                    else
                    {
                        timer = 0.0f;
                        return "Boss1Pot+Armored_Shooting";
                    }
                }
            }
            else if ((Target.transform.position - owner.transform.position).magnitude >= 50.0f)
            {
                boss.StopCoroutine(c);
                return "Boss1Pot+Armored_Shooting";
            }
            return null;
        }

        IEnumerator LaunchBurst()
        {
            firing = true;
            for (int i = 0; i < 3; i++)
            {
                boss.StartCoroutine(LaunchPot());
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

            Player player = Player.Instance;
            int type;
            Enemy enemy = boss.SpawnRandomPot(out type);
            if (enemy == null)
            {
                yield break;
            }

            NavMeshAgent navAgent = enemy.GetComponent<NavMeshAgent>();
            if (navAgent != null)
            {
                navAgent.enabled = false;
            }

            Pot pot = enemy.GetComponent<Pot>();
            if (pot != null)
            {
                pot.enabled = false;
            }

            Vector3 startPosition = AISpawnPoint.position;
            Vector2 direction = Vector2.zero;


            NavMeshPath path = new NavMeshPath();

            Vector3 playerPosition = Vector3.zero;
            Vector3 targetPosition = Vector3.zero;

            do
            {
                //TODO: Remove the y-axis
                direction = Random.insideUnitCircle.normalized;
                Vector3 randomDirection = new Vector3(direction.x, 0.0f, direction.y);
                playerPosition = player.transform.position;
                targetPosition = new Vector3(playerPosition.x, 0.0f, playerPosition.z) + ((randomDirection * 10.0f) + (randomDirection * Random.Range(0.0f, 5.0f)));
                NavMeshHit hit;
                NavMesh.SamplePosition(owner.transform.position + targetPosition, out hit, 20.0f, NavMesh.AllAreas);
                targetPosition = hit.position;
            } while (!(NavMesh.CalculatePath(Vector3.zero, targetPosition, NavMesh.AllAreas, path)));

            float heightMulti = Mathf.Max(100.0f - (startPosition - targetPosition).magnitude, 50);
            Vector3 peak = Utility.CreatePeak(startPosition, targetPosition, heightMulti);

            while (time != 1.5f)
            {
                time = Mathf.Clamp(time += Time.deltaTime, 0.0f, 1.5f);
                Vector3 newPosition = Utility.BezierCurve(startPosition, peak, targetPosition, time / 1.5f);

                enemy.transform.position = newPosition;

                yield return null;
            }
            if (type == 0)
            {
                enemy.health.TakeDamage(DamageType.BASIC, 1.0f);
            }
            if (navAgent != null)
            {
                navAgent.enabled = true;
            }

            if (pot != null)
            {
                pot.enabled = true;
            }

            numberOfShotsLanded++;
        }
    }

    public class Armored_Charging : State
    {
        private GameObject target = null;
        private Boss1Pot armoredPot = null;
        private MonoBehaviour monoBehaviour = null;

        private bool charging = false;
        private bool charged = false;
        private float timer = 0.0f;
        //private float rotateSpeed = 30.0f;

        public override void Init(GameObject owner)
        {

            armoredPot = owner.GetComponent<Boss1Pot>();

            target = GameObject.FindGameObjectWithTag("Player");

            monoBehaviour = owner.GetComponent<MonoBehaviour>();

            base.Init(owner);
        }

        public override void Enter()
        {
            charging = false;
            charged = false;
            timer = 0.0f;
        }

        public override void Exit()
        {
            charging = false;
            charged = false;
            timer = 0.0f;
        }

        public override string Update()
        {
            if (!charging)
            {
                Transform transform = owner.transform;

                Vector3 targetPosition = target.transform.position;
                Vector3 targetOffset = (new Vector3(Player.Instance.velocity.x, 0.0f, Player.Instance.velocity.z)) * 1.5f;
                targetPosition = new Vector3(targetPosition.x, 0.0f, targetPosition.z) + targetOffset;

                Vector3 targetDirection = (targetPosition - transform.position).normalized;

                owner.transform.rotation = Quaternion.FromToRotation(transform.forward, targetDirection);

                monoBehaviour.StartCoroutine(Charge());
            }
            else if (charged && timer > 5.0f)
            {
                return "Boss1Pot+Armored_Spawning";
            }
            else
            {
                timer += Time.deltaTime;
            }
            return null;
        }

        IEnumerator Charge()
        {
            charging = true;

            NavMeshAgent navMeshAgent = owner.GetComponent<NavMeshAgent>();

            Vector3 targetPosition = target.transform.position;

            navMeshAgent.SetDestination(targetPosition);
            while ((navMeshAgent.destination - owner.transform.position).magnitude > .01f)
            {
                yield return null;
            }

            charging = false;
            charged = true;
        }
    }
    #endregion
}