using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHud : MonoBehaviour {

    public TextMeshProUGUI InteractText;

    public void SetInteractText(string button, string name) {
        InteractText.gameObject.SetActive(true);
        InteractText.text = "Press '" + button + "' to interact with '" + name + "'";
    }

    public void DisableInteractText() {
        InteractText.gameObject.SetActive(false);
    }
}
