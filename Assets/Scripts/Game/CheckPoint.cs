using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour {

    public string PlayerTag;

    private void Start() {
        this.gameObject.layer = LayerMask.NameToLayer("Trigger");
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(PlayerTag)) {
            CheckPointSystem.Instance.SetLastCheckpoint(this);
        }
    }
}
