using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorPot : ChargerPot
{
    [HideInInspector]
    public Enemy enemy;

    [Header("Visual")]
    public int ArmorHealth = 2; 
    public List<Rigidbody> ArmorPieces;

    private int defaultLayer;
    private int armorCount = 3;

    protected override void Start()
    {
        base.Start();

        if (enemy == null) { enemy = GetComponentInChildren<Enemy>(true); }

        defaultLayer = LayerMask.NameToLayer("Default");
        armorCount = ArmorPieces.Count;

        enemy.health.OnDamage += RemoveArmor;
    }

    public void RemoveArmor(float value) {
        while (GetArmor() < ArmorPieces.Count) {
            int index = Random.Range(0, ArmorPieces.Count);
            Rigidbody armor = ArmorPieces[index];
            armor.isKinematic = false;
            armor.transform.parent = null;
            armor.gameObject.layer = defaultLayer;
            //armor.AddForce(armor.transform.position - this.transform.position);
            ArmorPieces.RemoveAt(index);
        }
    }

    public float GetArmor() {
        float healthDifference = enemy.health.MaxHealth - enemy.health.CurrentHealth;
        float armorTaken = healthDifference / ArmorHealth;
        float armorAmo = armorCount - armorTaken; 

        return Mathf.Max(Mathf.CeilToInt(armorAmo), 0.0f);
    }

    public float TestArmor(int maxHealth, int currHealth) {
        float healthDifference = maxHealth - currHealth;
        float armorTaken = healthDifference / ArmorHealth;
        float armorAmo = armorCount - armorTaken;

        return Mathf.Max(Mathf.CeilToInt(armorAmo), 0.0f);
    }

}
