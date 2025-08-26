using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// TierManager: controls which tier is active; unlocks upgrades for the tier when tier is reached.
/// </summary>
public class TierManager : MonoBehaviour
{
    public int maxTier = 10;
    public GameState state;

    public event Action<int> OnTierUnlocked;

    public void Initialize(GameState gs)
    {
        state = gs;
    }

    public int GetCurrentTier() => state.GetSave().currentTier;

    public void TryAdvanceTier(TaskManager taskManager, UpgradeManager upgradeManager)
    {
        var save = state.GetSave();
        int cur = save.currentTier;
        if (cur >= maxTier) return;

        // Condition: all tasks in current tier completed
        var tasks = taskManager.GetTasksForTier(cur);
        bool tasksDone = tasks.All(t => taskManager.IsCompleted(t.id));

        // Condition: all upgrades in current tier that have requiredLevelForTierComplete > 0
        var ups = upgradeManager.GetUpgradesForTier(cur);
        bool upgradesDone = true;
        foreach (var u in ups)
        {
            if (u.requiredLevelForTierComplete > 0)
            {
                int lvl = upgradeManager.GetUpgradeLevel(u.id);
                if (lvl < u.requiredLevelForTierComplete)
                {
                    upgradesDone = false;
                    break;
                }
            }
        }

        if (tasksDone && upgradesDone)
        {
            save.currentTier = Mathf.Clamp(cur + 1, 1, maxTier);
            SaveSystem.Save(save);
            upgradeManager.UnlockUpgradesForTier(save.currentTier);
            OnTierUnlocked?.Invoke(save.currentTier);
            PopupManager.Instance.Show("Tier unlocked", $"Tier {save.currentTier} unlocked! New tasks & upgrades available.");
        }
    }
}