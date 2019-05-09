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

    private string state;
    private IEnumerator routine;

    private float bobTimer = 0.0f;
    private float bobDist = 0.0f;

    float currXAngle;
    float currYAngle;
    Vector3 currPos;

    private float nextActiveTime;

    private void Start() {
        this.gameObject.SetActive(false);
    }

    public void Activate() {
        if (Time.time >= nextActiveTime) {
            this.gameObject.SetActive(true);
            this.transform.position = Origin.position;
            currPos = Origin.position;

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
        //Vector3 currPos = this.transform.position;
        Vector3 dest = Base.position + Base.forward * Distance;
        while (!IsCloseTo(currPos, dest)) {
            // CURVE??

            Vector3 dir = (dest - currPos).normalized;

            currPos += dir * MoveSpeed * Time.deltaTime;

            SetPosition(currPos);

            yield return null;

            //currPos = this.transform.position;
            dest = Base.position + Base.forward * Distance;
        }

        SetPosition(currPos);

        routine = RotateAround();
        StartCoroutine(routine);
    }

    // Moves the Pot towards it's Parent
    private IEnumerator MoveIn() {
        //Vector3 currPos = this.transform.position;
        Vector3 dest = Origin.position;
        while (!IsCloseTo(currPos, dest)) {
            // CURVE??

            Vector3 dir = (dest - currPos).normalized;

            currPos += dir * MoveSpeed * Time.deltaTime;

            SetPosition(currPos);

            yield return null;

            //currPos = this.transform.position;
            dest = Origin.position;
        }

        SetPosition(currPos);

        this.gameObject.SetActive(false);
    }

    private IEnumerator MoveToRotation(float angle, float e = .1f) {
        //Vector3 currPos = this.transform.position;
        Vector3 dest = Base.position + Quaternion.Euler(0, angle, 0) * Base.forward * Distance;
        while (!IsCloseTo(currPos, dest)) {
            Vector3 destDir = dest - currPos;
            Vector3 dir = destDir.normalized;

            Vector3 mvmt = dir * RotateSpeed * Time.deltaTime;
            if(mvmt.sqrMagnitude > destDir.sqrMagnitude) {
                currPos += destDir;
            } else {
                currPos += mvmt;
            }        

            SetPosition(currPos);

            yield return null;

            //currPos = this.transform.position;
            dest = Base.position + Quaternion.Euler(0, angle, 0) * Base.forward * Distance;
        }

        SetPosition(dest);
    }

    // Moves around the Player's Vision
    private IEnumerator RotateAround() {
        float angleInc = 1;
        float maxXAngle = PeripheralAngle;
        currXAngle = 0;

        // Rotate around player 6 times
        for(int i = 0; i < 6; i++) {

            // Keep rotating until we hit the maxAngle, aka PeripheralAngle
            while (true) {

                // Check if we are close to our current rotation dest, aka in a circle
                yield return MoveToRotation(currXAngle);

                // Change Rotation Direction
                if (currXAngle == maxXAngle) {
                    break;
                }

                // Next Rotation Position
                currXAngle += angleInc;
            }

            // Change Max Angle
            angleInc = maxXAngle == PeripheralAngle ? -1 : 1;
            maxXAngle = maxXAngle == PeripheralAngle ? -PeripheralAngle : PeripheralAngle;           
        }

        routine = RotateTowards();
        StartCoroutine(routine);
    }

    // Moves to the target's angle, but stays in the Player's vision
    private IEnumerator RotateTowards() {
        float angleInc = 1;
        float maxXAngle = PeripheralAngle;

        // Keep rotating until we hit the maxAngle
        while (true) {
            // Target Angle
            Vector3 targetDir = (Target.position - Base.position).normalized;
            float angle = Vector3.SignedAngle(Base.forward, targetDir, Vector3.up);

            // Clamp Max Angle
            if (angle > PeripheralAngle) {
                maxXAngle = PeripheralAngle;
            } else if (angle < -PeripheralAngle) {
                maxXAngle = -PeripheralAngle;
            } else {
                maxXAngle = angle;
            }

            // Check if we are close to our current rotation dest, aka in a circle
            yield return MoveToRotation(currXAngle, .01f);

            // Rotate towards destination
            if (currXAngle < maxXAngle) {
                currXAngle += angleInc;
            } else if(currXAngle > maxXAngle) {
                currXAngle -= angleInc;
            }
        }
    }

    private void SetPosition(Vector3 pos) {
        currPos = pos;

        float bobPos = Bob(MovingBobSpeed, MovingBobDistance);

        this.transform.position = currPos + Base.up * bobPos;

        this.transform.rotation = Quaternion.LookRotation(-Base.forward, Vector3.up);

        if(Target != null && Base != null) {
            Debug.DrawLine(Base.position, Target.position, Color.green);
        }
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

    // Determines whether 2 positions 
    private bool IsCloseTo(Vector3 pos, Vector3 point, float e = .1f) {
        pos.y = point.y = 0.0f;
        return (point - pos).sqrMagnitude <= e * e;
    }

    private void OnGUI() {
        GUI.Label(new Rect(170, 10, 150, 20), "Ang: " + currXAngle);
        GUI.Label(new Rect(170, 30, 150, 20), "Pos: " + currPos);
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

// DO NOT USE APPROXIMATION
// LERP
// ABSOULTE POSITIONS