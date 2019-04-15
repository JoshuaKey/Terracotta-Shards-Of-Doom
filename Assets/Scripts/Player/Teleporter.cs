using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Teleporter : MonoBehaviour {
    public string sceneName;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(Game.Instance.PlayerTag)) {
            LevelManager.Instance.LoadScene(sceneName);
        }
    }
}
