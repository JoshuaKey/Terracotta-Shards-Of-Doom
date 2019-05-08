using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererTrigger : MonoBehaviour
{
    LineRenderer lineyBoi;

    private void Start() {
        lineyBoi = this.GetComponent<LineRenderer>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(Game.Instance.PlayerTag)) {
            lineyBoi.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag(Game.Instance.PlayerTag)) {
            lineyBoi.enabled = false;
        }
    }
}
