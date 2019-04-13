using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonLever : MonoBehaviour {

    public Cannon cannon;
    public Interactable interactable;
    public 

    // Start is called before the first frame update
    void Start() {
        if (interactable == null) { interactable = GetComponentInChildren<Interactable>(true); }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
