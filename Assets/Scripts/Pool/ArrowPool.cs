using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPool : Pool<Arrow> {

    public static ArrowPool Instance;

    private void Awake() {
        if (Instance != null) { Destroy(this.gameObject); return; }
        Instance = this;
    }

}
