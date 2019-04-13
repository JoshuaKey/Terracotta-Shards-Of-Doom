using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//[RequireComponent(typeof(Collider))] -- Or in children
public class Interactable : MonoBehaviour {

    public bool CanInteract;

    public Action OnInteract;

    public void Interact() {
        if (CanInteract) {
            OnInteract?.Invoke();
            print("Interacted with " + this.name);
        }
    }

    public void Subscribe(Action action) {
        OnInteract += action;
    }
    public void UnSubscribe(Action action) {
        OnInteract -= action;
    }
}
