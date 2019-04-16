using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointSystem : MonoBehaviour {

    public static CheckPointSystem Instance;

    private CheckPoint StartingPoint;
    private CheckPoint LastCheckPoint;

    private CheckPoint[] checkpoints;

    // Start is called before the first frame update
    void Awake() {
        // Destroy Old Instance
        if (Instance != null) { Destroy(Instance.gameObject); }
        Instance = this;

        checkpoints = GetComponentsInChildren<CheckPoint>();

        StartingPoint = checkpoints[0];
        LastCheckPoint = StartingPoint;

        print("Starting Point: " + StartingPoint.transform.position);
    }

    public void SetLastCheckpoint(CheckPoint checkpoint) {
        LastCheckPoint = checkpoint;
    }
    public void LoadlastCheckpoint() {
        LoadCheckPoint(LastCheckPoint);
    }
    public void LoadStartPoint() {
        LoadCheckPoint(StartingPoint);
    }

    public void LoadCheckPoint(CheckPoint checkpoint) {
        Player player = Player.Instance;
        player.transform.position = checkpoint.transform.position;
        player.velocity = Vector3.zero;
        player.LookTowards(checkpoint.transform.forward);
    }
}
