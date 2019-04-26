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
    public IEnumerator RotateHandle(Quaternion endRot, float time) {
        {
            Quaternion startRot = LeverHandle.transform.rotation;
            float startTime = Time.time;
            while (Time.time < startTime + time) {
                float t = (Time.time - startTime) / time;

                Quaternion rot = Quaternion.SlerpUnclamped(startRot, endRot, t);

                LeverHandle.transform.rotation = rot;
                yield return null;
            }
            LeverHandle.transform.rotation = endRot;
        }
    }

}
