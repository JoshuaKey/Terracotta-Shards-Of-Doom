using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompassPot : MonoBehaviour {

    public Transform Origin; // Compass' Origin
    public Transform Base; // Rotating Around
    public Transform Target; // Rotating Towards
    public float PeripheralAngle = 45f;

    [Header("Timer")]
    public float ActiveTime = 5.0f;
    public float CooldownTime = 5.0f;

    [Header("Moving")]
    public float MoveSpeed = 5;
    public float Distance = 2;
    public float MovingBobSpeed = 1;
    public float MovingBobDistance = .5f;

    [Header("Rotating")]
    public float RotateSpeed = 3.14f;

    [Header("Bobing")]
    public float BobSpeed = 3f;
    public float BobDistance = 1f;

    private string state;
    private IEnumerator routine;

    private Vector3 localPos;
    private float bobTimer = 0.0f;
    private float bobDist = 0.0f;

    private float nextActiveTime;

    private void Start() {
        this.gameObject.SetActive(false);
    }

    public void Activate() {
        if (Time.time >= nextActiveTime) {
            this.gameObject.SetActive(true);
            this.transform.position = Origin.position;

            StartCoroutine(ActivateAbility());

            nextActiveTime = Time.time + ActiveTime + CooldownTime;
        }
    }

    private IEnumerator ActivateAbility() {
        // Enable
        if (routine != null) {
            StopCoroutine(routine);
            routine = null;
        }
        routine = MoveOut();
        StartCoroutine(routine);

        yield return new WaitForSeconds(ActiveTime);

        // Disable
        if (routine != null) {
            StopCoroutine(routine);
            routine = null;
        }
        routine = MoveIn();
        StartCoroutine(routine);
    }

    // Moves the Pot towards the Players Forward
    private IEnumerator MoveOut() {
        Vector3 pos = this.transform.position;
        Vector3 dest = Base.position + Base.forward * Distance;
        while (!IsCloseTo(pos, dest)) {
            pos = this.transform.position;
            dest = Base.position + Base.forward * Distance;

            Vector3 dir = (dest - pos).normalized;

            pos += dir * MoveSpeed * Time.deltaTime;

            this.transform.position = pos;

            yield return null;
        }

        routine = RotateAround();
        StartCoroutine(routine);
    }

    // Moves the Pot towards it's Parent
    private IEnumerator MoveIn() {
        Vector3 pos = this.transform.position;
        Vector3 dest = Origin.position;
        while (!IsCloseTo(pos, dest)) {
            pos = this.transform.position;
            dest = Origin.position;

            Vector3 dir = (dest - pos).normalized;

            pos += dir * MoveSpeed * Time.deltaTime;

            this.transform.position = pos;

            yield return null;
        }

        this.gameObject.SetActive(false);
    }

    // Moves around the Player's Vision
    private IEnumerator RotateAround() {
        Vector3 pos = this.transform.position;
        Vector3 dest = Base.position + Quaternion.Euler(0, PeripheralAngle, 0) * Base.forward * Distance;

        // Right
        while (!IsCloseTo(pos, dest)) {
            pos = this.transform.position;
            dest = Base.position + Quaternion.Euler(0, PeripheralAngle, 0) * Base.forward * Distance;

            Vector3 dir = (dest - pos).normalized;

            pos += dir * RotateSpeed * Time.deltaTime;

            this.transform.position = pos;

            yield return null;
        }

        // Left
        pos = this.transform.position;
        dest = Base.position + Quaternion.Euler(0, -PeripheralAngle, 0) * Base.forward * Distance;
        while (!IsCloseTo(pos, dest)) {
            pos = this.transform.position;
            dest = Base.position + Quaternion.Euler(0, -PeripheralAngle, 0) * Base.forward * Distance;

            Vector3 dir = (dest - pos).normalized;

            pos += dir * RotateSpeed * Time.deltaTime;

            this.transform.position = pos;

            yield return null;
        }

        // Right
        pos = this.transform.position;
        dest = Base.position + Quaternion.Euler(0, PeripheralAngle, 0) * Base.forward * Distance;
        while (!IsCloseTo(pos, dest)) {
            pos = this.transform.position;
            dest = Base.position + Quaternion.Euler(0, PeripheralAngle, 0) * Base.forward * Distance;

            Vector3 dir = (dest - pos).normalized;

            pos += dir * RotateSpeed * Time.deltaTime;

            this.transform.position = pos;

            yield return null;
        }

        // Left
        pos = this.transform.position;
        dest = Base.position + Quaternion.Euler(0, -PeripheralAngle, 0) * Base.forward * Distance;
        while (!IsCloseTo(pos, dest)) {
            pos = this.transform.position;
            dest = Base.position + Quaternion.Euler(0, -PeripheralAngle, 0) * Base.forward * Distance;

            Vector3 dir = (dest - pos).normalized;

            pos += dir * RotateSpeed * Time.deltaTime;

            this.transform.position = pos;

            yield return null;
        }

        routine = RotateTowards();
        StartCoroutine(routine);
    }

    // Moves to the target's angle, but stays in the Player's vision
    private IEnumerator RotateTowards() {
        while (true) {
            print(Target.name);

            Vector3 pos = this.transform.position;

            Vector3 targetDir = (Target.position - Base.position).normalized;
            float angle = Vector3.SignedAngle(Base.forward, targetDir, Vector3.up);
            Vector3 dest = Vector3.zero;
            if (angle > PeripheralAngle) {
                dest = Base.position + Quaternion.Euler(0, PeripheralAngle, 0) * Base.forward * Distance;
            } else if (angle < -PeripheralAngle) {
                dest = Base.position + Quaternion.Euler(0, -PeripheralAngle, 0) * Base.forward * Distance;
            } else {
                dest = Base.position + Quaternion.Euler(0, angle, 0) * Base.forward * Distance;
            }

            if(!IsCloseTo(pos, dest)) {
                Vector3 dir = (dest - pos).normalized;

                pos += dir * RotateSpeed * Time.deltaTime;

                this.transform.position = pos;
            }

            yield return null;
        }

    }

    //private IEnumerator Move(Vector3 dest, bool disable) {
    //    Vector3 pos = this.transform.localPosition;
    //    while (!IsCloseTo(pos, dest)) {
    //        //print("Moving");
    //        state = "Move";
    //        pos = this.transform.localPosition;

    //        // Moving
    //        Vector3 dir = (dest - pos).normalized;
    //        pos += dir * Speed * Time.deltaTime;

    //        // Bobing
    //        pos.y = baseY + Bob(MovingBobSpeed, MovingBobDistance);

    //        this.transform.localPosition = pos;

    //        yield return null;
    //    }

        
    //    if (disable) {
    //        routine = null;
    //        this.transform.parent = Target;
    //        this.gameObject.SetActive(false);
    //        state = "";
    //    } else {
    //        routine = Rotate();
    //        StartCoroutine(routine);
    //    }
    //}

    //private IEnumerator Rotate() {
    //    while (true) {
    //        Vector3 pos = this.transform.position;
    //        Vector3 dest = dir * Distance;

    //        if (dir == Vector3.zero) {
    //            state = "Circle";

    //            pos = Quaternion.Euler(RotateSpeed * Time.deltaTime, 0, 0) * pos;
    //            pos.y = baseY + Bob(BobSpeed, BobDistance);
    //        } else if(IsCloseTo(pos, dest)) {
    //            state = "Bob";

    //            pos.y = baseY + Bob(BobSpeed, BobDistance);
    //        } else {
    //            state = "Rotate";

    //            Vector3 posDir = this.transform.localPosition.normalized;
    //            pos = Vector3.RotateTowards(posDir, dir, RotateSpeed * Time.deltaTime, 0.0f);
    //            pos = pos * Distance;
    //            pos.y = baseY + Bob(MovingBobSpeed, MovingBobDistance);
    //        }

    //        this.transform.localPosition = pos;

    //        yield return null;
    //    }  
    //}

    private float Bob(float speed, float distance) {
        // Add onto Bob Timer (aka Sin Wave)
        bobTimer += Time.deltaTime * speed;

        // Set new Bob Height to Lerp between previous and new Distance
        // Aka, going from 1m to 3m will instantly change height magnitude.
        bobDist = Mathf.Lerp(bobDist, distance, Time.deltaTime);

        // Bob Value
        return Mathf.Sin(bobTimer) * bobDist;
    }

    // Determines whether 2 positions 
    private bool IsCloseTo(Vector3 pos, Vector3 point, float e = .1f) {
        pos.y = point.y = 0.0f;
        return (point - pos).sqrMagnitude <= e * e;
    }
}

// Compass States:
//  Move Out
//      Compass activates and moves out from player
//  Rotate Around
//      Compass rotates around player several times
//  Rotate Towards
//      Compass rotates toward the correct direction the player should go.
//  Bob             
//      Compass bobs up and down at a frequent speed to indicate it is pointing in the correct direction.
//  Move In
//      Compass travels towards the player and ends the ability.

// Compass Parent - Shoulder Pos
// Player Forward direction
// Player Forward Angle
// Player Peripheral Angle