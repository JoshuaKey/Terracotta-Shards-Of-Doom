using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialPot : MonoBehaviour {

    public SpecialPot PreviousPot;

    [Header("Components")]
    public Enemy enemy;

    void Start() {
        if(enemy == null) { enemy = GetComponentInChildren<Enemy>(); }

        if(PreviousPot != null) {
            this.gameObject.SetActive(false);

            PreviousPot.enemy.health.OnDeath += Spawn;
        } else {
            this.gameObject.SetActive(true);
        }
    }

    private void Spawn() {
        this.gameObject.SetActive(true);
    }
}
