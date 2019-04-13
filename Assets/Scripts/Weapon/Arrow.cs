using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour {

    public float Impulse = 10;
    public float Damage = 0f;
    public DamageType Type;

    protected new Rigidbody rigidbody;
    protected new Collider collider;

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
        Vector3 start = this.transform.position;
        Vector3 end = this.transform.position + rigidbody.velocity * Time.deltaTime;
        RaycastHit hit;
        if (Physics.Linecast(start, end, out hit)) {
            OnTriggerEnter(hit.collider);
        }

        // Debug
        Debug.DrawRay(this.transform.position, rigidbody.velocity * Time.deltaTime, Color.cyan, 0.3f);
    }

    public void Fire() {
        collider.enabled = true;
        rigidbody.isKinematic = false;

        rigidbody.AddForce(this.transform.forward * Impulse, ForceMode.Impulse);

        this.transform.parent = null;
    }

    private void OnTriggerEnter(Collider other) {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null) {
            enemy.health.TakeDamage(this.Type, this.Damage);
            //OnEnemyHit?.Invoke(enemy);
        }
        this.gameObject.SetActive(false);
    }
}
