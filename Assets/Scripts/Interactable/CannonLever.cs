using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonLever : Lever {

    [Header("Components")]
    public Cannon cannon;

    [Header("Values (Leave to default for original)")]
    public Transform Target;
    public Transform Peak;
    public float ChargeTime;
    public float LeapTime;

    // Start is called before the first frame update
    protected override void Start() {
        base.Start();

        interactable.OnInteract += this.AlignCannon;
    }

    public void AlignCannon() {
        AudioManager.Instance.PlaySoundWithParent("lever", ESoundChannel.SFX, gameObject);
        cannon.Rotate(Target, Peak, ChargeTime, LeapTime);
        interactable.CanInteract = false;
    }
}
