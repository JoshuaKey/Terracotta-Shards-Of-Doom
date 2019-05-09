using Luminosity.IO;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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
    public Image InteractImage;

    [Header("Enemy Count")]
    public GameObject EnemyCount;
    public TextMeshProUGUI EnemyCountText;

    [Header("Weapon Toggle")]
    public GameObject WeaponToggle;
    public Image NextWeaponIcon;
    public Image CurrWeaponIcon;
    public Image PrevWeaponIcon;
    public TextMeshProUGUI NextWeaponText;
    public TextMeshProUGUI CurrWeaponText;
    public TextMeshProUGUI PrevWeaponText;
    public Image NextWeaponInputIcon;
    public Image PrevWeaponInputIcon;

    [Header("Weapon Icons")]
    public Sprite SwordIcon;
    public Sprite BowIcon;
    public Sprite HammerIcon;
    public Sprite SpearIcon;
    public Sprite CrossbowIcon;
    public Sprite MagicMissleIcon;

    [Header("Weapon Wheel")]
    public GameObject WeaponWheel;
    public RectTransform WeaponWheelCursor;
    public Image[] WeaponWheelIcons; // Total of 6
    public TextMeshProUGUI[] WeaponWheelTexts; // Total of 6
    public Button[] WeaponWheelButtons; // Total of 6
    public Image[] WeaponWheelButtonImages; // Total of 6
    public int WeaponWheelAmo = 1;

    [Header("Boss Health Bar")]
    public GameObject BossHealthBar;
    public Slider BossHealthForegroundSlider;
    public Slider BossHealthBackgroundSlider;
    private IEnumerator BossHealthRoutine;

    [Header("Other")]
    public EventSystem eventSystem;

    public static PlayerHud Instance;

    private void Awake() {
        if (Instance != null) { Destroy(this.gameObject); return; }
        Instance = this;
    }
    private void Start() {
        if(eventSystem == null) { eventSystem = FindObjectOfType<EventSystem>(); }

        InputManager.ControlSchemesChanged += OnControlSchemeChanged;
        InputManager.PlayerControlsChanged += OnPlayerControlChanged;
    }

    private void OnPlayerControlChanged(PlayerID id) { UpdateInputIcons(); }
    private void OnControlSchemeChanged() { UpdateInputIcons(); }
    public void UpdateInputIcons() {
        NextWeaponInputIcon.sprite = InputController.Instance.GetActionIcon("Next Weapon");

        PrevWeaponInputIcon.sprite = InputController.Instance.GetActionIcon("Prev Weapon");

        InteractImage.sprite = InputController.Instance.GetActionIcon("Interact");
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
        PlayerHealthBar.SetActive(true);
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
        PlayerHealthBar.SetActive(false);
    }

    // Interact Text
    public void EnableInteractText() {
        InteractImage.gameObject.SetActive(true);
    }
    public void DisableInteractText() {
        InteractImage.gameObject.SetActive(false);
    }

    // Crosshair
    public void EnableCrosshair() {
        CrossHair.SetActive(true);
    }
    public void SetCrosshair(Sprite crosshair) {
        EnableCrosshair();
        CrossHairImage.sprite = crosshair;
        CrossHairBackground.sprite = crosshair;
    }
    public void DisableCrosshair() {
        CrossHair.SetActive(false);
    }

    // Enemy Count
    public void EnableEnemyCount() {
        EnemyCount.SetActive(true);
    }
    public void SetEnemyCount(int currEnemyCount, int maxEnemyCount) {
        EnableEnemyCount();
        EnemyCountText.text = currEnemyCount + " / " + maxEnemyCount + " Pots";
    }
    public void DisableEnemyCount() {
        EnemyCount.SetActive(false);
    }

    // Weapon toggle
    public void EnableWeaponToggle() {
        WeaponToggle.SetActive(true);
        NextWeaponIcon.sprite = GetIcon(NextWeaponText.text);
        CurrWeaponIcon.sprite = GetIcon(CurrWeaponText.text);
        PrevWeaponIcon.sprite = GetIcon(PrevWeaponText.text);
    }
    public void SetWeaponToggle(string prevWeapon, string currWeapon, string nextWeapon) {
        EnableWeaponToggle();
        NextWeaponText.text = nextWeapon;
        CurrWeaponText.text = currWeapon;
        PrevWeaponText.text = prevWeapon;
        NextWeaponIcon.sprite = GetIcon(nextWeapon);
        CurrWeaponIcon.sprite = GetIcon(currWeapon);
        PrevWeaponIcon.sprite = GetIcon(prevWeapon);
    }
    public void DisableWeaponToggle() {
        WeaponToggle.SetActive(false);
    }
    public Sprite GetIcon(string weaponName)
    {
        Sprite retval = null;

        switch(weaponName)
        {
            case "Sword":
                retval = SwordIcon;
                break;
            case "Bow":
                retval = BowIcon;
                break;
            default:
                retval = SwordIcon;
                break;
        }

        return retval;
    }

    // Weapon Wheel
    public void EnableWeaponWheel() {
        WeaponWheel.SetActive(true);
    }
    public void SetWeaponWheel(string[] weapons) {
        // Supplies Images and Weapon Wheel dynamically arranges them in a circle.
        EnableWeaponWheel();

        WeaponWheelAmo = weapons.Length;

        float fillAmo = 1 / (float)(WeaponWheelAmo);
        float zRot = 0;
        float rotIncrease = 360 / WeaponWheelAmo;

        for (int i = 0; i < WeaponWheelButtons.Length; i++) {
            Button button = WeaponWheelButtons[i];
            Image buttonImage = WeaponWheelButtonImages[i];
            TextMeshProUGUI text = WeaponWheelTexts[i];
            Image image = WeaponWheelIcons[i];

            if (i < WeaponWheelAmo) {
                // Button
                {
                    button.gameObject.SetActive(true);

                    RectTransform rect = button.GetComponent<RectTransform>();
                    Vector3 rot = rect.localRotation.eulerAngles;
                    rot.z = zRot;
                    button.transform.localRotation = Quaternion.Euler(rot);
                }

                // Button Image 
                {
                    buttonImage.fillAmount = fillAmo;
                }
                
                // Weapon Text
                {
                    text.gameObject.SetActive(false);

                    //text.text = weapons[i];

                    //Vector3 rot = text.rectTransform.localRotation.eulerAngles;
                    //rot.z = -zRot;
                    //text.transform.localRotation = Quaternion.Euler(rot);

                    //Vector3 pos = Quaternion.Euler(0, 0, -rotIncrease / 2f) * new Vector3(0, 100, 0);
                    //text.rectTransform.localPosition = pos;
                }

                // Weapon Icon
                {
                    //image.gameObject.SetActive(false);
                    image.sprite = GetIcon(weapons[i]);

                    Vector3 rot = image.rectTransform.localRotation.eulerAngles;
                    rot.z = -zRot;
                    image.transform.localRotation = Quaternion.Euler(rot);

                    Vector3 pos = Quaternion.Euler(0, 0, -rotIncrease / 2f) * new Vector3(0, 100, 0);
                    image.rectTransform.localPosition = pos;
                }
                
                zRot -= rotIncrease;
            } else {
                button.gameObject.SetActive(false);
            }
        }
    }
    public void HighlightWeaponWheel(int index, Vector3 dir) {

        WeaponWheelCursor.localPosition = dir * 100;

        GameObject obj = null;
        if (index != -1) {
            obj = WeaponWheelButtons[index].gameObject;
        }
        eventSystem.SetSelectedGameObject(obj);
    }
    public void DisableWeaponWheel() {
        WeaponWheel.SetActive(false);
    }

    // Boss Health Bar
    public void EnableBossHealthBar() {
        BossHealthBar.SetActive(true);
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
        BossHealthBar.SetActive(false);
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
