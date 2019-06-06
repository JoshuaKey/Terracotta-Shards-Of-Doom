using Luminosity.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectOnStart : MonoBehaviour {

    public Selectable selectable;

    private void Awake() {

    }

    //private void Update() {
    //    if (selectable != null && InputController.Instance != null) {
    //        if (InputManager.PlayerOneControlScheme.Name == InputController.Instance.ControllerSchemeName) {
    //            print("Selecting " + selectable.name);
    //            selectable.Select();
    //        }
    //    }
    //}

    private void OnEnable() {
        if (selectable == null) { selectable = GetComponentInChildren<Selectable>(); }
        InputManager.ControlSchemesChanged += OnControlSchemeChanged;
        InputManager.PlayerControlsChanged += OnPlayerControlChanged;
        UpdateSelectable();
    }

    private void OnDisable() {
        InputManager.ControlSchemesChanged -= OnControlSchemeChanged;
        InputManager.PlayerControlsChanged -= OnPlayerControlChanged;
    }

    private void OnPlayerControlChanged(PlayerID id) { UpdateSelectable(); }
    private void OnControlSchemeChanged() { UpdateSelectable(); }
    public void UpdateSelectable() {
        if (selectable != null && InputController.Instance != null) {
            if (InputManager.PlayerOneControlScheme.Name == InputController.Instance.ControllerSchemeName) {
                print("Selecting " + selectable.name);
                selectable.Select();
                selectable.OnSelect(null);
            }
        }
    }

    private IEnumerator InputCheck() {
        WaitForSecondsRealtime wait = new WaitForSecondsRealtime(2.0f);

        while (selectable == null || InputController.Instance == null) {
            yield return wait;
        }

        if (InputManager.PlayerOneControlScheme.Name == InputController.Instance.ControllerSchemeName) {
            print("Selecting " + selectable.name);
            selectable.Select();
        }
        print("Here");
        print(InputManager.PlayerOneControlScheme.Name);
    }

    // What if we had a method that returns a method? ReturnMethod()(); What is wrong with me?...

}
