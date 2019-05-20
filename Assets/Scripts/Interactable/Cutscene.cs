using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutscene : MonoBehaviour
{
    private void Start()
    {
        Interactable interactble = GetComponent<Interactable>();
        if (interactble == null) throw new System.Exception("A Cutscene tried to find an Interactable but couldn't.");

        interactble.Subscribe(OnInteract);
    }

    public void OnInteract()
    {
        StartCoroutine(PlayCutscene());
    }

    IEnumerator PlayCutscene()
    {
        throw new System.NotImplementedException();
    }
}
