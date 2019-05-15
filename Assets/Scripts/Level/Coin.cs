using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : PoolObject {

    public int Value;

    [Header("Movement")]
    public float MoveSpeed = 5.0f;
    public float LerpSpeed = 2.0f;
    public float BobSpeed = 1.0f;
    public float BobDist = 1.0f;
    public float RotateSpeed = 30f;

    [Header("Components")]
    public new Rigidbody rigidbody;
    public new Collider collider;
    public Transform coinModel;

    private Vector3 currPos;
    private float bobTimer = 0.0f;
    private float bobDist = 0.0f;

    void Start() {
        currPos = this.transform.position;
    }

    void Update() {
        Player player = Player.Instance;

        Vector3 pos = Vector3.Lerp(currPos, player.transform.position, LerpSpeed * Time.deltaTime);
        SetPosition(pos);

        this.transform.Rotate(Vector3.up * RotateSpeed * Time.deltaTime, Space.Self);
    }

    private void SetPosition(Vector3 pos) {
        currPos = pos;

        float bobPos = Bob(BobSpeed, BobDist);

        this.transform.position = currPos + Vector3.up * bobPos;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="speed"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
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
            Destroy(this.gameObject);
        }
    }
}
