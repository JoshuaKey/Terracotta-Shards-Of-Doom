using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public EnemyProgression MainProgression;

    // Start is called before the first frame update
    void Awake() {
        if (Instance != null) { Destroy(this.gameObject); return; }
        Instance = this;
    }
    void Start() {
        if (MainProgression == null) { MainProgression = GetComponentInChildren<EnemyProgression>(true); }

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

    public Enemy GetClosestEnemy(Vector3 pos) {
        var activeEnemies = enemies.Where(e => e.gameObject.activeInHierarchy);
        var distances = enemies.Select(e => (e.transform.position - pos).sqrMagnitude);

        float currMin = float.MaxValue;
        int currIndex = -1;
        for(int i = 0; i < distances.Count(); i++) {
            if(distances.ElementAt(i) < currMin) {
                currMin = distances.ElementAt(i);
                currIndex = i;
            }
        }

        return activeEnemies.ElementAt(currIndex);
    }
    public Enemy GetEnemy(int index) {
        if (index < 0 || index >= enemies.Count) { return null; }
        return enemies[index];
    }
    public int GetEnemiesKilled() {
        return enemiesKilled;
    }
}
