using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHud : MonoBehaviour {

    [Header("Player")]
    public Slider PlayerHealthBar;
    public TextMeshProUGUI InteractText;
    public Image CrossHairs;

    [Header("Boss")]
    public Slider BossHealthBar;

    public static PlayerHud Instance;

    public void Start() {
        if (Instance != null) { Destroy(this.gameObject); return; }
        Instance = this;
    }

    // Player
    public void SetPlayerHealth(float percent) {
        PlayerHealthBar.gameObject.SetActive(true);
        PlayerHealthBar.value = percent;
    }
    public void SetInteractText(string button, string name) {
        InteractText.gameObject.SetActive(true);
        InteractText.text = "Press '" + button + "' to interact with '" + name + "'";
    }

    public void EnableCrosshairs() {
        CrossHairs.gameObject.SetActive(true);
    }

    public void DisableCrosshairs() {
        CrossHairs.gameObject.SetActive(false);
    }
    public void DisableInteractText() {
        InteractText.gameObject.SetActive(false);
    }
    public void DisablePlayerUI() {
        PlayerHealthBar.gameObject.SetActive(false);
    }

    // Boss
    public void SetBossHealth(float percent) {
        BossHealthBar.gameObject.SetActive(true);
        BossHealthBar.value = percent;
    }

    public void DisableBossUI() {
        BossHealthBar.gameObject.SetActive(false);
    }

}
