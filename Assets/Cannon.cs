using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour {

    // A cannon can be interacted with
    // When Interacted, the Player will align with the Barrel
    // Then the player will shoot, in a specific direction and velocit

    [HideInInspector]
    public float BaseAngle;
    [HideInInspector]
    public float BarrelAngle;

    [Header("Time")]
    public float ChargeTime = 2.5f;
    public float LeapTime = 10.0f;

    [Header("Object")]
    public GameObject Barrel;
    public GameObject Base;
    public Transform Target;

    private Interactable interactable;

    private WaitForSeconds chargedWait;
    private WaitForSeconds leapWait;

    // Start is called before the first frame update
    void Start() {
        interactable = GetComponent<Interactable>();
        if (interactable == null) { interactable = GetComponentInChildren<Interactable>(); }

        interactable.Subscribe(FirePlayer);

        BarrelAngle = Barrel.transform.root.localEulerAngles.z;
        BaseAngle = Base.transform.root.localEulerAngles.y;
    }

    public void Align(float baseAngle, float barrelAngle) {
        // Dont care right now...
    }

    public void FirePlayer() {
        //print("Here");
        Player player = Player.Instance;
        StartCoroutine(Shoot(player));
    }

    public IEnumerator Shoot(Player player) {
        player.CanWalk = false;
        player.CanMove = false;

        Quaternion playerRot = player.transform.rotation;
        player.transform.rotation = Barrel.transform.rotation;
        player.transform.position = Barrel.transform.position;
        player.rotation = Barrel.transform.rotation.eulerAngles;

        // Move Barrel Back ? Animation
        yield return new WaitForSeconds(ChargeTime);

        Vector3 startPos = Barrel.transform.position;
        float startTime = Time.time;
        while (Time.time < startTime + LeapTime) {
            float t = (Time.time - startTime) / LeapTime;

            Vector3 pos = this.transform.localPosition;
            Quaternion rot = this.transform.localRotation;

            pos.x = Mathf.Lerp(startPos.x, Target.position.x, t);
            pos.y = Mathf.Lerp(startPos.y, Target.position.y, t);
            pos.z = Mathf.Lerp(startPos.z, Target.position.z, t);

            player.transform.position = pos;
            yield return null;
        }

        player.transform.position = Target.position;
        player.CanWalk = true;
        player.CanMove = true;
    }

}
