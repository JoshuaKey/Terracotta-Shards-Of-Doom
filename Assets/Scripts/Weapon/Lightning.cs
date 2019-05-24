using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : MonoBehaviour {

    [Header("Life")]
    public float LifeTime = 20f;
    public float Range = 2;
    public float Delay = 2;

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
        //LevelManager.Instance.MoveToScene(this.gameObject);

        DamageEnemy(enemy, c);

        if (this.gameObject.activeInHierarchy) {
            StartCoroutine(Die());
            StartCoroutine(Seek());
        }
    }

    private IEnumerator Die() {
        startLife = Time.time;

        yield return new WaitForSeconds(LifeTime);

        Destroy(this.gameObject);
    }
    
    private IEnumerator Seek() {
        yield return new WaitForSeconds(Delay);

        bool enemyInRange = false;

        Collider[] colliders = Physics.OverlapSphere(this.transform.position, Range, layerMask);
        print(colliders.Length);
        foreach (Collider c in colliders) {
            Enemy enemy = c.GetComponentInChildren<Enemy>();
            if (enemy == null) { enemy = c.GetComponentInParent<Enemy>(); }
            if (enemy != null && enemy != currEnemy) {
                DamageEnemy(enemy, c);
                enemyInRange = true;
                print(enemy.name);
                
                break;
            }
        }
        print(enemyInRange);


        if (!enemyInRange) {
            print("Destroying Lightning");

            Destroy(this.gameObject);
        } else if(this.gameObject.activeInHierarchy) {
            StartCoroutine(Seek());
        }       
    }

    private void DamageEnemy(Enemy enemy, Collider c) {
        
        float damage = enemy.health.TakeDamage(this.Type, this.Damage);
        bool isDead = enemy.health.IsDead();

        if (damage > 0 && isDead) {
            Vector3 forward = this.transform.forward;
            forward.y = 0.0f;
            forward = forward.normalized;
            enemy.Explode(forward * ExplosionKnockback, this.transform.position);
        } 

        this.transform.parent = enemy.transform;
        this.transform.position = c.bounds.center;
        this.transform.rotation = Quaternion.Euler(-90, 0, 0);

        currEnemy = enemy;
    }

}
