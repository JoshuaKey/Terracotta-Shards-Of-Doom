using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHud : MonoBehaviour {

    // Interact
    public TextMeshProUGUI InteractText;

    // Boss
    public RectTransform BossUI;
    public Slider BossHealthBar;

    public static PlayerHud Instance;

    public void Start() {
        if (Instance != null) { Destroy(this.gameObject); return; }
        Instance = this;
    }

    public void SetBossHealth(float percent) {
        BossHealthBar.value = percent;
    }

    public void DisableBossUI() {
        BossUI.gameObject.SetActive(false);
    }

    public void SetInteractText(string button, string name) {
        InteractText.gameObject.SetActive(true);
        InteractText.text = "Press '" + button + "' to interact with '" + name + "'";
    }

    public void DisableInteractText() {
        InteractText.gameObject.SetActive(false);
    }
}
