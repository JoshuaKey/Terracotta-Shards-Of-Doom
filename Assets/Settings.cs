using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour {
    public string InputConfigurationFile;

    public static Settings Instance;

    void Awake() {
        if (Instance != null) { Destroy(this.gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(this);
    }

    public void Save(string file) {
        print("Saving Settings to " + file);
    }
    public void Load(string file) {
        print("Loading Settings to " + file);
    }
}
