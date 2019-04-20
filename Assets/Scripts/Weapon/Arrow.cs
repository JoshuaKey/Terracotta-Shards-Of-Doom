using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour {

    public int Pierce = 1;
    public float Impulse = 10;
    public float Damage = 0f;
    public DamageType Type;

    protected new Rigidbody rigidbody;
    protected new Collider collider;

    private int currPierce = 0;

    // Start is called before the first frame update
    void Start() {
        collider = GetComponent<Collider>();
        if (collider == null) { collider = GetComponentInChildren<Collider>(true); }

        rigidbody = GetComponent<Rigidbody>();
        if (rigidbody == null) { rigidbody = GetComponentInChildren<Rigidbody>(true); }

        collider.enabled = false;
        rigidbody.isKinematic = true;
    }

    private void FixedUpdate() {
        if (!rigidbody.isKinematic) {
            Vector3 start = this.transform.position;
            Vector3 end = this.transform.position + rigidbody.velocity * Time.deltaTime;
            RaycastHit hit;
            if (Physics.Linecast(start, end, out hit)) {
                OnTriggerEnter(hit.collider);
            }
        }
    }

    public void Fire() {
        collider.enabled = true;
        rigidbody.isKinematic = false;

        rigidbody.AddForce(this.transform.forward * Impulse, ForceMode.Impulse);

        this.transform.parent = null;
        LevelManager.Instance.MoveToScene(this.gameObject);

        FixedUpdate();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.isTrigger) { return; }

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null) {
            enemy.health.TakeDamage(this.Type, this.Damage);
            //OnEnemyHit?.Invoke(enemy);
        }

        currPierce++;
        if(currPierce == Pierce) {
            this.gameObject.SetActive(false);
            //rigidbody.velocity = Vector3.zero;
            //this.transform.SetParent(other.transform, true);
        }
    }
}
