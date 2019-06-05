using Luminosity.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class InputController : MonoBehaviour {
    [Header("Input")]
    public string MouseAndKeyboardSchemeName;
    public string LeftHandedSchemeName;
    public string ControllerSchemeName;

    [Header("Settings")]
    public string InputConfigurationFile = "input.xml";
    public bool IsLeftHanded = false;

#region Input Icons
    [Header("Keyboard Icons")]
    // 4th row
    public Sprite Keyboard_Tab_Sprite;
    public Sprite Keyboard_Q_Sprite;
    public Sprite Keyboard_W_Sprite;
    public Sprite Keyboard_E_Sprite;
    public Sprite Keyboard_R_Sprite;
    public Sprite Keyboard_T_Sprite;
    public Sprite Keyboard_Y_Sprite;
    public Sprite Keyboard_U_Sprite;
    public Sprite Keyboard_I_Sprite;
    public Sprite Keyboard_O_Sprite;
    public Sprite Keyboard_P_Sprite;
    public Sprite Keyboard_LeftBracket_Sprite;
    public Sprite Keyboard_RightBracket_Sprite;
    public Sprite Keyboard_BackSlash_Sprite;
    // 3rd row
    [Space]
    public Sprite Keyboard_CapsLock_Sprite;
    public Sprite Keyboard_A_Sprite;
    public Sprite Keyboard_S_Sprite;
    public Sprite Keyboard_D_Sprite;
    public Sprite Keyboard_F_Sprite;
    public Sprite Keyboard_G_Sprite;
    public Sprite Keyboard_H_Sprite;
    public Sprite Keyboard_J_Sprite;
    public Sprite Keyboard_K_Sprite;
    public Sprite Keyboard_L_Sprite;
    public Sprite Keyboard_SemiColon_Sprite;
    public Sprite Keyboard_SingleQuote_Sprite;
    public Sprite Keyboard_Enter_Sprite;
    // 2nd row
    [Space]
    public Sprite Keyboard_Shift_Sprite;
    public Sprite Keyboard_Z_Sprite;
    public Sprite Keyboard_X_Sprite;
    public Sprite Keyboard_C_Sprite;
    public Sprite Keyboard_V_Sprite;
    public Sprite Keyboard_B_Sprite;
    public Sprite Keyboard_N_Sprite;
    public Sprite Keyboard_M_Sprite;
    public Sprite Keyboard_Comma_Sprite;
    public Sprite Keyboard_Period_Sprite;
    public Sprite Keyboard_Slash_Sprite;
    // 1st row
    [Space]
    public Sprite Keyboard_Control_Sprite;
    public Sprite Keyboard_Alt_Sprite;
    public Sprite Keyboard_Left_Sprite;
    public Sprite Keyboard_Right_Sprite;
    public Sprite Keyboard_Up_Sprite;
    public Sprite Keyboard_Down_Sprite;
    // 5th row
    [Space]
    public Sprite Keyboard_Tilda_Sprite;
    public Sprite Keyboard_1_Sprite;
    public Sprite Keyboard_2_Sprite;
    public Sprite Keyboard_3_Sprite;
    public Sprite Keyboard_4_Sprite;
    public Sprite Keyboard_5_Sprite;
    public Sprite Keyboard_6_Sprite;
    public Sprite Keyboard_7_Sprite;
    public Sprite Keyboard_8_Sprite;
    public Sprite Keyboard_9_Sprite;
    public Sprite Keyboard_0_Sprite;
    public Sprite Keyboard_Dash_Sprite;
    public Sprite Keyboard_Equal_Sprite;
    public Sprite Keyboard_Backspace_Sprite;
    // 6th row
    [Space]
    public Sprite Keyboard_Escape_Sprite;
    public Sprite Keyboard_Home_Sprite;
    public Sprite Keyboard_End_Sprite;
    public Sprite Keyboard_Insert_Sprite;
    public Sprite Keyboard_Delete_Sprite;
    [Space]
    [Header("Xbox Icons")]
    public Sprite Xbox_A_Sprite;
    public Sprite Xbox_X_Sprite;
    public Sprite Xbox_B_Sprite;
    public Sprite Xbox_Y_Sprite;
    public Sprite Xbox_Start_Sprite;
    public Sprite Xbox_Select_Sprite;
    public Sprite Xbox_D_Up_Sprite;
    public Sprite Xbox_D_Down_Sprite;
    public Sprite Xbox_D_Left_Sprite;
    public Sprite Xbox_D_Right_Sprite;
    public Sprite Xbox_RB_Sprite;
    public Sprite Xbox_RT_Sprite;
    public Sprite Xbox_LT_Sprite;
    public Sprite Xbox_LB_Sprite;
    public Sprite Xbox_RightStick_Sprite;
    public Sprite Xbox_LeftStick_Sprite;
    #endregion

    public static InputController Instance;

    private void Awake() {
        if (Instance != null) { Destroy(this.gameObject); return; }
        Instance = this;
    }

    private void Start() {
        StartCoroutine(CheckControllerStatus());

        CheckFile();

        Settings.OnLoad += OnSettingsLoad;
    }

    private void OnDestroy() {
        Settings.OnLoad -= OnSettingsLoad;
    }

    private void OnSettingsLoad(Settings settings) {
        IsLeftHanded = settings.IsLeftHanded;
        if(InputConfigurationFile != settings.InputConfigurationFile) {
            InputManager.Load(Application.dataPath + "/" + InputConfigurationFile);
        }
    }

    public void CheckFile() {
        if(File.Exists(Application.dataPath + "/" + InputConfigurationFile)) {
            InputManager.Load(Application.dataPath + "/" + InputConfigurationFile);
        } else {
            InputManager.Save(Application.dataPath + "/" + InputConfigurationFile);
        }
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

        string currScheme = InputManager.PlayerOneControlScheme.Name;
        if (currScheme != newScheme) {
            InputManager.SetControlScheme(newScheme, PlayerID.One);
            currScheme = newScheme;
            print("Control Scheme: " + newScheme);
        }
    }

    public Sprite GetActionIcon(string action) {
        ControlScheme scheme = InputManager.PlayerOneControlScheme;
        InputAction inputAction = scheme.GetAction(action);
        InputBinding actionBinding = inputAction.GetBinding(0);
        KeyCode code = actionBinding.Positive;

        return GetInputIcon(code);
    }

    public string GetActionText(string action) {
        ControlScheme scheme = InputManager.PlayerOneControlScheme;
        InputAction inputAction = scheme.GetAction(action);
        InputBinding actionBinding = inputAction.GetBinding(0);
        KeyCode code = actionBinding.Positive;

        return GetInputText(code);
    }

    public Sprite GetInputIcon(KeyCode k) {
        string currScheme = InputManager.PlayerOneControlScheme.Name;
        if (currScheme == MouseAndKeyboardSchemeName || currScheme == LeftHandedSchemeName) {
            return GetKeyboardIcon(k);
        } else if (currScheme == ControllerSchemeName) {
            return GetXboxIcon(k);
        }

        throw new KeyNotFoundException("Invalid Key. Check the current Control Scheme and Key Input.");
    }

    public string GetInputText(KeyCode k) {
        string currScheme = InputManager.PlayerOneControlScheme.Name;
        if (currScheme == MouseAndKeyboardSchemeName || currScheme == LeftHandedSchemeName) {
            return GetKeyboardText(k);
        } else if (currScheme == ControllerSchemeName) {
            return GetXboxText(k);
        }

        throw new KeyNotFoundException("Invalid Key. Check the current Control Scheme and Key Input.");
    }

    public Sprite GetKeyboardIcon(KeyCode k) {
        switch (k) {
            case KeyCode.Tab:
                return Keyboard_Tab_Sprite;
            case KeyCode.Q:
                return Keyboard_Q_Sprite;
            case KeyCode.W:
                return Keyboard_W_Sprite;
            case KeyCode.E:
                return Keyboard_E_Sprite;
            case KeyCode.R:
                return Keyboard_R_Sprite;
            case KeyCode.T:
                return Keyboard_T_Sprite;
            case KeyCode.Y:
                return Keyboard_Y_Sprite;
            case KeyCode.U:
                return Keyboard_U_Sprite;
            case KeyCode.I:
                return Keyboard_I_Sprite;
            case KeyCode.O:
                return Keyboard_O_Sprite;
            case KeyCode.P:
                return Keyboard_P_Sprite;
            case KeyCode.LeftCurlyBracket:
            case KeyCode.LeftBracket:
                return Keyboard_LeftBracket_Sprite;
            case KeyCode.RightCurlyBracket:
            case KeyCode.RightBracket:
                return Keyboard_RightBracket_Sprite;
            case KeyCode.Pipe:
            case KeyCode.Backslash:
                return Keyboard_BackSlash_Sprite;
            case KeyCode.CapsLock:
                return Keyboard_CapsLock_Sprite;
            case KeyCode.A:
                return Keyboard_A_Sprite;
            case KeyCode.S:
                return Keyboard_S_Sprite;
            case KeyCode.D:
                return Keyboard_D_Sprite;
            case KeyCode.F:
                return Keyboard_F_Sprite;
            case KeyCode.G:
                return Keyboard_G_Sprite;
            case KeyCode.H:
                return Keyboard_H_Sprite;
            case KeyCode.J:
                return Keyboard_J_Sprite;
            case KeyCode.K:
                return Keyboard_K_Sprite;
            case KeyCode.L:
                return Keyboard_L_Sprite;
            case KeyCode.Colon:
            case KeyCode.Semicolon:
                return Keyboard_SemiColon_Sprite;
            case KeyCode.DoubleQuote:
            case KeyCode.Quote:
                return Keyboard_SingleQuote_Sprite;
            case KeyCode.KeypadEnter:
                return Keyboard_Enter_Sprite;
            case KeyCode.LeftShift:
            case KeyCode.RightShift:
                return Keyboard_Shift_Sprite;
            case KeyCode.Z:
                return Keyboard_Z_Sprite;
            case KeyCode.X:
                return Keyboard_X_Sprite;
            case KeyCode.C:
                return Keyboard_C_Sprite;
            case KeyCode.V:
                return Keyboard_V_Sprite;
            case KeyCode.B:
                return Keyboard_B_Sprite;
            case KeyCode.N:
                return Keyboard_N_Sprite;
            case KeyCode.M:
                return Keyboard_M_Sprite;
            case KeyCode.Less:
            case KeyCode.Comma:
                return Keyboard_Comma_Sprite;
            case KeyCode.Greater:
            case KeyCode.Period:
                return Keyboard_Period_Sprite;
            case KeyCode.Slash:
            case KeyCode.Question:
                return Keyboard_Slash_Sprite;
            case KeyCode.LeftControl:
            case KeyCode.RightControl:
                return Keyboard_Control_Sprite;
            case KeyCode.LeftAlt:
            case KeyCode.RightAlt:
                return Keyboard_Alt_Sprite;
            case KeyCode.LeftArrow:
                return Keyboard_Left_Sprite;
            case KeyCode.RightArrow:
                return Keyboard_Right_Sprite;
            case KeyCode.UpArrow:
                return Keyboard_Up_Sprite;
            case KeyCode.DownArrow:
                return Keyboard_Down_Sprite;
            case KeyCode.Tilde:
            case KeyCode.BackQuote:
                return Keyboard_Tilda_Sprite;
            case KeyCode.Alpha1:
                return Keyboard_1_Sprite;
            case KeyCode.Alpha2:
                return Keyboard_2_Sprite;
            case KeyCode.Alpha3:
                return Keyboard_3_Sprite;
            case KeyCode.Alpha4:
                return Keyboard_4_Sprite;
            case KeyCode.Alpha5:
                return Keyboard_5_Sprite;
            case KeyCode.Alpha6:
                return Keyboard_6_Sprite;
            case KeyCode.Alpha7:
                return Keyboard_7_Sprite;
            case KeyCode.Alpha8:
                return Keyboard_8_Sprite;
            case KeyCode.Alpha9:
                return Keyboard_9_Sprite;
            case KeyCode.Alpha0:
                return Keyboard_0_Sprite;
            case KeyCode.Minus:
            case KeyCode.Underscore:
                return Keyboard_Dash_Sprite;
            case KeyCode.Equals:
            case KeyCode.Plus:
                return Keyboard_Equal_Sprite;
            case KeyCode.Backspace:
                return Keyboard_Backspace_Sprite;
            case KeyCode.Home:
                return Keyboard_Home_Sprite;
            case KeyCode.End:
                return Keyboard_End_Sprite;
            case KeyCode.Insert:
                return Keyboard_Insert_Sprite;
            case KeyCode.Delete:
                return Keyboard_Delete_Sprite;
            case KeyCode.Escape:
                return Keyboard_Escape_Sprite;
        }
        print("Unknown Keycode: " + k);
        return null;
    }

    public string GetKeyboardText(KeyCode k) {
        return k == KeyCode.None ? "" : k.ToString();
    }

    public Sprite GetXboxIcon(KeyCode k) {
        switch (k) {
            case KeyCode.Joystick1Button0:
                return Xbox_A_Sprite;
            case KeyCode.Joystick1Button1:
                return Xbox_B_Sprite;
            case KeyCode.Joystick1Button2:
                return Xbox_X_Sprite;
            case KeyCode.Joystick1Button3:
                return Xbox_Y_Sprite;
            case KeyCode.Joystick1Button4:
                return Xbox_LB_Sprite;
            case KeyCode.Joystick1Button5:
                return Xbox_RB_Sprite;
            case KeyCode.Joystick1Button6:
                return Xbox_Select_Sprite;
            case KeyCode.Joystick1Button7:
                return Xbox_Start_Sprite;
        }

        print("Unknown Keycode: " + k);
        return null;
    }

    public string GetXboxText(KeyCode k) {
        switch (k) {
            case KeyCode.Joystick1Button0:
                return "A";
            case KeyCode.Joystick1Button1:
                return "B";
            case KeyCode.Joystick1Button2:
                return "X";
            case KeyCode.Joystick1Button3:
                return "Y";
            case KeyCode.Joystick1Button4:
                return "LB";
            case KeyCode.Joystick1Button5:
                return "RB";
            case KeyCode.Joystick1Button6:
                return "Select";
            case KeyCode.Joystick1Button7:
                return "Start";
        }

        print("Unknown Keycode: " + k);
        return "";
    }
}
