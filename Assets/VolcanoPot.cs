using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolcanoPot : Pot {

    public LavaShot LavaShotPrefab;
    public Transform LavaSpawn;

    [Header("Attack")]
    public float MaxAimDistance = 20.0f;
    public float MinAimDistance = 3.0f;
    public float AttackSpeed = 2.0f;

    // Start is called before the first frame update
    void Start() {
        stateMachine = new StateMachine();
        stateMachine.Init(gameObject,
            new Volcano_Idle(),
            new Volcano_Shoot(AttackSpeed));
    }

    public override void Animate() { Waddle(); }

    public void ShootLavaShot() {
        LavaShot lava = Instantiate(LavaShotPrefab, LavaSpawn.position, Quaternion.identity);

        Vector3 dest = Player.Instance.transform.position;
        Vector3 peak = Utility.CreatePeak(lava.transform.position, dest, 20);

        lava.Fire(dest, peak); 
    }
}

public class Volcano_Idle : State {

    VolcanoPot volcano;
    Player player;

    public override void Init(GameObject owner) {
        base.Init(owner);
        volcano = owner.GetComponent<VolcanoPot>();
        player = Player.Instance;
    }

    public override void Enter() {
    }

    public override void Exit() {
    }

    public override string Update() {
        float playerDist = (player.transform.position - volcano.transform.position).sqrMagnitude;
        float minDist = volcano.MinAimDistance * volcano.MinAimDistance;
        float maxDist = volcano.MaxAimDistance * volcano.MaxAimDistance;
        if (playerDist <= maxDist && playerDist >= minDist) {
            return "Volcano_Shoot";
        }

        return null;
    }

}

public class Volcano_Shoot : TimedState {

    VolcanoPot volcano;
    Player player;

    public Volcano_Shoot(float seconds) : base(seconds) { }

    public override void Init(GameObject owner) {
        base.Init(owner);
        volcano = owner.GetComponent<VolcanoPot>();
        player = Player.Instance;
    }

    public override void Enter() {
        base.Enter();
        volcano.ShootLavaShot();
    }

    public override void Exit() {
    }

    public override string Update() {
        if (timer >= seconds) {
            return "Volcano_Idle";
        }

        base.Update();
        return null;
    }

}
