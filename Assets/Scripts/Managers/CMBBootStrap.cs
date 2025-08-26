using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Bootstrap: attach to one GameObject in the scene; auto-adds managers and wires events.
/// </summary>
public class CMBBootstrap : MonoBehaviour
{
    public GameState gameState;
    public TierManager tierManager;
    public TaskManager taskManager;
    public UpgradeManager upgradeManager;
    public AchievementManager achievementManager;
    public NarrativeManager narrativeManager;
    public OfflineManager offlineManager;

    private void Awake()
    {
        gameState = gameObject.GetComponent<GameState>() ?? gameObject.AddComponent<GameState>();
        tierManager = gameObject.GetComponent<TierManager>() ?? gameObject.AddComponent<TierManager>();
        taskManager = gameObject.GetComponent<TaskManager>() ?? gameObject.AddComponent<TaskManager>();
        upgradeManager = gameObject.GetComponent<UpgradeManager>() ?? gameObject.AddComponent<UpgradeManager>();
        achievementManager = gameObject.GetComponent<AchievementManager>() ?? gameObject.AddComponent<AchievementManager>();
        narrativeManager = gameObject.GetComponent<NarrativeManager>() ?? gameObject.AddComponent<NarrativeManager>();
        offlineManager = gameObject.GetComponent<OfflineManager>() ?? gameObject.AddComponent<OfflineManager>();

        // init order
        upgradeManager.Initialize();
        taskManager.Initialize();
        achievementManager.Initialize();
        narrativeManager.Initialize();
        tierManager.Initialize(gameState);

        // unlock tier 1 upgrades explicitly
        upgradeManager.UnlockUpgradesForTier(gameState.GetSave().currentTier);

        taskManager.OnTaskCompleted += (def, st) =>
        {
            // check achievements for tasks that unlock them
            if (!string.IsNullOrEmpty(def.unlocksAchievementId))
                achievementManager.TryUnlock(def.unlocksAchievementId);

            // try advancing tier
            tierManager.TryAdvanceTier(taskManager, upgradeManager);
        };

        upgradeManager.OnUpgradePurchased += (u, lvl) =>
        {
            // after purchase, recompute, then try to advance tier (some upgrades are required)
            upgradeManager.RecomputeEffects();
            tierManager.TryAdvanceTier(taskManager, upgradeManager);
        };

        // apply offline if returning from background
        offlineManager.ApplyOfflineIfAny();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            offlineManager.RecordInactiveTime();
            SaveSystem.Save(gameState.GetSave());
        }
        else
        {
            offlineManager.ApplyOfflineIfAny();
            var s = SaveSystem.Load() ?? new SaveData();
            s.lastInactiveBinary = "0";
            SaveSystem.Save(s);
        }
    }

    private void OnApplicationQuit()
    {
        offlineManager.RecordInactiveTime();
        SaveSystem.Save(gameState.GetSave());
    }
}
