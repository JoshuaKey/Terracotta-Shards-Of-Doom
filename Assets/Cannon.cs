using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour {

    // A cannon can be interacted with
    // When Interacted, the Player will align with the Barrel
    // Then the player will shoot, in a specific direction and velocity

    public Vector3 Rotation;
    public Vector3 Speed;

    private Interactable interactable;

    // Start is called before the first frame update
    void Start() {
        interactable = GetComponent<Interactable>();
        if (interactable == null) { interactable = GetComponentInChildren<Interactable>(); }

        interactable.Subscribe(FirePlayer);
    }

    public void Align() {
        // Dont care right now...
    }

    public void FirePlayer() {
        Player player = Player.Instance;
        //StartCoroutine(Shoot(player));
    }

    //public IEnumerator Shoot() {

    //}

}
