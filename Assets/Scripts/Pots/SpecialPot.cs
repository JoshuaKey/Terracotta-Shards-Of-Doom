using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialPot : MonoBehaviour {

    public SpecialPot PreviousPot;
    [Space]
    public Material HasCollectedMaterial;

    [Header("Components")]
    public Enemy enemy;
    public new MeshRenderer renderer;

    void Start() {
        if(enemy == null) { enemy = GetComponentInChildren<Enemy>(); }

        if(PreviousPot != null) {
            this.gameObject.SetActive(false);

            PreviousPot.enemy.health.OnDeath += Spawn;
        } else {
            this.gameObject.SetActive(true);
        }

        string levelName = LevelManager.Instance.GetLevelName();
        // Initialize value if it doesn't exist...
        if (!LevelManager.Instance.Levels[levelName].SpecialPots.ContainsKey(name)) {
            LevelManager.Instance.Levels[levelName].SpecialPots[name] = false;
        }
        // If already collected, modify material
        if (LevelManager.Instance.Levels[levelName].SpecialPots[name]) {
            renderer.material = HasCollectedMaterial;
            if (enemy.brokenPot != null) {
                enemy.brokenPot.SetMaterial(HasCollectedMaterial);
            }
        }

        this.enemy.health.OnDeath += OnDeath;
    }

    private void OnDeath() {
        string levelName = LevelManager.Instance.GetLevelName();
        LevelManager.Instance.Levels[levelName].SpecialPots[name] = true;
    }

    private void Spawn() {
        this.gameObject.SetActive(true);
    }
}
