using Luminosity.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    public string PlayerTag = "Player";
    public string PlayerStatsFile = "save.json"; // ?
    public string SettingsFile = "settings.json"; // ?

    public static Game Instance;
    
    void Awake() {
        if(Instance != null) { Destroy(this.gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(this);  
    }

    private void Start() {
        //DialogueSystem.Instance.SetCharacterImage(image...);
        //DialogueSystem.Instance.SetCharacterName("nAVi thE pOt!");
        //DialogueSystem.Instance.QueueDialogue("WhY dO I eXisT?", true);
        //DialogueSystem.Instance.QueueDialogue(new string["WhY dO I eXisT?", "WhYyyYyY!?"], true);

        Settings.OnLoad += OnSettingsLoad;
    }

    private void OnSettingsLoad(Settings settings) {
        Screen.SetResolution(settings.ResolutionWidth, settings.ResolutionHeight, settings.ScreenMode);
        QualitySettings.SetQualityLevel(settings.GraphicsLevel);
    }

    [ContextMenu("Save Player Stats")]
    public void SavePlayerStats() {
        PlayerStats.Save(Application.dataPath + "/" + PlayerStatsFile);
    }
    [ContextMenu("Load Player Stats")]
    public void LoadPlayerStats() {
        PlayerStats.Load(Application.dataPath + "/" + PlayerStatsFile);
    }
    [ContextMenu("Save Settings")]
    public void SaveSettings() {
        Settings.Save(Application.dataPath + "/" + SettingsFile);
    }
    [ContextMenu("Load Settings")]
    public void LoadSettings() {
        Settings.Load(Application.dataPath + "/" + SettingsFile);
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

// Normal Pots - Wether a pot has been defeated. Percentage
//  Enemy Manager iterates across all. Changes material color by multiplier if already killed
//  static Index for current level. Each pot keeps track of its index at start. 
//   Pot tells player stats when it's been killed.
//      - will the order of the pots ever change?
//  Index is determined at build time
// Special Pots
//  ^^^ Index
// Crates

// In Saints row all collectibles had an id.

// In previous game, I had a class which represented all save data. 
// When i saved, I created a new instance, grabbed all nessecary data from other objects and
//  saved it to a file.

// Settings currently holds all of its values. mose of the values could be extrapolated outside.
//  For example, the Camera contains the current FOV. Audio Mixer has Volume.
//  InputConfigurationFile isn't needed (if we only use 1)
//  IsLeftHanded is stored in InputController
//  Sensitivity can be stored in Player
// You can Save and Load by calling its methods or calling Game.SaveSettings(). 
// You can update the settings by creating a clone, reassigning variables then calling UpdateSettings()
// You can also reset them to their default by calling Reset()
// Settings does not currently work. It would need to change the actual values like Camera and Audio.
//  We can use OnUpdate, or do it manually from inside Settings

// We could instead have SettingsData. 
//  Settings UI directly changes all settings. 
//  Can Save, Load or Reset settings.
//  When we save or load. We directly access all nessecary values like FOV in Camera, etc.

// PlayerStats has the Coins, Weapons and Level Data
//  Coins could (should) go in Player
//  Weapons is stored in Player (Unfriendly) (Make it friendly?)
//  

// Settings 
