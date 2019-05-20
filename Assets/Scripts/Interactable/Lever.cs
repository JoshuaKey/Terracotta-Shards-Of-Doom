using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour {

    [Header("Components")]
    public Interactable interactable;

    [Header("Angle")]
    public GameObject LeverBase;
    public GameObject LeverHandle;
    public Vector3 HandleAngle = new Vector3(0, 0, 40);
    public float RotateTime = 0.5f;

    // Start is called before the first frame update
    protected virtual void Start() {
        if (interactable == null) { interactable = GetComponentInChildren<Interactable>(true); }

        interactable.OnInteract += this.Rotate;
    }

    public void Rotate() {
        StartCoroutine(RotateHandle(Quaternion.Euler(HandleAngle), RotateTime));
    }
    public IEnumerator RotateHandle(Quaternion endRot, float rotTime) {
        Quaternion startRot = LeverHandle.transform.localRotation;
        float startTime = Time.time;
        while (Time.time < startTime + rotTime) {
            float t = (Time.time - startTime) / rotTime;

            Quaternion rot = Quaternion.SlerpUnclamped(startRot, endRot, t);

            LeverHandle.transform.localRotation = rot;
            yield return null;
        }
        LeverHandle.transform.localRotation = endRot; 
    }

}
