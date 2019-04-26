using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magic : Weapon {

    [Header("Visuals")]
    public MagicMissile MagicMissilePrefab;
    public Transform[] MagicMissilePositions;
    //public MagicMissilePool magicMissilePool;

    protected new Rigidbody rigidbody;
    protected new Collider collider;

    private List<GameObject> enemyList = new List<GameObject>();
    private List<MagicMissile> currMissiles = new List<MagicMissile>();
    private float charge = 0.0f;
    private float missileChargeInc = 0.0f;
    private int maxMissileCount = 0;

    void Start() {
        if (collider == null) { collider = GetComponentInChildren<Collider>(true); }
        if (rigidbody == null) { rigidbody = GetComponentInChildren<Rigidbody>(true); }

        collider.enabled = false;

        CanCharge = true;
        Type = DamageType.BASIC;

        this.name = "Magic";

        charge = 0.0f;
        maxMissileCount = MagicMissilePositions.Length;
        missileChargeInc = 1f / maxMissileCount;
    }

    private void OnEnable() { }
    private void OnDisable() {
        StopAllCoroutines();
        charge = 0.0f;
    }

    public override void Charge() {
        collider.enabled = true; 

        // Attack Speed = Charge Time
        // Time to Full Charge 
        charge += Time.deltaTime / AttackSpeed;
        charge = Mathf.Min(charge, 1.0f);

        int index = (int)(charge / missileChargeInc);
        if(index >= currMissiles.Count) {
            MagicMissile missile = Instantiate(MagicMissilePrefab, this.transform);
            missile.transform.localPosition = MagicMissilePositions[index].localPosition;
            currMissiles.Add(missile);
        }
    }

    public override void Attack() {
        if (!CanAttack()) { return; }

        for(int i = 0; i < currMissiles.Count; i++) {
            MagicMissile m = currMissiles[i];
            m.Target =  i >= enemyList.Count ? null : enemyList[i];
            m.Impulse = Player.Instance.camera.transform.forward * 10f;
            m.LifeTime = 20f;
            m.Damage = this.Damage;
            m.Type = this.Type;
            m.Fire();
        }

        charge = 0.0f;
        currMissiles.Clear();
        enemyList.Clear();
        collider.enabled = false;
    }

    private void OnTriggerEnter(Collider other) {
        if (!enemyList.Contains(other.gameObject)) {
            Enemy enemy = other.GetComponent<Enemy>();
            if(enemy != null){
                enemyList.Add(other.gameObject);
            }
        }
    }

}
