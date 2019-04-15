using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointSystem : MonoBehaviour {

    public static CheckPointSystem Instance;

    //[HideInInspector]
    public CheckPoint StartingPoint;
    //[HideInInspector]
    public CheckPoint LastCheckPoint;

    private CheckPoint[] checkpoints;

    // Start is called before the first frame update
    void Start() {
        // Destroy Old Instance
        if (Instance != null) { Destroy(Instance.gameObject); }
        Instance = this;

        checkpoints = GetComponentsInChildren<CheckPoint>();

        StartingPoint = checkpoints[0];
        LastCheckPoint = StartingPoint;

        print("Starting Point: " + StartingPoint.transform.position);
    }
}
