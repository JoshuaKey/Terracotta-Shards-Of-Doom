using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour {

    public Vector3 WindForce;

    private void OnTriggerStay(Collider other) {
        if (other.CompareTag(Game.Instance.PlayerTag)) {

            Player.Instance.velocity += WindForce * Time.deltaTime;
        }
    }
}
