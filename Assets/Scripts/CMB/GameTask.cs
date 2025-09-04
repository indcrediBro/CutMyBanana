using System;

namespace CMB
{
    [Serializable]
    public class GameTask
    {
        public TaskType taskType;
        public string description;
        public double targetAmount;
        public double progress;
        public uint experienceReward;
        public bool isCompleted;

        public GameTask(TaskType type, double target, uint reward)
        {
            taskType = type;
            targetAmount = target;
            experienceReward = reward;
            progress = 0;
            isCompleted = false;

            description = type switch
            {
                TaskType.Slice => $"Slice {target} bananas",
                TaskType.Upgrade => $"Buy {target} upgrades",
                _ => "Unknown task"
            };
        }

        public void AddProgress(double amount = 1)
        {
            if (isCompleted) return;

            progress += amount;
            if (progress >= targetAmount)
                CompleteTask();
        }

        private void CompleteTask()
        {
            isCompleted = true;
            // trigger XP gain
            GameEvents.OnExperienceGained?.Invoke(experienceReward);
        }
    }
}