using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaShot : PoolObject {

    public float ShotTime = 1.0f;

    public new Rigidbody rigidbody;
    public new Collider collider;
    public Attack attack;
    public new LineRenderer renderer;
    public Transform Target;

    // Start is called before the first frame update
    protected override void Start() {
        if (collider == null) { collider = GetComponentInChildren<Collider>(true); }

        if (rigidbody == null) { rigidbody = GetComponentInChildren<Rigidbody>(true); }

        if (attack == null) { attack = GetComponentInChildren<Attack>(true); }

        //collider.enabled = false;
        //rigidbody.isKinematic = true;
        //attack.isAttacking = false;

        attack.OnAttack += OnAttack;
    }

    protected override void OnDisable() {
        // Death Particle Effect
        if(Target != null) {
            Destroy(Target.gameObject);
        }  
    }

    public void Fire(Vector3 dest, Vector3 peak) {
        collider.enabled = true;
        rigidbody.isKinematic = false;
        rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        attack.isAttacking = true;

        this.transform.parent = null;
        LevelManager.Instance.MoveToScene(this.gameObject);

        Target.gameObject.SetActive(true);
        Target.parent = null;
        Vector3 offset = Vector3.zero;// Random.insideUnitCircle * Random.value * Target.localScale.x;
        offset.z = offset.y;
        offset.y = 0;
        offset += Vector3.down * .9f;
        Target.position = dest + offset;

        //renderer.positionCount = 11;
        //Vector3[] positions = new Vector3[renderer.positionCount];
        //for(int i = 0; i < renderer.positionCount; i++) {
        //    float t = (float)i / (renderer.positionCount - 1);
        //    positions[i] = Interpolation.BezierCurve(this.transform.position, peak, dest, t);
        //}
        //renderer.SetPositions(positions);
        //renderer.enabled = true;

        StartCoroutine(Shoot(this.transform.position, dest, peak));
    }

    private IEnumerator Shoot(Vector3 start, Vector3 dest, Vector3 peak) {
        float startTime = Time.time;
        while (Time.time < startTime + ShotTime) {
            float t = (Time.time - startTime) / ShotTime;

            Vector3 pos = Utility.BezierCurve(start, peak, dest, t);

            this.transform.position = pos;

            yield return null;
        }
        this.transform.position = dest;

        Destroy(this.gameObject);
    }

    private void OnAttack() {
        Destroy(this.gameObject);
    }
}
