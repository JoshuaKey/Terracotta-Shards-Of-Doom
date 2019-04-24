using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossBow : Weapon {

    private List<GameObject> enemiesHit = new List<GameObject>();

    void Start() {
        CanCharge = false;
        Type = DamageType.BASIC;

        this.name = "Crossbow";
    }

    private void OnDisable() {
        StopAllCoroutines();
    }

    public override void Attack() {
        if (!CanAttack()) { return; }

        base.Attack();
        StartCoroutine(Shoot());
    }

    private IEnumerator Shoot() {
        yield return null;
    }
}
