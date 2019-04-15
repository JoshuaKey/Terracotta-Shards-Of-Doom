using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour {

    public string PlayerTag;

    private void OnTriggerEnter(Collider other) {
        print("Death: " + other.name);
        if (other.CompareTag(PlayerTag)) {
            Player player = Player.Instance;
            CheckPoint checkpoint = CheckPointSystem.Instance.LastCheckPoint;

            player.transform.position = checkpoint.transform.position;
            player.LookTowards(checkpoint.transform.forward);
            player.CanMove = false;
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
        if (other.CompareTag(PlayerTag)) {
            Player player = Player.Instance;
            player.CanMove = true;
        }       
    }
}
