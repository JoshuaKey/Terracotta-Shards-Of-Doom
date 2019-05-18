using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : PoolObject {

    public int Value;

    [Header("Movement")]
    public float LerpSpeed = 2.0f;
    public float SpeedOverLifeMultiple = 2.0f;
    public float BobSpeed = 1.0f;
    public float BobDist = 1.0f;
    public float RotateSpeed = 30f;

    private float currSpeed;

    [Header("Components")]
    public new Rigidbody rigidbody;
    public new Collider collider;
    public Transform coinModel;

    [SerializeField] private Vector3 currPos;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float bobTimer = 0.0f;
    [SerializeField] private float bobDist = 0.0f;

    protected override void Start() {
        base.Start();
        currPos = this.transform.position;
        currSpeed = LerpSpeed;
        offset = Random.insideUnitSphere * .5f;
        bobTimer = Random.Range(0.0f, 10.0f);
    }

    void Update() {
        Player player = Player.Instance;

        Vector3 pos = Vector3.Lerp(currPos, player.transform.position + offset, currSpeed * Time.deltaTime);
        SetPosition(pos);

        currSpeed += SpeedOverLifeMultiple * Time.deltaTime;

        this.transform.Rotate(Vector3.up * RotateSpeed * Time.deltaTime, Space.Self);
    }

    public override void Reset() {
        base.Reset();
        this.transform.position = Vector3.zero;
        this.transform.localScale = Vector3.one;
        this.transform.rotation = Quaternion.identity;
        currPos = Vector3.zero;
        offset = Vector3.zero;
        currSpeed = 0.0f;
        bobTimer = 0.0f;
        bobDist = 0.0f;
    }

    public void SetPosition(Vector3 pos) {
        currPos = pos;

        float bobPos = Bob(BobSpeed, BobDist);

        this.transform.position = currPos + Vector3.up * bobPos;
    }

    private float Bob(float speed, float distance) {
        // Add onto Bob Timer (aka Sin Wave)
        bobTimer += Time.deltaTime * speed;

        // Set new Bob Height to Lerp between previous and new Distance
        // Aka, going from 1m to 3m will instantly change height magnitude.
        bobDist = Mathf.Lerp(bobDist, distance, Time.deltaTime);

        // Bob Value
        return Mathf.Sin(bobTimer) * bobDist;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(Game.Instance.PlayerTag)) {
            print("Coin Collected: " + Value);
            Game.Instance.playerStats.Coins += Value;
            Value = 0;
            this.gameObject.SetActive(false);

            PlayerHud.Instance.SetCoinCount(Game.Instance.playerStats.Coins);
            PlayerHud.Instance.PlayCoinAnimation();
        }
    }

    protected override void OnDisable() {
        //print("Coin Disabled");
        base.OnDisable();
        if(Value != 0) {
            print("Coin Collected: " + Value);
            Game.Instance.playerStats.Coins += Value;
            Value = 0;
        }
    }
}
