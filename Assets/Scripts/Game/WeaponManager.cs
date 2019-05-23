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

    //public FireSword FireSwordPrefab;
    //public IceBow IceBowPrefab;
    //public EarthHammer EarthHammerPrefab;
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
            case "CrossBow":
                w = GameObject.Instantiate(CrossBowPrefab);
                w.name = "CrossBow";
                break;
            case "Magic":
                w = GameObject.Instantiate(MagicPrefab);
                w.name = "Magic";
                break;
            case "FireSword":
                break;
            case "IceBow":
                break;
            case "EarthHammer":
            case "RockHammer":
                break;
            case "LightningSpear":
                break;
            case "MagicMissile":
            case "RocketLauncher":
                w = GameObject.Instantiate(RocketLauncherPrefab);
                break;
            case "MagicMagic":
                w = GameObject.Instantiate(MagicMagicPrefab);
                w.name = "MagicMagic";
                break;
        }
        return w;
    }
}
