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
public class LevelData {
    public int TotalPots;
    public StringBoolDictionary CollectedPots = new StringBoolDictionary();
    public StringBoolDictionary SpecialPots = new StringBoolDictionary();
    public bool HasDestroyedCrate;
}

[Serializable]
public class PlayerStats {
    public int Coins = 0;
    public StringBoolDictionary Weapons = new StringBoolDictionary();
    public StringLevelDictionary Levels = new StringLevelDictionary();

    [NonSerialized] private static PlayerStats instance;
    public static PlayerStats Instance {
        get {
            if (instance == null) { instance = new PlayerStats(); }
            return instance;
        }
    }
    [NonSerialized] public Action OnSave;
    [NonSerialized] public Action OnLoad;
    //[NonSerialized] public Action OnUpdate;
    [NonSerialized] public Action OnReset;

    /// <summary>
    /// Changes the Current Player Stats.
    /// 
    /// Invokes OnUpdate Event.
    /// </summary>
    /// <param name="settings">New Player Stats</param>
    private void UpdatePlayerStats(PlayerStats stats) {
        this.Coins = stats.Coins;

        this.Weapons.Clear();
        foreach (KeyValuePair<string, bool> data in stats.Weapons) {
            this.Weapons.Add(data.Key, data.Value);
        }

        this.Levels.Clear();
        foreach (KeyValuePair<string, LevelData> levelData in stats.Levels) {
            LevelData oldLevel = levelData.Value;
            LevelData level = new LevelData();

            level.TotalPots = oldLevel.TotalPots;

            foreach (KeyValuePair<string, bool> data in oldLevel.CollectedPots) {
                level.CollectedPots.Add(data.Key, data.Value);
            }

            foreach (KeyValuePair<string, bool> data in oldLevel.SpecialPots) {
                level.SpecialPots.Add(data.Key, data.Value);
            }

            level.HasDestroyedCrate = levelData.Value.HasDestroyedCrate;

            this.Levels.Add(levelData.Key, level);
        }
    }

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
    public void Save(string file) {
        Debug.Log("Saving Player Stats to " + file);

        string data = JsonUtility.ToJson(this);

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
    /// Invokes OnUpdate and OnLoad Event.
    /// </summary>
    /// <param name="file"></param>
    public void Load(string file) {
        Debug.Log("Loading Player Stats to " + file);

        if (File.Exists(file)) {
            string json = File.ReadAllText(file);

            PlayerStats data = JsonUtility.FromJson<PlayerStats>(json);

            UpdatePlayerStats(data);

            OnLoad?.Invoke();
        }
    }
    /// <summary>
    /// Updates the Player Stats with default values. 
    /// 
    /// Invokes OnUpdate and OnReset Event.
    /// </summary>
    public void Reset() {
        Debug.Log("Reseting PlayerStats");

    }

}
