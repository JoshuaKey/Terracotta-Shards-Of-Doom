using UnityEngine;

public static class InputExt {

    public static bool GetAxisAsButton(string name) {
        return Input.GetAxis(name) > 0.0f;
    }

}

