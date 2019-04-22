using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompassPot : MonoBehaviour {

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

    private IEnumerator routine;
    private float baseY;

    private float bobTimer = 0.0f;
    private float bobDist = 0.0f;

    private Vector3 dir;

    private void Start() {
        baseY = this.transform.localPosition.y;
        this.gameObject.SetActive(false);
    }

    public void Enable(Vector3 moveDir) {
        this.gameObject.SetActive(true);
        this.dir = moveDir;

        if (routine != null) {
            StopCoroutine(routine);
        }   

        routine = Move(moveDir * Distance, false);
        StartCoroutine(routine);
    }
    public void UpdateDirection(Vector3 faceDir) {
        this.gameObject.SetActive(true);
        this.dir = faceDir;
    }
    public void Disable() {
        this.gameObject.SetActive(true);

        if (routine != null) {
            StopCoroutine(routine);
        }

        routine = Move(Vector3.zero, true);
        StartCoroutine(routine);
    }

    private IEnumerator Move(Vector3 dest, bool disable) {
        Vector3 pos = this.transform.localPosition;
        while (!IsCloseTo(pos, dest)) {
            print("Moving ");
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
                print("Circling");

                pos = Quaternion.Euler(RotateSpeed * Time.deltaTime, 0, 0) * pos;
                pos.y = baseY + Bob(BobSpeed, BobDistance);
            } else if(IsCloseTo(pos, dest)) {
                print("Bobing");

                pos.y = baseY + Bob(BobSpeed, BobDistance);
            } else {
                print("Rotating");

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
    private bool IsCloseTo(Vector3 pos, Vector3 point, float e = .5f) {
        pos.y = point.y = 0.0f;
        return (point - pos).sqrMagnitude <= e * e;
    }

    private void OnGUI() {
        GUI.Label(new Rect(330, 10, 150, 20), "Pos: " + this.transform.position);
    }
}
