using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WeaponPickup : MonoBehaviour {

    public Vector3 Axis;
    public float RotateSpeed;
    public float Speed = .5f;
    public float Magnitude = 3;

    public Interactable interactable;
    public GameObject WeaponDisplay;
    public string WeaponName;

    private Vector3 origin;

    // Start is called before the first frame update
    void Start() {
        if (interactable == null) { interactable = GetComponentInChildren<Interactable>(true); }
        interactable.OnInteract += this.Pickup;

        // Checks if the Player has a weapon that has the same name as this pickup,
        //  or if they have an Advanced Weapon with the old weapon name as this pickup...
        bool hasPickedUp = Player.Instance.weapons.Find(x => {
            if (x is AdvancedWeapon) {
                AdvancedWeapon w = (AdvancedWeapon)x;
                return (WeaponName == w.name) || (WeaponName == w.OldWeaponName);
            } else {
                return WeaponName == x.name;
            }
        }) != null;
        // If Player has already picked up weapon, delete this
        if (hasPickedUp) {
            Destroy(this.gameObject);
        }

        origin = this.transform.position;
    }

    // Update is called once per frame
    void Update() {
        WeaponDisplay.transform.Rotate(Axis * RotateSpeed * Time.deltaTime, Space.World);

        Vector3 up = Vector3.up *  Mathf.Sin(Time.time * Speed) * Magnitude;
        WeaponDisplay.transform.position = origin + up;
    }

    public void Pickup() {
        Player player = Player.Instance;
        Weapon weapon = WeaponManager.Instance.GetWeapon(WeaponName);
        player.AddWeapon(weapon);

        interactable.OnInteract -= this.Pickup;
        interactable.CanInteract = false;
        this.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(Game.Instance.PlayerTag)) {
            Pickup();
        }
    }
}
