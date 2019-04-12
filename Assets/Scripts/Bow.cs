using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : Weapon {

    [Header("Force")]
    public float MinSpeed = 5f;
    public float MaxSpeed = 50f;
    public float MaxDistance = 100f;
    public float TimeToMax = 1f;

    [Header("Visuals")]
    public Arrow ArrowPrefab;

    // Value between 0 and 1;
    private float charge;

    protected void Start() {
        CanCharge = true;
        Type = DamageType.BASIC;
    }

    public override void Charge() {
        charge += Time.deltaTime / TimeToMax;
        charge = Mathf.Min(charge, 1.0f);
        print("Charge: " + charge);
    }

    public override void Attack() {
        if (!CanAttack()) { return; }

        base.Attack();
        charge = 0.0f;

        Player player = Player.Instance;
        Ray ray = new Ray(player.transform.position, player.transform.forward);
        RaycastHit hit;
        Vector3 aimPoint = Vector3.zero;
        if(Physics.Raycast(ray, out hit, MaxDistance)) {
            aimPoint = hit.point;
        } else {
            aimPoint = player.transform.position + player.transform.forward * MaxDistance;
        }

        Arrow arrow = GameObject.Instantiate(ArrowPrefab, this.transform);
        arrow.Impulse = MinSpeed + charge * (MaxSpeed - MinSpeed);
        // Bow can instantiate, remove itself from Parent, then fly off...
    }
}
