using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Teleporter : MonoBehaviour {
    public string sceneName;
    public bool HasPlayerCompletedLevel;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(Game.Instance.PlayerTag)) {
            if (HasPlayerCompletedLevel) {
                string levelName = LevelManager.Instance.GetLevelName();
                Game.Instance.playerStats.Levels[levelName].IsCompleted = true;
            }

            LevelManager.Instance.LoadScene(sceneName);
        }
    }
}
