using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class BrokenPot : MonoBehaviour {

    public GameObject brokenPot;

    private Enemy enemy;

    void Start() {
        enemy = GetComponent<Enemy>();

        brokenPot.SetActive(false);
    }

    void Update() {
        if (enemy.IsDead()) {
            this.gameObject.SetActive(false);
            brokenPot.SetActive(true);

            brokenPot.transform.parent = null;

            brokenPot.transform.position = this.transform.position;
            brokenPot.transform.rotation = this.transform.rotation;
            brokenPot.transform.localScale = this.transform.localScale;
        }   
    }
}
