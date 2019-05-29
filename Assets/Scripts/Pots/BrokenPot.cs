using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenPot : MonoBehaviour {

    //private Rigidbody[] pieces;
    //private MeshRenderer[] renderers;
    public float PieceDisappearTime = .5f;
    private List<Rigidbody> pieces = new List<Rigidbody>();
    private List<MeshRenderer> renderers = new List<MeshRenderer>();

    private void Awake() {
        //pieces = GetComponentsInChildren<Rigidbody>(true);
        //renderers = GetComponentsInChildren<MeshRenderer>(true);   

        pieces.AddRange(GetComponentsInChildren<Rigidbody>(true));
        renderers.AddRange(GetComponentsInChildren<MeshRenderer>(true));
    }

    void Start() {
        this.gameObject.SetActive(false);
    }

    private void OnEnable() {
        StartCoroutine(Disappear());
    }

    private void OnDisable() {
        StopAllCoroutines();
    }

    private IEnumerator Disappear() {
        WaitForSeconds wait = new WaitForSeconds(PieceDisappearTime);

        do {

            yield return wait;

            int index = Random.Range(0, pieces.Count);
            GameObject obj = pieces[index].gameObject;
            Destroy(obj);
            pieces.RemoveAt(index);
            renderers.RemoveAt(index);

        } while (pieces.Count != 0);

        Destroy(this.gameObject);
    }

    public void Explode(Vector3 force, Vector3 pos) {
        foreach(Rigidbody rb in pieces) {
            rb.AddExplosionForce(force.magnitude, pos, 10.0f, 1.0f, ForceMode.Impulse);
        }
        //StartCoroutine(Disappear());
    }

    public void Explode(float force, Vector3 pos) {
        foreach (Rigidbody rb in pieces) {
            rb.AddExplosionForce(force, pos, 10.0f, 1.0f, ForceMode.Impulse);
        }
        //StartCoroutine(Disappear());
    }

    public void SetMaterial(Material m) {
        foreach(MeshRenderer r in renderers) {
            r.material = m;
        }
    }
}
