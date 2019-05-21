using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] float damage = 1;
    [SerializeField] DamageType damageType = DamageType.BASIC;
    [SerializeField] public float knockback = 20f;

    [HideInInspector] public bool isAttacking = false;
    [HideInInspector] public bool hasHitPlayer = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Game.Instance.PlayerTag) && isAttacking && !hasHitPlayer)
        {
            hasHitPlayer = true;
            Player.Instance.health.TakeDamage(damageType, damage);

            //Player.Instance.Knockback(transform.forward * knockback);

            // I changed the Knocbakc Direction... Fight me
            Vector3 dir = Player.Instance.transform.position - this.transform.position;
            dir.y = 0.0f;
            dir = dir.normalized;
            Player.Instance.Knockback(dir * knockback);
        }
    }
}
