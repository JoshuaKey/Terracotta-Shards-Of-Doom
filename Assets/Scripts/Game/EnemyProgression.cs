using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProgression : MonoBehaviour {

    public Enemy[] Enemies;

    private int enemiesKilled = 0;
    private int enemiesNeeded = 0;

    // Start is called before the first frame update
    void Start() {
        for(int i = 0; i < Enemies.Length; i++) {
            Enemies[i].health.OnDeath += Check;
        }
        enemiesNeeded = Enemies.Length;

        this.gameObject.SetActive(false);

        PlayerHud.Instance.SetEnemyCount(enemiesKilled, enemiesNeeded);
    }

    private void Check() {
        enemiesKilled++;
        if(enemiesKilled >= enemiesNeeded) {
            this.gameObject.SetActive(true);
        }

        PlayerHud.Instance.SetEnemyCount(enemiesKilled, enemiesNeeded);
    }

}
