using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicStart : MonoBehaviour {

    // Start is called before the first frame update
    void Start() {
        
    }

    private void Update() {
        AudioManager.Instance.PlaySceneMusic(SceneManager.GetActiveScene().name);
        this.enabled = false;
    }

}
