using System;
using System.Collections.Generic;
using UnityEngine;

namespace CMB
{
    public enum TaskType
    {
        Slice,
        Upgrade
    }
    
    [System.Serializable]
    public class TaskManager
    {
        public List<GameTask> activeTasks;
        public List<GameTask> completedTasks;
        private Dictionary<TaskType, int> taskProgressionCounters;

        private System.Random rng;

        public TaskManager()
        {
            activeTasks = new List<GameTask>();
            completedTasks = new List<GameTask>();
            rng = new System.Random();
            taskProgressionCounters = new Dictionary<TaskType, int>();

            foreach (TaskType type in Enum.GetValues(typeof(TaskType)))
                taskProgressionCounters[type] = 0;
            
            SubscribeToEvents();
            GenerateTasks();
        }

        public void _OnDestroy() => UnsubscribeFromEvents();

        #region Task Generation
        public void GenerateTasks()
        {
            activeTasks.Clear();

            foreach (TaskType type in Enum.GetValues(typeof(TaskType)))
            {
                var task = GenerateTask(type);
                activeTasks.Add(task);
            }
        }

        private GameTask GenerateTask(TaskType type)
        {
            int completedCount = taskProgressionCounters[type];
            double target = 0;
            int reward = 0;

            switch (type)
            {
                case TaskType.Slice:
                    target = GameSettings.sliceBaseTarget + completedCount * GameSettings.sliceTargetIncrement;
                    reward = GameSettings.sliceBaseReward + completedCount * GameSettings.sliceRewardIncrement;
                    break;

                case TaskType.Upgrade:
                    target = GameSettings.upgradeBaseTarget + completedCount * GameSettings.upgradeTargetIncrement;
                    reward = GameSettings.upgradeBaseReward + completedCount * GameSettings.upgradeRewardIncrement;
                    break;
            }

            return new GameTask(type, target, (uint)reward);
        }

        #endregion

        #region Events
        private void SubscribeToEvents()
        {
            GameEvents.OnSlice += () => UpdateProgress(TaskType.Slice, 1);
            GameEvents.OnUpgradePurchased += (_) => UpdateProgress(TaskType.Upgrade, 1);
        }

        private void UnsubscribeFromEvents()
        {
            GameEvents.OnSlice -= () => UpdateProgress(TaskType.Slice, 1);
            GameEvents.OnUpgradePurchased -= (_) => UpdateProgress(TaskType.Upgrade, 1);
        }

        #endregion

        private void UpdateProgress(TaskType type, double amount)
        {
            var task = activeTasks.Find(t => t.taskType == type && !t.isCompleted);
            if (task == null) return;

            task.AddProgress(amount);
            GameManager.Instance.m_uiManager.UpdateTaskProgress(task);

            if (task.isCompleted)
            {
                
                completedTasks.Add(task);
                activeTasks.Remove(task);
                taskProgressionCounters[type]++; 
                GameManager.Instance.m_popupManager.Show("Task Completed!",task.description);

                activeTasks.Add(GenerateTask(type));
                GameEvents.OnTaskListChanged?.Invoke(); // refresh UI
            }
        }

    }
}
