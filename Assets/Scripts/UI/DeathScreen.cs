using Luminosity.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeathScreen : MonoBehaviour {

    public Image Background;
    public Image ContinueIcon;

    [Header("Other")]
    public EventSystem eventSystem;

    public static DeathScreen Instance;

    private void Awake() {
        if (Instance != null) { Destroy(this.gameObject); return; }
        Instance = this;
    }
    private void Start() {
        if (eventSystem == null) { eventSystem = FindObjectOfType<EventSystem>(); }

        InputManager.ControlSchemesChanged += OnControlSchemeChanged;
        InputManager.PlayerControlsChanged += OnPlayerControlChanged;

        DisableDeathScreen();
    }

    private void OnDestroy() {
        InputManager.ControlSchemesChanged -= OnControlSchemeChanged;
        InputManager.PlayerControlsChanged -= OnPlayerControlChanged;
    }

    private void OnPlayerControlChanged(PlayerID id) { UpdateInputIcons(); }
    private void OnControlSchemeChanged() { UpdateInputIcons(); }
    public void UpdateInputIcons() {
        ContinueIcon.sprite = InputController.Instance.GetActionIcon("UI_Submit");
    }

    // DeathScreen
    public void EnableDeathScreen() {
        this.gameObject.SetActive(true);
    }
    public void SetBackgroundAlpha(float alpha) {
        Color c = Background.color;
        c.a = alpha / 255f;
        Background.color = c;
    }
    public void DisableDeathScreen() {
        this.gameObject.SetActive(false);
    }

}
