using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompassPot : MonoBehaviour {

    [Header("Timer")]
    public float ActiveTime = 5.0f;
    public float CooldownTime = 5.0f;

    [Header("Moving")]
    public float Speed = 5;
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
    private float baseY;

    private float bobTimer = 0.0f;
    private float bobDist = 0.0f;

    private Vector3 dir;

    private float nextActiveTime;

    private void Start() {
        baseY = this.transform.localPosition.y;
        this.gameObject.SetActive(false);
    }

    public void Activate(Vector3 moveDir) {
        if (Time.time >= nextActiveTime) {
            this.gameObject.SetActive(true);
            this.dir = moveDir;

            StartCoroutine(ActivateAbility());

            nextActiveTime = Time.time + ActiveTime + CooldownTime;
        }
    }

    public void SetDirection(Vector3 dir) {
        this.dir = dir;
    }

    private void Enable() {
        if (routine != null) {
            StopCoroutine(routine);
            if (state != "") {
                dir = this.transform.localPosition.normalized;
            }
        }   

        routine = Move(dir * Distance, false);
        StartCoroutine(routine);
    }
    private void Disable() {
        if (routine != null) {
            StopCoroutine(routine);
        }

        routine = Move(Vector3.zero, true);
        StartCoroutine(routine);
    }

    private IEnumerator ActivateAbility() {
        Enable();
        yield return new WaitForSeconds(ActiveTime);
        Disable();
    }

    private IEnumerator Move(Vector3 dest, bool disable) {
        Vector3 pos = this.transform.localPosition;
        while (!IsCloseTo(pos, dest)) {
            //print("Moving");
            state = "Move";
            pos = this.transform.localPosition;

            // Moving
            Vector3 dir = (dest - pos).normalized;
            pos += dir * Speed * Time.deltaTime;

            // Bobing
            pos.y = baseY + Bob(MovingBobSpeed, MovingBobDistance);

            this.transform.localPosition = pos;

            yield return null;
        }

        
        if (disable) {
            routine = null;
            this.gameObject.SetActive(false);
            state = "";
        } else {
            routine = Rotate();
            StartCoroutine(routine);
        }
    }

    private IEnumerator Rotate() {
        while (true) {
            Vector3 pos = this.transform.localPosition;
            Vector3 dest = dir * Distance;

            if (dir == Vector3.zero) {
                //print("Circling");
                state = "Circle";

                pos = Quaternion.Euler(RotateSpeed * Time.deltaTime, 0, 0) * pos;
                pos.y = baseY + Bob(BobSpeed, BobDistance);
            } else if(IsCloseTo(pos, dest)) {
                //print("Bobing");
                state = "Bob";

                pos.y = baseY + Bob(BobSpeed, BobDistance);
            } else {
                //print("Rotating");
                state = "Rotate";

                Vector3 posDir = this.transform.localPosition.normalized;
                pos = Vector3.RotateTowards(posDir, dir, RotateSpeed * Time.deltaTime, 0.0f);
                pos = pos * Distance;
                pos.y = baseY + Bob(MovingBobSpeed, MovingBobDistance);
            }

            this.transform.localPosition = pos;

            yield return null;
        }  
    }

    private float Bob(float speed, float distance) {
        bobTimer += Time.deltaTime * speed;
        bobDist = Mathf.Lerp(bobDist, distance, Time.deltaTime);
        return Mathf.Sin(bobTimer) * bobDist;
    }
    private bool IsCloseTo(Vector3 pos, Vector3 point, float e = .1f) {
        pos.y = point.y = 0.0f;
        return (point - pos).sqrMagnitude <= e * e;
    }
}
