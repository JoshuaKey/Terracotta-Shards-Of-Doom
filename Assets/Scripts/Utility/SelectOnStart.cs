using Luminosity.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectOnStart : MonoBehaviour {

    public Selectable selectable;

    private void Awake() {
        if (selectable == null) { selectable = GetComponentInChildren<Selectable>(); }
    }

    private void OnEnable() {
        if(selectable != null && InputController.Instance != null) {
            if(InputManager.PlayerOneControlScheme.Name == InputController.Instance.ControllerSchemeName) {
                selectable.Select();
            }
        }
    }

}
