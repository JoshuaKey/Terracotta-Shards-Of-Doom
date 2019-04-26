using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour {

    public Health health;
    
    void Start() {
        if (health == null) { health = GetComponentInChildren<Health>(true); }

        // Assume player killed Pot...
        health.OnDeath += GainPowerUp;
    }

    public virtual void GainPowerUp() { print("Power Up!!!!"); }
}
