using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

    public string PersistentSceneName = "Persistent";
    public string StartingSceneName = "Hub";

    public static LevelManager Instance;

    // Start is called before the first frame update
    void Start() {
        if(Instance != null) { Destroy(this.gameObject); return; }

        if (SceneManager.GetActiveScene().name == PersistentSceneName) {
            SceneManager.LoadScene(StartingSceneName);
        } 
        Instance = this;

        string levelName = GetLevelName();
        print("Level: " + levelName);
        if (!Game.Instance.playerStats.Levels.ContainsKey(levelName)) {
            Game.Instance.playerStats.Levels[levelName] = new LevelData();
            print("Here");
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        print(scene.name + " was Loaded!");

        Player.Instance.gameObject.SetActive(false);

        CheckPointSystem.Instance.LoadStartPoint();
        Player.Instance.health.Reset();
        PlayerHud.Instance.SetPlayerHealthBar(1.0f);

        Player.Instance.gameObject.SetActive(true);

        if (!Game.Instance.playerStats.Levels.ContainsKey(scene.name)) {
            Game.Instance.playerStats.Levels[scene.name] = new LevelData();
        }
    }

    public void MoveToScene(GameObject obj) {
        SceneManager.MoveGameObjectToScene(obj, SceneManager.GetActiveScene());
    }

    public void RestartLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);    
    }
    public void LoadLevel(int world, int level) {
        SceneManager.LoadScene(world + "-" + level);
    }
    public void LoadHub() {
        SceneManager.LoadScene("Hub");    
    }
    public void LoadScene(string scene) {
        SceneManager.LoadScene(scene);
    }

    public string GetLevelName() {
        return SceneManager.GetActiveScene().name;
    }
}