using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushBlock : MonoBehaviour {

    public float InteractForce = 5.0f;

    [Header("Components")]
    public Interactable interactable;
    public Rigidbody rb;

    // Start is called before the first frame update
    void Start() {
        if (interactable == null) { interactable = GetComponentInChildren<Interactable>(true); }
        if (rb == null) { rb = GetComponentInChildren<Rigidbody>(true); }

        interactable.OnInteract += PlayerPush;
    }

    public void PlayerPush() {
        Vector3 dir = Player.Instance.camera.transform.forward;
        dir.y = 0.0f;
        dir = dir.normalized;
        Push(dir * InteractForce);
    }
    public void Push(Vector3 force) {
        rb.AddForce(force, ForceMode.Impulse);
    }

    //private void OnTriggerEnter(Collider other) {
    //    print(other.name);
    //    if (other.CompareTag(Game.Instance.PlayerTag)) {
    //        PlayerPush();
    //    }
    //}
}
