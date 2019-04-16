using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    public string PlayerTag = "Player";

    public static Game Instance;

    // Start is called before the first frame update
    void Start() {
        if(Instance != null) { Destroy(this.gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(this);
    }



}
