using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetReceiver : MonoBehaviour {

    // Which Objects are the "Target"
    public string TargetProjectileTag = "TargetProjectile";

    // What happens when we "receive" the Target
    public Action<TargetReceiver, GameObject> OnReceive;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(TargetProjectileTag)) {
            OnReceive?.Invoke(this, other.gameObject);
        } 
    }
}
