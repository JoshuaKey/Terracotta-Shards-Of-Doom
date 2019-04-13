using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour {

    public Vector3 Axis;
    public float Speed;

    public Interactable interactable;
    public Weapon WeaponPrefab;

    // Start is called before the first frame update
    void Start() {
        interactable.OnInteract += this.Pickup;

        WeaponPrefab = GameObject.Instantiate(WeaponPrefab);
        WeaponPrefab.transform.SetParent(this.transform, false);
        WeaponPrefab.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        this.transform.Rotate(Axis, Speed * Time.deltaTime);
    }

    public void Pickup() {
        Player player = Player.Instance;
        player.AddWeapon(WeaponPrefab);

        interactable.OnInteract -= this.Pickup;
        interactable.CanInteract = false;
        this.gameObject.SetActive(false);
    }
}
