using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolcanoPot : Pot {
    // Start is called before the first frame update
    void Start() {
        stateMachine = new StateMachine();
        stateMachine.Init(gameObject,
            new Charger_Idle(),
            new Charger_Charge(),
            new Charger_Attack(attackDuration));
    }

    public override void Animate() {
       
    }
}
public class Volcano_Idle : State {
    //if player is within aggro radius and visible
    //or pot is damaged
    //move to charger_charge

    VolcanoPot vocano;

    public override void Init(GameObject owner) {
        base.Init(owner);
        chargerPot = owner.GetComponent<ChargerPot>();
        health = owner.GetComponent<Health>();
    }

    public override void Enter() {
        isDamaged = false;
        health.OnDamage += OnDamage;
    }

    public override void Exit() {
        health.OnDamage -= OnDamage;
    }

    public override string Update() {
        Vector3 towardPlayer = Player.Instance.transform.position - owner.transform.position;

        RaycastHit hit;
        if ((Physics.Raycast(owner.transform.position, towardPlayer, out hit, chargerPot.aggroRadius, ~LayerMask.GetMask("Enemy"))
            && hit.collider.tag == "Player")
            || isDamaged) {
            return "Charger_Charge";
        }

        return null;
    }

    public void OnDamage(float damage) {
        isDamaged = true;
    }
}
