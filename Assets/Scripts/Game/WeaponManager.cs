using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour {

    public Sword SwordPrefab;
    public Bow BowPrefab;
    public Hammer HammerPrefab;
    public Spear SpearPrefab;
    public CrossBow CrossBowPrefab;
    public Magic MagicPrefab;

    [Header("Advanced Weapons")]
    public FireSword FireSwordPrefab;
    //public IceBow IceBowPrefab;
    public RockHammer RockHammerPrefab;
    //public LightningSpear LightningSpearPrefab;
    public RocketLauncher RocketLauncherPrefab;
    public MagicMagic MagicMagicPrefab;

    public static WeaponManager Instance;

    void Awake() {
        if (Instance != null) { Destroy(this.gameObject); return; }
        Instance = this;
    }

    public Weapon GetWeapon(string weaponName) {
        Weapon w = null;
        switch (weaponName) {
            case "Sword":
                w = GameObject.Instantiate(SwordPrefab);
                w.name = "Sword";
                break;
            case "Bow":
                w = GameObject.Instantiate(BowPrefab);
                w.name = "Bow";
                break;
            case "Hammer":
                w = GameObject.Instantiate(HammerPrefab);
                w.name = "Hammer";
                break;
            case "Spear":
                w = GameObject.Instantiate(SpearPrefab);
                w.name = "Spear";
                break;
            case "Crossbow":
                w = GameObject.Instantiate(CrossBowPrefab);
                w.name = "Crossbow";
                break;
            case "Magic":
                w = GameObject.Instantiate(MagicPrefab);
                w.name = "Magic";
                break;
            case "Fire Sword":
                w = GameObject.Instantiate(FireSwordPrefab);
                break;
            case "Ice Bow":
                break;
            case "Rock Hammer":
                w = GameObject.Instantiate(RockHammerPrefab);
                break;
            case "Lightning Spear":
                break;
            case "Magic Missile":
                w = GameObject.Instantiate(RocketLauncherPrefab);
                break;
            case "Magic Magic":
                w = GameObject.Instantiate(MagicMagicPrefab);
                break;
        }
        return w;
    }
}
