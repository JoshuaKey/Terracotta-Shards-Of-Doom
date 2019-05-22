using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

    public Enemy PotPrefab;
    public Enemy ChargerPotPrefab;
    public Enemy RunnerPotPrefab;
    public Enemy ArmorPotPrefab;
    public Enemy HealthPotPrefab;

    public static EnemyManager Instance;

    [Space]
    public bool CollectAllPotsInScene = true;
    public Material HasCollectedMaterial;
    public List<Enemy> enemies = new List<Enemy>();
    private int enemiesKilled;

    public Action OnEnemyDeath;

    [Space]
    public EnemyProgression MainProgression;
    public bool ChildEnemiesToManager = false;

    // Start is called before the first frame update
    void Awake() {
        if (Instance != null) { Destroy(this.gameObject); return; }
        Instance = this;
    }
    void Start() {
        if (MainProgression == null) { MainProgression = GetComponentInChildren<EnemyProgression>(true); }

        if (CollectAllPotsInScene) {
            Pot[] potArray = GameObject.FindObjectsOfType<Pot>();
            foreach (Pot p in potArray) {
                Enemy e = p.GetComponent<Enemy>();
                enemies.Add(e);
            }
        }

        string levelName = LevelManager.Instance.GetLevelName();
        Game.Instance.playerStats.Levels[levelName].TotalPots = enemies.Count;
        for (int i = 0; i < enemies.Count; i++) {
            if (ChildEnemiesToManager) {
                enemies[i].transform.SetParent(this.transform, true);
            }
            enemies[i].health.OnDeath += EnemyDeath;
            string name = enemies[i].name;
            
            if (!Game.Instance.playerStats.Levels[levelName].CollectedPots.ContainsKey(name)) {
                Game.Instance.playerStats.Levels[levelName].CollectedPots[name] = false;
            }
            // If already collected, modify material
            if (Game.Instance.playerStats.Levels[levelName].CollectedPots[name]) {
                enemies[i].SetMaterial(HasCollectedMaterial);
            }
            //if (!LevelManager.Instance.Levels[levelName].CollectedPots.ContainsKey(name)) {
            //    LevelManager.Instance.Levels[levelName].CollectedPots[name] = false;
            //}
            //// If already collected, modify material
            //if (LevelManager.Instance.Levels[levelName].CollectedPots[name]) {
            //    enemies[i].SetMaterial(HasCollectedMaterial);
            //}
        }
    }

    private Enemy SpawnPrefab(Enemy prefab) {
        Enemy enemy = GameObject.Instantiate(prefab);
        enemy.transform.parent = this.transform;
        enemies.Add(enemy);
        return enemy;
    }

    public Enemy SpawnPot() {
        Enemy enemy = SpawnPrefab(PotPrefab);
        return enemy;
    }
    public Enemy SpawnHealthPot() {
        return SpawnPrefab(HealthPotPrefab);
    }
    public Enemy SpawnRunnerPot() {
        return SpawnPrefab(RunnerPotPrefab);
    }
    public Enemy SpawnChargerPot() {
        return SpawnPrefab(ChargerPotPrefab);
    }
    public Enemy SpawnArmorPot() {
        return SpawnPrefab(ArmorPotPrefab);
    }

    private void EnemyDeath() {
        enemiesKilled++;
        OnEnemyDeath?.Invoke();

        int index = enemies.FindIndex(x => x.health.IsDead());
        Enemy enemyKilled = enemies[index];
        string levelName = LevelManager.Instance.GetLevelName();
        Game.Instance.playerStats.Levels[levelName].CollectedPots[enemyKilled.name] = true;
        enemies.RemoveAt(index);
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
    public int GetEnemyCount() {
        return enemies.Count();
    }
    public int GetEnemiesKilled() {
        return enemiesKilled;
    }
}
