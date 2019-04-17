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
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        print(scene.name + " was Loaded!");
        CheckPointSystem.Instance.LoadStartPoint();
        Player.Instance.health.Heal(Player.Instance.health.MaxHealth);
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
}