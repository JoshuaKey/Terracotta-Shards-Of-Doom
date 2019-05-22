using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snowball : MonoBehaviour {

    public float Acceleration = 100;
    public float SpeedScaleRatio = .2f;
    public new Rigidbody rigidbody;
    public TargetProjectile projectile;
    public ParticleSystem ExplosionEffect;

    private Vector3 direction;

    private void Start() {
        projectile.OnFire += Fire;
        direction = this.transform.forward;
        print("Dir " + direction);

    }

    private void Fire(TargetProjectile proj) {
        
        direction = this.transform.forward;
        print("Dir " + direction);
    }

    private void OnDestroy() {
        ParticleSystem.MainModule mainMod = ExplosionEffect.main;
        mainMod.maxParticles = (int)(this.transform.localScale.x * 100);

        ParticleSystem.EmissionModule emissionModule;
        emissionModule = ExplosionEffect.emission;
        emissionModule.rateOverTime = 0;

        ParticleSystem.Burst burst = emissionModule.GetBurst(0);
        burst.count = mainMod.maxParticles;

        ExplosionEffect.transform.parent = null;
        ExplosionEffect.Play();
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (projectile.HasFired()) {
            Vector3 force = direction * Acceleration;

            rigidbody.AddForce(force * Time.deltaTime, ForceMode.Force);

            float magnitude = Mathf.Max(1.0f, rigidbody.velocity.magnitude * SpeedScaleRatio);
            float scale = Mathf.Max(this.transform.localScale.x, magnitude);  

            this.transform.localScale = Vector3.one * scale;
        }
    }

    private void OnTriggerEnter(Collider other) {
        Snowball snowball = other.GetComponentInChildren<Snowball>();
        if(snowball != null) {
            Destroy(this.gameObject);
        }
    }
}
