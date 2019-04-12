using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour {

    public float Impulse = 10;

    protected new Rigidbody rigidbody;
    protected new Collider collider;

    // Start is called before the first frame update
    void Start() {
        collider = GetComponent<Collider>();
        if (collider == null) { collider = GetComponentInChildren<Collider>(true); }

        rigidbody = GetComponent<Rigidbody>();
        if (rigidbody == null) { rigidbody = GetComponentInChildren<Rigidbody>(true); }

        collider.enabled = false;

        rigidbody.AddForce(this.transform.forward * Impulse, ForceMode.Impulse);

        this.transform.parent = null;
    }

    private void OnTriggerEnter(Collider other) {
        print("Hit " + other.name);
    }
}
