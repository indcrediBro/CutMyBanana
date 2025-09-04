using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CMB
{
    public class TaskItemUI : MonoBehaviour
    {
        public TMP_Text descriptionText;
        public Slider progressSlider;

        private GameTask boundTask;

        public void Setup(GameTask task)
        {
            boundTask = task;

            if (descriptionText != null)
                descriptionText.text = task.description + $" (+{task.experienceReward} XP)";

            if (progressSlider != null)
            {
                progressSlider.maxValue = (float)task.targetAmount;
                progressSlider.value = (float)task.progress;
            }
        }

        public void UpdateProgress(double current, double target)
        {
            if (progressSlider != null)
            {
                progressSlider.maxValue = (float)target;
                progressSlider.value = (float)current;
            }
        }
    }
}