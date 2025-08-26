using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public double currency = 0;
    public double totalEarned = 0;
    public double totalSlices = 0;
    public double perSlice = 1.0; // manual slice base value
    public double cps = 0.0;
    public double walletCapacity = 0.0; // offline wallet capacity
    public int currentTier = 1;
    public int totalUpgradePurchases = 0;
    public int totalUpgradesPurchasedCount = 0; // tracking total count
    public List<UpgradeState> upgrades = new List<UpgradeState>();
    public List<TaskState> tasks = new List<TaskState>();
    public List<string> completedAchievements = new List<string>();
    public string lastInactiveBinary = "0";
    public long lastQuitTime;
}

[Serializable]
public class UpgradeState
{
    public string id;
    public int level;
    public bool unlocked; // explicitly unlocked by achievement or initialization
}

[Serializable]
public class TaskState
{
    public string id;
    public bool completed;
    public int progress;
}

public static class SaveSystem
{
    private const string PlayerPrefsKey = "CutMyBanana_SaveV1";
    private static string FilePath => Path.Combine(Application.persistentDataPath, "cmb_save_v1.json");

    public static void Save(SaveData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);
#if UNITY_WEBGL && !UNITY_EDITOR
            PlayerPrefs.SetString(PlayerPrefsKey, json);
            PlayerPrefs.Save();
#else
            File.WriteAllText(FilePath, json);
#endif
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[SaveSystem] Save failed: " + ex.Message);
        }
    }

    public static SaveData Load()
    {
        try
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            if (!PlayerPrefs.HasKey(PlayerPrefsKey)) return null;
            string json = PlayerPrefs.GetString(PlayerPrefsKey);
            return JsonUtility.FromJson<SaveData>(json);
#else
            if (!File.Exists(FilePath)) return null;
            string json = File.ReadAllText(FilePath);
            return JsonUtility.FromJson<SaveData>(json);
#endif
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[SaveSystem] Load failed: " + ex.Message);
            return null;
        }
    }

    public static void Clear()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        PlayerPrefs.DeleteKey(PlayerPrefsKey);
        PlayerPrefs.Save();
#else
        if (File.Exists(FilePath)) File.Delete(FilePath);
#endif
        Debug.Log("[SaveSystem] Save cleared.");
    }
}
