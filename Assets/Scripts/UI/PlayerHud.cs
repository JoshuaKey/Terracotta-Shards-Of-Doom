using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHud : MonoBehaviour {

    public float HealthTransitionSpeed = 1.0f;

    [Header("Player Health Bar")]
    public GameObject PlayerHealthBar;
    public Slider PlayerHealthForegroundSlider;
    public Slider PlayerHealthBackgroundSlider;
    private IEnumerator PlayerHealthRoutine;

    [Header("Crosshair")]
    public GameObject CrossHair;
    public Image CrossHairImage;
    public Image CrossHairBackground;

    [Header("Interact Text")]
    public GameObject InteractTextObject;
    public TextMeshProUGUI InteractText;

    [Header("Enemy Count")]
    public GameObject EnemyCount;
    public TextMeshProUGUI EnemyCountText;

    [Header("Weapon Toggle")]
    public GameObject WeaponToggle;
    public Image NextWeaponIcon;
    public Image CurrWeaponIcon;
    public Image PrevWeaponIcon;
    public TextMeshProUGUI NextWeaponInputIcon;
    public TextMeshProUGUI PrevWeaponInputIcon;

    [Header("Weapon Wheel")]
    public GameObject WeaponWheel;
    public TextMeshProUGUI WeaponWheelIcon;

    [Header("Compass")]
    public GameObject Compass;

    [Header("Boss Health Bar")]
    public GameObject BossHealthBar;
    public Slider BossHealthForegroundSlider;
    public Slider BossHealthBackgroundSlider;
    private IEnumerator BossHealthRoutine;

    public static PlayerHud Instance;

    public void Start() {
        if (Instance != null) { Destroy(this.gameObject); return; }
        Instance = this;
    }

    // Hud
    public void EnablePlayerHud() {
        this.gameObject.SetActive(true);
    }
    public void DisablePlayerHud() {
        this.gameObject.SetActive(false);
    }

    // Player Health Bar
    public void EnablePlayerHealthBar() {
        PlayerHealthBar.gameObject.SetActive(true);
    }
    public void SetPlayerHealthBar(float percent) {
        EnablePlayerHealthBar();

        if (PlayerHealthRoutine != null) {
            StopCoroutine(PlayerHealthRoutine);
        }

        if (percent > PlayerHealthForegroundSlider.value) { // Healing
            PlayerHealthBackgroundSlider.value = percent;
            PlayerHealthRoutine = SliderTransition(PlayerHealthForegroundSlider, percent);
        } else { // Taking Damage
            PlayerHealthForegroundSlider.value = percent;
            PlayerHealthRoutine = SliderTransition(PlayerHealthBackgroundSlider, percent);
        }
        StartCoroutine(PlayerHealthRoutine);
    }
    public void DisablePlayerHealthBar() {
        PlayerHealthBar.gameObject.SetActive(false);
    }

    // Interact Text
    public void EnableInteractText() {
        InteractText.gameObject.SetActive(true);
    }
    public void SetInteractText(string button, string name) {
        EnableInteractText();
        //InteractText.text = "Press <sprite name=\"" + button + "\"> to interact with '" + name + "'";
        InteractText.text = "Press '" + button + "' to interact with '" + name + "'";
    }
    public void DisableInteractText() {
        InteractText.gameObject.SetActive(false);
    }

    // Crosshair
    public void EnableCrosshair() {
        CrossHair.gameObject.SetActive(true);
    }
    public void SetCrosshair(Sprite crosshair) {
        EnableCrosshair();
        CrossHairImage.sprite = crosshair;
        CrossHairBackground.sprite = crosshair;
    }
    public void DisableCrosshair() {
        CrossHair.gameObject.SetActive(false);
    }

    // Enemy Count
    public void EnableEnemyCount() {
        EnemyCount.gameObject.SetActive(true);
    }
    public void SetEnemyCount(int currEnemyCount, int maxEnemyCount) {
        EnableEnemyCount();
        EnemyCountText.text = currEnemyCount + " / " + maxEnemyCount + " Pots";
    }
    public void DisableEnemyCount() {
        EnemyCount.gameObject.SetActive(false);
    }

    // Weapon toggle
    public void EnableWeaponToggle() {

    }
    public void SetWeaponToggle() {

    }
    public void DisableWeaponToggle() {

    }

    // Weapon Wheel
    public void EnableWeaponWheel() {

    }
    public void SetWeaponWheel() {

    }
    public void DisableWeaponWheel() {

    }

    // Compass
    public void EnableCompass() {

    }
    public void SetCompass() {

    }
    public void DisableCompass() {

    }

    // Boss Health Bar
    public void EnableBossHealthBar() {
        BossHealthBar.gameObject.SetActive(true);
    }
    public void SetBossHealthBar(float percent) {
        EnableBossHealthBar();
        if (BossHealthRoutine != null) {
            StopCoroutine(BossHealthRoutine);
        }
        if (percent > BossHealthForegroundSlider.value) { // Healing
            BossHealthBackgroundSlider.value = percent;
            BossHealthRoutine = SliderTransition(BossHealthForegroundSlider, percent);
        } else { // Taking Damage
            BossHealthForegroundSlider.value = percent;
            BossHealthRoutine = SliderTransition(BossHealthBackgroundSlider, percent);
        }
        StartCoroutine(BossHealthRoutine);
    }
    public void DisableBossHealthBar() {
        BossHealthBar.gameObject.SetActive(false);
    }

    public IEnumerator SliderTransition(Slider slider, float value) {
        bool increase = slider.value < value;
        bool finished = false;

        float start = slider.value;
        float t = 0.0f;
        while(!finished) {
            t += Time.deltaTime * HealthTransitionSpeed;

            float newVal = Mathf.Lerp(start, value, t);

            slider.value = newVal;

            finished = increase ? slider.value >= value : slider.value <= value;

            yield return null;  
        }
    }
}
