using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour {

    public string PlayerTag;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(PlayerTag)) {
            CheckPointSystem.Instance.SetLastCheckpoint(this);
        }
    }
}
