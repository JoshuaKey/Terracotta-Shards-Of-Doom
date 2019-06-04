using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snowball : MonoBehaviour {

    public float Acceleration = 100;
    public float SpeedScaleRatio = .2f;
    public new Rigidbody rigidbody;
    public TargetProjectile projectile;
    public ParticleSystem ExplosionEffect;
    public ParticleSystem HitEffect;

    private Vector3 direction;

    private void Start() {
        projectile.OnFire += Fire;
        direction = this.transform.forward;
    }

    private void Fire(TargetProjectile proj) {

        direction = this.transform.forward;
        HitEffect.Play();
    }

    private void OnDestroy() {
        ParticleSystem.MainModule mainMod = ExplosionEffect.main;
        mainMod.maxParticles = (int)(this.transform.localScale.x * 10);
        //mainMod.startSpeed = this.transform.localScale.x;

        ParticleSystem.EmissionModule emissionModule;
        emissionModule = ExplosionEffect.emission;
        emissionModule.rateOverTime = 0;

        ParticleSystem.Burst burst = emissionModule.GetBurst(0);
        burst.count = mainMod.maxParticles;

        ExplosionEffect.transform.parent = null;
        ExplosionEffect.transform.localScale = Vector3.one;
        ExplosionEffect.gameObject.SetActive(true);
        ExplosionEffect.Play();
    }

    // Update is called once per frame
    void FixedUpdate() {
        print(projectile.HasFired());
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
