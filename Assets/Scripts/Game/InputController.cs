using Luminosity.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {
    [Header("Input")]
    public string MouseAndKeyboardSchemeName;
    public string LeftHandedSchemeName;
    public string ControllerSchemeName;
    public bool IsLeftHanded = false;

    private string currScheme = "";

    private void Start() {
        StartCoroutine(CheckControllerStatus());
    }

    private IEnumerator CheckControllerStatus() {
        WaitForSeconds wait = new WaitForSeconds(2.0f);

        while (true) {
            CheckInput();

            yield return wait;
        }
    }

    public void CheckInput() {
        string newScheme = "";
        if (Input.GetJoystickNames().Length > 0 && Input.GetJoystickNames()[0] != "") {
            newScheme = ControllerSchemeName;
        } else {
            newScheme = IsLeftHanded ? LeftHandedSchemeName : MouseAndKeyboardSchemeName;
        }

        if (currScheme != newScheme) {
            InputManager.SetControlScheme(newScheme, PlayerID.One);
            currScheme = newScheme;
            print("Control Scheme: " + newScheme);
        }
    }
}
