using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour {

    public Vector3 Axis;
    public float RotateSpeed;
    public float Speed = .5f;
    public float Magnitude = 3;

    public Interactable interactable;
    public Weapon WeaponPrefab;

    private Vector3 origin;

    // Start is called before the first frame update
    void Start() {
        if (interactable == null) { interactable = GetComponentInChildren<Interactable>(true); }
        interactable.OnInteract += this.Pickup;

        string name = WeaponPrefab.name;
        WeaponPrefab = GameObject.Instantiate(WeaponPrefab);
        WeaponPrefab.transform.SetParent(this.transform, false);
        WeaponPrefab.gameObject.SetActive(false);
        WeaponPrefab.name = name;

        origin = this.transform.position;
    }

    // Update is called once per frame
    void Update() {
        this.transform.Rotate(Axis, RotateSpeed * Time.deltaTime);

        Vector3 up = Vector3.up *  Mathf.Sin(Time.time * Speed) * Magnitude;
        this.transform.position = origin + up;
    }

    public void Pickup() {
        Player player = Player.Instance;
        player.AddWeapon(WeaponPrefab);

        interactable.OnInteract -= this.Pickup;
        interactable.CanInteract = false;
        this.gameObject.SetActive(false);

        PlayerHud.Instance.EnableWeaponToggle();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(Game.Instance.PlayerTag)) {
            Pickup();
        }
    }
}
