using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Teleporter : MonoBehaviour {
    public string previousScene = "";
    public string sceneName;
    public bool HasPlayerCompletedLevel;

    private SoundClip soundClip;

    private void Start() {
        if(previousScene != null && previousScene != "") {
            string levelName = LevelManager.Instance.GetLevelName();
            LevelData level;
            if (!LevelManager.Instance.Levels.TryGetValue(previousScene, out level) || !level.IsCompleted) {
                print("Teleporter " + this.name + " Shutting off. Level " + previousScene + " is not completed");
                this.gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(Game.Instance.PlayerTag)) {
            if (HasPlayerCompletedLevel) {
                string levelName = LevelManager.Instance.GetLevelName();
                LevelManager.Instance.Levels[levelName].IsCompleted = true;
				Game.Instance.SavePlayerStats();
            }

            LevelManager.Instance.LoadScene(sceneName);
        }
    }

    public void StartSound() => soundClip = AudioManager.Instance.PlaySoundWithParent("active_portal", ESoundChannel.SFX, gameObject, true);

    private void OnDisable() => soundClip.Stop();
}
