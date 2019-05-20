using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ice : MonoBehaviour {

    public float IceAcceleration = .01f;

    private static float playerAccel = 0.0f;
    private static int isPlayerOnIce = 0;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(Game.Instance.PlayerTag)) {
            
            if(isPlayerOnIce == 0) {
                playerAccel = Player.Instance.AccelerationFactor;
                Player.Instance.AccelerationFactor = IceAcceleration;
            }         

            isPlayerOnIce++;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag(Game.Instance.PlayerTag)) {
            isPlayerOnIce--;

            if (isPlayerOnIce == 0) {
                Player.Instance.AccelerationFactor = playerAccel;
            }
        }
    }

}
