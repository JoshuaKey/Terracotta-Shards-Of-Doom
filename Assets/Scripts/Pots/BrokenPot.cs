using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenPot : MonoBehaviour {

    private Rigidbody[] pieces;

    void Start() {
        pieces = GetComponentsInChildren<Rigidbody>();

        this.gameObject.SetActive(false);
    }

    public void Explode() {
        
        // Put Explodey Logic here
    }
}
