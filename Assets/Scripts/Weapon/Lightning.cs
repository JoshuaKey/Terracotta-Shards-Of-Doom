using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : MonoBehaviour {

    [Header("Life")]
    public float LifeTime = 20f;
    public float Range = 2;
    public float DamageDelay = 2; // Delay in Damage Ticks on 1 enemy
    public float Delay = .333f;

    [Header("Damage")]
    public float Damage = 0f;
    public DamageType Type;
    public float ExplosionKnockback = 5.0f;

    [Header("Visual")]
    public GameObject LightningEffect;

    private float startLife;
    private int layerMask;

    private Enemy currEnemy;

    protected void Start() {
        layerMask = PhysicsCollisionMatrix.Instance.MaskForLayer(this.gameObject.layer);

        Type = DamageType.LIGHTNING;
    }

    public void Fire(Enemy enemy, Collider c) {
        LightningEffect.transform.localScale = new Vector3(5, 5, 2);
        this.transform.parent = null;
        LevelManager.Instance.MoveToScene(this.gameObject);

        // I want to damage the current enemy
        // If it dies, I wait a frame, then jump to the next
        // If it doesn't I wait 

        StartCoroutine(Die());

        StartCoroutine(DamageEnemy(enemy, c));

        //if (this.gameObject.activeInHierarchy) {           
        //    StartCoroutine(Seek());
        //}
    }

    private IEnumerator Die() {
        startLife = Time.time;

        yield return new WaitForSeconds(LifeTime);

        Destroy(this.gameObject);
    }
    
    private void Seek() {
        Enemy enemy = null;
        Collider collider = null;

        Collider[] colliders = Physics.OverlapSphere(this.transform.position, Range, layerMask);
        foreach (Collider c in colliders) {
            Enemy e = c.GetComponentInChildren<Enemy>();
            if (e == null) { e = c.GetComponentInParent<Enemy>(); }
            if (e != null && e != currEnemy) {
                enemy = e;
                collider = c;
                break;
            }
        }
        print(colliders.Length);


        if (enemy == null) {
            Destroy(this.gameObject);
        } else {
            StartCoroutine(DamageEnemy(enemy, collider));
        }
    }

    private IEnumerator DamageEnemy(Enemy enemy, Collider c) {
        
        float damage = enemy.health.TakeDamage(this.Type, this.Damage);
        bool isDead = enemy.health.IsDead();

        if (damage > 0 && isDead) {
            Vector3 forward = this.transform.forward;
            forward.y = 0.0f;
            forward = forward.normalized;
            enemy.Explode(forward * ExplosionKnockback, this.transform.position);
        } 

        this.transform.position = c.bounds.center;
        this.transform.rotation = Quaternion.Euler(-90, 0, 0);

        currEnemy = enemy;

        if (!isDead) {
            this.transform.parent = enemy.transform;
            yield return new WaitForSeconds(DamageDelay);
        } else {
            this.transform.parent = null;
            yield return new WaitForSeconds(Delay);
        }
        Seek();
    }

}
