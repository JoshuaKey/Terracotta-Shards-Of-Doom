using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class StringBoolDictionary : SerializableDictionaryBase<string, bool> { }
[Serializable]
public class StringLevelDictionary : SerializableDictionaryBase<string, LevelData> { }

[Serializable]
public class PlayerStats {
    public int Coins = 0;
    public string CurrentLevel;
    public List<string> Weapons = new List<string>();
    public StringLevelDictionary Levels = new StringLevelDictionary();

    [NonSerialized] public static Action OnSave;
    [NonSerialized] public static Action<PlayerStats> OnLoad;
    [NonSerialized] public static Action OnReset;

    /// <summary>
    /// Saves the current Player Stats to the specific file. 
    /// 
    /// Do not use.
    /// 
    /// Use Game.SavePlayerStats() instead.
    /// 
    /// Invokes OnSave Event
    /// </summary>
    /// <param name="file"></param>
    public static void Save(string file) {
        Debug.Log("Saving Player Stats to " + file);

        PlayerStats stats = new PlayerStats();
    
        stats.Coins = Player.Instance.Coins;
        stats.CurrentLevel = LevelManager.Instance.GetLevelName();
        stats.Weapons = new List<string>();
        Player.Instance.weapons.ForEach(x => {
            if(x is AdvancedWeapon) {
                AdvancedWeapon w = (AdvancedWeapon)x;
                stats.Weapons.Add(w.OldWeaponName);
            }
            stats.Weapons.Add(x.name);
        });

        stats.Levels = LevelManager.Instance.Levels;

        string data = JsonUtility.ToJson(stats, true);

        File.WriteAllText(file, data);

        OnSave?.Invoke();
    }

    /// <summary>
    /// Loads new Player Stats from the specific file. 
    /// 
    /// Do not use.
    /// 
    /// Use Game.LoadPlayerStats() instead.
    /// 
    /// Invokes OnLoad Event.
    /// </summary>
    /// <param name="file"></param>
    public static void Load(string file) {
        Debug.Log("Loading Player Stats to " + file);

        if (File.Exists(file)) {
            string json = File.ReadAllText(file);

            PlayerStats data = JsonUtility.FromJson<PlayerStats>(json);

            OnLoad?.Invoke(data);
        }
    }

    /// <summary>
    /// Updates the Player Stats with default values. 
    /// 
    /// Invokes OnReset Event.
    /// </summary>
    public void Reset() {
        Debug.Log("Reseting PlayerStats");

        OnReset?.Invoke();
    }

}
