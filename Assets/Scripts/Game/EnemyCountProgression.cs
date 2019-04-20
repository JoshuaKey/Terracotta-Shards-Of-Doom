using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCountProgression : MonoBehaviour {

    public int EnemyCount;

    // Start is called before the first frame update
    void Start() {
        EnemyManager.Instance.OnEnemyDeath += Check;

        this.gameObject.SetActive(false);

        PlayerHud.Instance.SetEnemyCount(0, EnemyCount);
    }

    public void Check() {
        if(EnemyManager.Instance.GetEnemiesKilled() >= EnemyCount) {
            print(this.name + " Has Spawned!");
            this.gameObject.SetActive(true);
        }

        PlayerHud.Instance.SetEnemyCount(EnemyManager.Instance.GetEnemiesKilled(), EnemyCount);
    }
}
