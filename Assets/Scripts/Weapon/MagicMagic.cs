using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMagic : AdvancedWeapon {
    [Header("Visuals")]
    public float MissileShootDelay = .2f;
    public float MagicMissileImpulse = 10f;
    public MagicMissile MagicMissilePrefab;
    public Transform[] MagicMissilePositions;
    //public MagicMissilePool magicMissilePool;

    protected new Rigidbody rigidbody;
    protected new Collider collider;

    private List<Enemy> enemyList = new List<Enemy>();
    private List<GameObject> targetList = new List<GameObject>();
    private List<MagicMissile> currMissiles = new List<MagicMissile>();
    private float charge = 0.0f;
    private float missileChargeInc = 0.0f;
    private int maxMissileCount = 0;
    private IEnumerator routine;

    private void Awake() {
        CanCharge = true;
        Type = DamageType.BASIC;
        OldWeaponName = "Magic";

        this.name = "MagicMagic";
    }

    void Start() {
        if (collider == null) { collider = GetComponentInChildren<Collider>(true); }
        if (rigidbody == null) { rigidbody = GetComponentInChildren<Rigidbody>(true); }

        collider.enabled = false;

        charge = 0.0f;
        maxMissileCount = MagicMissilePositions.Length;
        missileChargeInc = 1f / maxMissileCount;
    }

    private void OnEnable() {
        if (collider != null) {
            collider.enabled = false;
        }
    }
    private void OnDisable() {
        StopAllCoroutines();
        charge = 0.0f;
        if (collider != null) {
            collider.enabled = false;
        }

        enemyList.Clear();
        targetList.Clear();
        currMissiles.ForEach(x => Destroy(x.gameObject));
        currMissiles.Clear();
    }

    public override void Charge() {
        collider.enabled = true;

        // Attack Speed = Charge Time
        // Time to Full Charge 
        charge += Time.deltaTime / AttackSpeed;
        charge = Mathf.Min(charge, 1.0f);

        //  0  charge -> 0 count, index 0
        //  .2 charge -> 0 count, index 1
        //  .4 charge -> 1 count, index 2
        //  .6 charge -> 2 count, index 3
        //  .8 charge -> 3 count, index 4
        //  1  charge -> 4 count, index 5

        int index = Mathf.RoundToInt(charge / missileChargeInc);
        if (index > currMissiles.Count) {
            MagicMissile missile = Instantiate(MagicMissilePrefab, this.transform);
            missile.transform.position = MagicMissilePositions[index - 1].position;
            currMissiles.Add(missile);
        }
    }

    public override void Attack() {
        if (!CanAttack() || currMissiles.Count == 0 || routine != null) { return; }

        for (int i = 0; i < enemyList.Count; i++) {
            Enemy e = enemyList[i];
            if (e.health.IsDead()) {
                enemyList.RemoveAt(i);
                targetList.RemoveAt(i);
                i--;
                continue;
            }

            RaycastHit hit;
            int layerMask = LayerMask.GetMask("Default");
            Vector3 pos = e.transform.position + Vector3.up;
            if (Physics.Linecast(Player.Instance.camera.transform.position, pos, out hit, layerMask)) {
                if (hit.collider.gameObject != e.gameObject) {
                    enemyList.RemoveAt(i);
                    targetList.RemoveAt(i);
                    i--;
                    continue;
                }
            }
        }

        routine = Fire();
        StartCoroutine(routine);
    }

    private IEnumerator Fire() {
        for (int i = 0; i < currMissiles.Count; i++) {
            MagicMissile m = currMissiles[i];
            m.Target = targetList.Count == 0 ? null : targetList[i % targetList.Count];
            m.Impulse = m.Target == null ? Vector3.zero : Vector3.up * MagicMissileImpulse;
            m.LifeTime = 20f;
            m.Damage = this.Damage;
            m.Type = this.Type;
            m.Knockback = this.Knockback;
            m.RigidbodyKnockback = this.RigidbodyKnockback;
            m.Fire();

            yield return new WaitForSeconds(MissileShootDelay);
        }

        charge = 0.0f;
        currMissiles.Clear();
        enemyList.Clear();
        targetList.Clear();
        collider.enabled = false;
        routine = null;
    }

    private void OnTriggerEnter(Collider other) {
        if (!targetList.Contains(other.gameObject)) {
            Enemy enemy = other.GetComponentInChildren<Enemy>();
            if (enemy == null) { enemy = other.GetComponentInParent<Enemy>(); }
            if (enemy != null) {
                targetList.Add(other.gameObject);
                enemyList.Add(enemy);
            }
        }
    }

    public override bool CanSwap() {
        return true;
    }
}
