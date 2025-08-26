using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public List<TaskSO> allTasks;
    Dictionary<string, TaskSO> lookup = new Dictionary<string, TaskSO>();
    Dictionary<string, TaskState> runtime;

    public event Action<TaskSO, TaskState> OnTaskCompleted;

    private int totalEarnedCounter = 0;
    private int totalUpgradesPurchased = 0;

    public void Initialize()
    {
        GameEvents.OnBananasEarned += HandleBananasEarned;
        GameEvents.OnUpgradePurchased += HandleUpgradePurchased;

        lookup = allTasks.ToDictionary(t => t.id, t => t);

        var saved = SaveSystem.Load() ?? new SaveData();
        runtime = new Dictionary<string, TaskState>();

        // load existing task states
        if (saved.tasks != null)
        {
            foreach (var ts in saved.tasks)
                runtime[ts.id] = ts;
        }

        // ensure all tasks exist
        foreach (var t in allTasks)
        {
            if (!runtime.ContainsKey(t.id))
                runtime[t.id] = new TaskState { id = t.id, completed = false, progress = 0 };
        }

        // restore counters
        totalEarnedCounter =(int) saved.totalEarned;
        totalUpgradesPurchased = saved.totalUpgradesPurchasedCount;

        SaveRuntime();
    }

    public IEnumerable<TaskSO> GetTasksForTier(int tier) => allTasks.Where(t => t.tier == tier);

    public IEnumerable<(TaskSO def, TaskState state)> GetActiveTaskStates(int tier)
    {
        foreach (var def in GetTasksForTier(tier))
        {
            if (runtime.TryGetValue(def.id, out var st)) yield return (def, st);
        }
    }

    public bool IsCompleted(string id)
    {
        return runtime.ContainsKey(id) && runtime[id].completed;
    }

    // add progress only to tasks of the active tier
    public void AddProgressForTier(int tier, Func<TaskSO, bool> predicate, int amount = 1)
    {
        foreach (var def in GetTasksForTier(tier).Where(predicate))
            AddProgress(def.id, amount);
    }

    public void AddProgress(string taskId, int amount = 1)
    {
        if (!runtime.ContainsKey(taskId)) return;
        var st = runtime[taskId];
        if (st.completed) return;

        var def = lookup[taskId];
        st.progress += amount;

        if (st.progress >= def.target)
        {
            st.completed = true;
            // reward currency
            var s = SaveSystem.Load() ?? new SaveData();
            s.currency += def.rewardCurrency;
            SaveSystem.Save(s);

            OnTaskCompleted?.Invoke(def, st);
            PopupManager.Instance.Show("Task complete", $"{def.title} complete! +{def.rewardCurrency} bananas");
        }

        SaveRuntime();
    }

    private void HandleBananasEarned(int amount)
    {
        int tier = FindFirstObjectByType<TierManager>().GetCurrentTier();
        totalEarnedCounter += amount;

        foreach (var (def, st) in GetActiveTaskStates(tier))
        {
            if (def.title.ToLower().Contains("earn") && !st.completed)
            {
                st.progress = totalEarnedCounter;
                if (st.progress >= def.target)
                {
                    st.completed = true;
                    RewardTask(def, st);
                }
            }
        }
        SaveRuntime();
    }

    private void HandleUpgradePurchased(string upgradeId)
    {
        int tier = FindFirstObjectByType<TierManager>().GetCurrentTier();
        totalUpgradesPurchased++;

        foreach (var (def, st) in GetActiveTaskStates(tier))
        {
            if (def.title.ToLower().Contains("purchase") && !st.completed)
            {
                st.progress = totalUpgradesPurchased;
                if (st.progress >= def.target)
                {
                    st.completed = true;
                    RewardTask(def, st);
                }
            }
        }
        SaveRuntime();
    }

    private void RewardTask(TaskSO def, TaskState st)
    {
        var s = SaveSystem.Load() ?? new SaveData();
        s.currency += def.rewardCurrency;
        SaveSystem.Save(s);

        OnTaskCompleted?.Invoke(def, st);
        PopupManager.Instance.Show("Task complete", $"{def.title} complete! +{def.rewardCurrency} bananas");
    }

    private void SaveRuntime()
    {
        var s = SaveSystem.Load() ?? new SaveData();
        s.tasks = runtime.Values.ToList();
        s.totalEarned = totalEarnedCounter;
        s.totalUpgradesPurchasedCount = totalUpgradesPurchased;
        SaveSystem.Save(s);
    }
}
