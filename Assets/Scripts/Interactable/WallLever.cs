using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallLever : Lever {

    [Header("Wall")]
    public Wall wall;

    protected override void Start() {
        base.Start();

        interactable.OnInteract += this.MoveWall;
    }

    public void MoveWall() {
        interactable.CanInteract = false;
        AudioManager.Instance.PlaySoundWithParent("lever", ESoundChannel.SFX, gameObject);
        wall.Open();
    }
}
