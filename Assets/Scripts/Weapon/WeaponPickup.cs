using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        string name = WeaponName;
        // Initialize
        if (!Game.Instance.playerStats.Weapons.ContainsKey(name)) {
            Game.Instance.playerStats.Weapons[name] = false;
        }
        // Destory if already collected
        if (Game.Instance.playerStats.Weapons[name]) {
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
        print(weapon);
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
