using Luminosity.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    public string PlayerTag = "Player";
    public string PlayerStatsFile = "save1.dat"; // ?
    public string SettingsFile = "settings.config"; // ?
    public bool LoadPlayerStatsOnStart = false;
    public bool LoadSettingsOnStart = false;

    public static Game Instance;
    
    void Awake() {
        if(Instance != null) { Destroy(this.gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(this);   
    }

    private void Start() {
        if (LoadPlayerStatsOnStart) {
            PlayerStats.Instance.Load(PlayerStatsFile);
        }
        if (LoadSettingsOnStart) {
            Settings.Instance.Load(SettingsFile);
        }
    }

    [ContextMenu("Save Player Stats")]
    public void SavePlayerStats() {
        PlayerStats.Instance.Save(PlayerStatsFile);
    }
    [ContextMenu("Load Player Stats")]
    public void LoadPlayerStats() {
        PlayerStats.Instance.Load(PlayerStatsFile);
    }
    [ContextMenu("Save Settings")]
    public void SaveSettings() {
        Settings.Instance.Save(SettingsFile);
    }
    [ContextMenu("Load Settings")]
    public void LoadSettings() {
        Settings.Instance.Load(SettingsFile);
    }
}

// Player Stats contains persistent save data.
// Settings contains the configurable data that impacts how you play the game.
// Player Stats:
//  Coins - int
//  Weapons - bool[]
//  Lvl: Special Pots - bool[]
//  Lvl: Collectable Pots - bool[]
//  Lvl: Completed - bool
//  Lvl: Crate - bool
// Settings
//  Input Config File - string
//  Master Volume - float
//  Music Volume - float
//  Sound Volume - float
//  IsLeftHanded - bool
//  FOV - float
//  Input Configuration - Input Manager
//  ScreenResolution (Screen.currentResolution) - Resolution
//  FullScreen (Screen.fullScreenMode) - FullScreenMode
//  GraphicsLevel (QualitySettings.SetQualityLevel) - int

// How do we want to save the data? Binary, Json
