using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectMagic : MonoBehaviour {

    public TargetProjectile projectile;
    public ParticleSystem HitEffect;

    // Start is called before the first frame update
    void Start() {
        projectile.OnFire += Fire;
    }

    private void Fire(TargetProjectile proj) {
        HitEffect.Play();
    }
}
