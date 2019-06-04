using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[Serializable]
public class Settings {
    // Data
    public string InputConfigurationFile = "input.xml";
    public bool IsLeftHanded = false;

    public int DifficultyLevel;
    public bool SkipTutorial;

    public float MasterVolume = 1.0f;
    public float SoundVolume = 1.0f;
    public float MusicVolume = 1.0f;

    public float FOV = 60.0f;
    public float VerticalSensitivity = 1.0f;
    public float HorizontalSensitivity = 1.0f;

    public int ResolutionWidth; 
    public int ResolutionHeight; 
    public FullScreenMode ScreenMode;
    public int GraphicsLevel;

    [NonSerialized] public static Action OnSave;
    [NonSerialized] public static Action<Settings> OnLoad;
    [NonSerialized] public static Action OnReset;

    /// <summary>
    /// Saves the current settings to the specific file. 
    /// 
    /// Do not use.
    /// 
    /// Use Game.SaveSettings() instead.
    /// 
    /// Invokes OnSave Event
    /// </summary>
    /// <param name="file"></param>
    public static void Save(string file) {
        Debug.Log("Saving Settings to " + file);

        Settings settings = new Settings();

        settings.InputConfigurationFile = InputController.Instance.InputConfigurationFile;
        settings.IsLeftHanded = InputController.Instance.IsLeftHanded;
        settings.DifficultyLevel = PauseMenu.Instance.GetDifficulty();
        settings.SkipTutorial = PauseMenu.Instance.GetSkipTutorial();
        AudioManager.Instance.audioMixer.GetFloat("MasterVolume", out settings.MasterVolume);
        AudioManager.Instance.audioMixer.GetFloat("SFXVolume", out settings.SoundVolume);
        AudioManager.Instance.audioMixer.GetFloat("MusicVolume", out settings.MusicVolume);//
        settings.FOV = Player.Instance.camera.fieldOfView;
        settings.VerticalSensitivity = Player.Instance.VerticalRotationSensitivity;
        settings.HorizontalSensitivity = Player.Instance.HorizontalRotationSensitivity;
        settings.ResolutionHeight = Screen.currentResolution.height;
        settings.ResolutionWidth = Screen.currentResolution.width;
        settings.ScreenMode = Screen.fullScreenMode;
        settings.GraphicsLevel = QualitySettings.GetQualityLevel();

        string data = JsonUtility.ToJson(settings, true);

        File.WriteAllText(file, data);

        OnSave?.Invoke();
    }

    /// <summary>
    /// Loads new settings from the specific file. 
    /// 
    /// Do not use.
    /// 
    /// Use Game.LoadSettings() instead.
    /// 
    /// Invokes OnUpdate and OnLoad Event.
    /// </summary>
    /// <param name="file"></param>
    public static void Load(string file) {
        Debug.Log("Loading Settings to " + file);

        if (File.Exists(file)) {
            string json = File.ReadAllText(file);

            Settings data = JsonUtility.FromJson<Settings>(json);

            OnLoad?.Invoke(data);
        }
    }

    /// <summary>
    /// Updates the Settings with default values. 
    /// 
    /// Invokes OnUpdate and OnReset Event.
    /// </summary>
    public void Reset() {
        Debug.Log("Reseting Settings");

        OnReset?.Invoke();
    }
}
