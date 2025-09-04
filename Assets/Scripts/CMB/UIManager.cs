using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace CMB
{
    [System.Serializable]
    public class UIManager
    {
        [Header("Core UI")]
        public TMP_Text currencyText;
        public TMP_Text sliceCountText;
        public TMP_Text levelText;
        public TMP_Text experienceText;
        public Slider experienceSlider;

        [Header("Tasks UI")]
        public Transform taskListContainer;   // parent object with VerticalLayoutGroup
        public GameObject taskItemPrefab;     // prefab for a single task row
        private Dictionary<GameTask, TaskItemUI> taskUIBindings;

        public void _OnAwake()
        {
            taskUIBindings = new Dictionary<GameTask, TaskItemUI>();

            SubscribeToEvents();
            UpdateCurrencyDisplay();
            UpdateSliceCountDisplay();
            UpdateExperienceDisplay(0);

            RefreshTaskUI();
        }

        public void _OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        #region Events
        public void SubscribeToEvents()
        {
            GameEvents.OnCurrencyGained += UpdateCurrencyDisplay;
            GameEvents.OnSlice += UpdateSliceCountDisplay;
            GameEvents.OnExperienceGained += UpdateExperienceDisplay;

            // Custom hook when task list changes
            GameEvents.OnTaskListChanged += RefreshTaskUI;
        }

        public void UnsubscribeFromEvents()
        {
            GameEvents.OnCurrencyGained -= UpdateCurrencyDisplay;
            GameEvents.OnSlice -= UpdateSliceCountDisplay;
            GameEvents.OnExperienceGained -= UpdateExperienceDisplay;

            GameEvents.OnTaskListChanged -= RefreshTaskUI;
        }
        #endregion

        #region Display Updates
        private void UpdateCurrencyDisplay()
        {
            if (currencyText != null)
                currencyText.text = $"Money: {GameManager.Instance.m_playerData.currentMoney:F2}";
        }

        private void UpdateSliceCountDisplay()
        {
            if (sliceCountText != null)
                sliceCountText.text = $"Slices: {GameManager.Instance.m_playerData.currentSliceCount}";
        }

        private void UpdateExperienceDisplay(uint _exp)
        {
            if (levelText != null && experienceSlider != null)
            {
                levelText.text = $"Level: {GameManager.Instance.m_experienceData.playerLevel}";
                experienceText.text =
                    $"XP: {GameManager.Instance.m_experienceData.currentExperience} / {GameManager.Instance.m_experienceData.GetExperienceForNextLevel()}";
                experienceSlider.value = GameManager.Instance.m_experienceData.currentExperience;
                experienceSlider.maxValue = GameManager.Instance.m_experienceData.GetExperienceForNextLevel();
            }
        }

        public void RefreshTaskUI()
        {
            // Clear old bindings
            foreach (Transform child in taskListContainer)
                GameObject.Destroy(child.gameObject);

            taskUIBindings.Clear();

            // Rebuild UI for all active tasks
            foreach (var task in GameManager.Instance.m_taskManager.activeTasks)
            {
                var go = GameObject.Instantiate(taskItemPrefab, taskListContainer);
                var itemUI = go.GetComponent<TaskItemUI>();

                itemUI.Setup(task);
                taskUIBindings.Add(task, itemUI);
            }
        }

        public void UpdateTaskProgress(GameTask task)
        {
            if (taskUIBindings.TryGetValue(task, out var ui))
                ui.UpdateProgress(task.progress, task.targetAmount);
        }
        #endregion
    }
}
