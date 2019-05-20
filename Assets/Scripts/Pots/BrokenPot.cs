using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenPot : MonoBehaviour {

    private Rigidbody[] pieces;
    private MeshRenderer[] renderers;

    private void Awake() {
        pieces = GetComponentsInChildren<Rigidbody>(true);
        renderers = GetComponentsInChildren<MeshRenderer>(true);     
    }

    void Start() {
        this.gameObject.SetActive(false);
    }

    public void Explode(Vector3 force, Vector3 pos) {
        foreach(Rigidbody rb in pieces) {
            rb.AddExplosionForce(force.magnitude, pos, 10.0f, 1.0f, ForceMode.Impulse);
        }
    }

    public void Explode(float force, Vector3 pos) {
        foreach (Rigidbody rb in pieces) {
            rb.AddExplosionForce(force, pos, 10.0f, 1.0f, ForceMode.Impulse);
        }
    }

    public void SetMaterial(Material m) {
        foreach(MeshRenderer r in renderers) {
            r.material = m;
        }
    }
}
