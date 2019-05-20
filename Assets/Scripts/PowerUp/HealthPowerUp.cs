using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPowerUp : PowerUp {

    public float HealthGain = 1f;

    public override void GainPowerUp() {
        Player player = Player.Instance;
        player.health.Heal(HealthGain);
    }
}
