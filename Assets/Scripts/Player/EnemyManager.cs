using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

    //public Enemy PotPrefab;
    //public Enemy ChargerPotPrefab;
    //public Enemy RunnerPotPrefab;
    //public Enemy ArmorPotPrefab;

    public static EnemyManager Instance;

    private List<Enemy> enemies = new List<Enemy>();
    private int enemiesKilled;

    public Action OnEnemyDeath;

    // Start is called before the first frame update
    void Awake() {
        if (Instance != null) { Destroy(this.gameObject); return; }
        Instance = this;
    }
    void Start() {
        Enemy[] enemiesArray = GameObject.FindObjectsOfType<Enemy>();

        enemies.AddRange(enemiesArray);

        for (int i = 0; i < enemies.Count; i++) {
            enemies[i].transform.SetParent(this.transform, true);
            enemies[i].health.OnDeath += EnemyDeath;
        }
    }

    private void EnemyDeath() {
        enemiesKilled++;
        OnEnemyDeath?.Invoke();
        print(EnemyManager.Instance.GetEnemiesKilled() + " Enemies Killed");
    }

    public Enemy GetEnemy(int index) {
        if (index < 0 || index >= enemies.Count) { return null; }
        return enemies[index];
    }
    public int GetEnemiesKilled() {
        return enemiesKilled;
    }
}
