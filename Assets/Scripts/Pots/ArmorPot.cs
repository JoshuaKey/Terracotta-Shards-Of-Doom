using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorPot : ChargerPot
{
    [HideInInspector]
    public Enemy enemy;

    [Header("Visual")]
    public int ArmorHealth = 2;
    public int ArmorCount = 3;
    public List<Rigidbody> ArmorPieces;

    protected override void Start()
    {
        base.Start();

        if (enemy == null) { enemy = GetComponentInChildren<Enemy>(true); }

        ArmorCount = ArmorPieces.Count;

        enemy.health.OnDamage += RemoveArmor;

        //print("Testing (8, 8): " + TestArmor(8, 8));
        //print("Testing (8, 7): " + TestArmor(8, 7));
        //print("Testing (8, 6): " + TestArmor(8, 6));
        //print("Testing (8, 5): " + TestArmor(8, 5));
        //print("Testing (8, 4): " + TestArmor(8, 4));
        //print("Testing (8, 3): " + TestArmor(8, 3));
        //print("Testing (8, 2): " + TestArmor(8, 2));
        //print("Testing (8, 1): " + TestArmor(8, 1));
        //print("Testing (8, 0): " + TestArmor(8, 0));
    }

    public void RemoveArmor(float value) {
        while (GetArmor() < ArmorPieces.Count) {
            int index = Random.Range(0, ArmorPieces.Count);
            Rigidbody armor = ArmorPieces[index];
            armor.isKinematic = false;
            armor.transform.parent = null;
            ArmorPieces.RemoveAt(index);
        }
    }

    public float GetArmor() {
        float healthDifference = enemy.health.MaxHealth - enemy.health.CurrentHealth;
        float armorTaken = healthDifference / ArmorHealth;
        float armorAmo = ArmorCount - armorTaken;

        return Mathf.Max(Mathf.CeilToInt(armorAmo), 0.0f);
    }

    //public float TestArmor(int maxHealth, int currHealth) {
    //    float healthDifference = maxHealth - currHealth;
    //    float armorTaken = healthDifference / ArmorHealth;
    //    float armorAmo = ArmorCount - armorTaken;


    //    return Mathf.Max(Mathf.CeilToInt(armorAmo), 0.0f);

    //    // 8,8 => 3
    //    // 8, 7.5 => 2.5
    //    //print("Diff: " + healthDifference);
    //    //print("Taken: " + armorTaken);
    //    //print("Amo: " + armorAmo);
    //}

}
