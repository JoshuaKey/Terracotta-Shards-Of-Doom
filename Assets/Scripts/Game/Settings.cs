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

    public float MasterVolume = 1.0f;
    public float SoundVolume = 1.0f;
    public float MusicVolume = 1.0f;

    [Range(30, 90)]
    public float FOV = 60.0f;

    // A multiple for how many pixels to move across the screen. 
    // Base is 60 pixels
    [Range(0, 10)]
    public float VerticalSensitivity = 1.0f;

    [Range(0, 10)]
    public float HorizontalSensitivity = 1.0f; 

    public Resolution CurrResolution; // Does not save...
    public FullScreenMode ScreenMode;
    public int GraphicsLevel;

    [NonSerialized] public static Settings instance;
    public static Settings Instance {
        get {
            if (instance == null) { instance = new Settings(); }
            return instance;
        }
    }
    [NonSerialized] public Action OnSave;
    [NonSerialized] public Action OnLoad;
    [NonSerialized] public Action OnUpdate;
    [NonSerialized] public Action OnReset;

    /// <summary>
    /// Creates a copy of the current Settings.
    /// Can then be used to update by reassigning values.
    /// </summary>
    /// <returns>Clone of current Settings</returns>
    public Settings CreateCopy() {
        Settings copy = new Settings();

        copy.InputConfigurationFile = this.InputConfigurationFile;
        copy.IsLeftHanded = this.IsLeftHanded;

        copy.MasterVolume = this.MasterVolume;
        copy.SoundVolume = this.SoundVolume;
        copy.MusicVolume = this.MusicVolume;

        copy.FOV = this.FOV;
        copy.VerticalSensitivity = this.VerticalSensitivity;
        copy.HorizontalSensitivity = this.HorizontalSensitivity;

        copy.CurrResolution = this.CurrResolution;
        copy.ScreenMode = this.ScreenMode;
        copy.GraphicsLevel = this.GraphicsLevel;

        return copy;
    }

    /// <summary>
    /// Changes the Current settings.
    /// 
    /// Invokes OnUpdate Event.
    /// </summary>
    /// <param name="settings">New Settings</param>
    public void UpdateSettings(Settings settings) {
        this.InputConfigurationFile = settings.InputConfigurationFile;
        this.IsLeftHanded = settings.IsLeftHanded;

        this.MasterVolume = settings.MasterVolume;
        this.SoundVolume = settings.SoundVolume;
        this.MusicVolume = settings.MusicVolume;

        this.FOV = settings.FOV;
        this.VerticalSensitivity = settings.VerticalSensitivity;
        this.HorizontalSensitivity = settings.HorizontalSensitivity;

        this.CurrResolution = settings.CurrResolution;
        this.ScreenMode = settings.ScreenMode;
        this.GraphicsLevel = settings.GraphicsLevel;

        // We never actually change anything...

        OnUpdate?.Invoke();
    }
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
    public void Save(string file) {
        Debug.Log("Saving Settings to " + file);

        string data = JsonUtility.ToJson(this);

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
    public void Load(string file) {
        Debug.Log("Loading Settings to " + file);

        if (File.Exists(file)) {
            string json = File.ReadAllText(file);

            Settings data = JsonUtility.FromJson<Settings>(json);

            UpdateSettings(data);

            OnLoad?.Invoke();
        }
    }
    /// <summary>
    /// Updates the Settings with default values. 
    /// 
    /// Invokes OnUpdate and OnReset Event.
    /// </summary>
    public void Reset() {
        Debug.Log("Reseting Settings");

        Settings settings = new Settings();
        settings.InputConfigurationFile = "input.xml";
        settings.IsLeftHanded = false;

        settings.MasterVolume = 1.0f;
        settings.SoundVolume = 1.0f;
        settings.MusicVolume = 1.0f;

        settings.FOV = 60f;
        settings.VerticalSensitivity = 1.0f;
        settings.HorizontalSensitivity = 1.0f;

        settings.CurrResolution = new Resolution();
        settings.CurrResolution.width = 1280;
        settings.CurrResolution.height = 720;
    
        settings.ScreenMode = FullScreenMode.ExclusiveFullScreen; // Full Screen

        settings.GraphicsLevel = 3; // (High, I think...)

        UpdateSettings(settings);

        OnReset?.Invoke();
    }
}
