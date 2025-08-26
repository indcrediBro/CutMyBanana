using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// AchievementManager: simple unlock + persist; unlocking may call upgrade unlocks & narrative
/// </summary>
public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance { get; private set; }
    public List<AchievementSO> all;
    Dictionary<string, AchievementSO> lookup = new Dictionary<string, AchievementSO>();
    HashSet<string> unlocked;

    private void Awake() { Instance = this; }

    public void Initialize()
    {
        lookup = all.ToDictionary(a => a.id, a => a);
        var saved = SaveSystem.Load() ?? new SaveData();
        unlocked = new HashSet<string>(saved.completedAchievements ?? new List<string>());
    }

    public void TryUnlock(string id)
    {
        if (!lookup.ContainsKey(id)) return;
        if (unlocked.Contains(id)) return;
        var a = lookup[id];
        unlocked.Add(id);
        var save = SaveSystem.Load() ?? new SaveData();
        if (a.rewardCurrency > 0) save.currency += a.rewardCurrency;
        save.completedAchievements = unlocked.ToList();
        SaveSystem.Save(save);

        if (a.unlocksUpgradeIds != null)
        {
            foreach (var up in a.unlocksUpgradeIds) UpgradeManager.Instance.UnlockUpgradeByAchievement(up);
        }
        if (!string.IsNullOrEmpty(a.unlocksNarrativeId))
        {
            var nm = FindFirstObjectByType<NarrativeManager>();
            nm?.PlayNarrative(a.unlocksNarrativeId);
        }
        PopupManager.Instance.Show("Achievement unlocked", $"{a.title}: {a.description}");
    }
}