using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour {

    private void OnTriggerEnter(Collider other) {
        //print("Death: " + other.name);
        if (other.CompareTag(Game.Instance.PlayerTag)) {
            CheckPointSystem.Instance.LoadlastCheckpoint();
            Player.Instance.CanMove = false;
        } else {
            Health health = other.GetComponentInChildren<Health>(true);
            if (health == null) { health = other.GetComponentInParent<Health>(); }

            if (health != null) {
                health.TakeDamage(DamageType.TRUE, health.CurrentHealth);
            } else {
                other.gameObject.SetActive(false);
            }           
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag(Game.Instance.PlayerTag)) {
            Player.Instance.CanMove = true;
        }       
    }
}
