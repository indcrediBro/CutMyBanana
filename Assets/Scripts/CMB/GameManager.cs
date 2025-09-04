using System;
using UnityEngine;
using Utilities;

namespace CMB
{
    public static class GameEvents
    {
        public static Action OnSlice;                       
        public static Action OnCurrencyGained;              
        public static Action<uint> OnExperienceGained;            
        public static Action OnTaskListChanged;

        public static Action<double> OnCurrencySpent;
        public static Action<string> OnUpgradePurchased;            
        public static Action OnTierAdvanced;                        
        public static Action<string> OnNarrativeUnlocked;           
        public static Action<string> OnAchievementUnlocked;         
        public static Action<double> OnOfflineEarningsApplied;      
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public PlayerData m_playerData{ get; private set; }
        public ExperienceData m_experienceData{ get; private set; }
        
        [SerializeField] private ObjectPooler m_objectPooler;
        public UIManager m_uiManager;
        public TaskManager m_taskManager;
        public PopupManager m_popupManager;
        public ShopManager m_shopManager;

        private void Awake()
        {
            InitializeSingleton();
            InitializeData();
            InitializeEssentials();
            SubscribeToEvents();
            
            m_uiManager._OnAwake();
        }

        private void OnDisable()
        {
            UnsubscribeFromEvents();
            m_uiManager._OnDestroy();
        }

        private void Start()
        {
            // Initialize all managers
        }
        
        private void InitializeSingleton()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        
        private void SubscribeToEvents()
        {
            GameEvents.OnSlice += AddSlices;
            GameEvents.OnCurrencyGained += AddMoney;
            GameEvents.OnExperienceGained += AddExperience;
            m_uiManager.SubscribeToEvents();
        }
        
        private void UnsubscribeFromEvents()
        {
            GameEvents.OnSlice -= AddSlices;
            GameEvents.OnCurrencyGained -= AddMoney;
            GameEvents.OnExperienceGained -= AddExperience;
            m_uiManager.UnsubscribeFromEvents();
        }

        private void InitializeData()
        {
            m_playerData = new PlayerData();
            m_experienceData = new ExperienceData();
        }

        private void InitializeEssentials()
        {
            m_objectPooler = new ObjectPooler();
            m_objectPooler.InitializePool();
            m_taskManager = new TaskManager();
        }
        
        
        private void AddSlices()
        {
            m_playerData.currentSliceCount += 1 * m_playerData.currentPerSliceMultiplier;
        }
        
        private void AddMoney()
        {
            m_playerData.currentMoney += m_playerData.currentBananaPrice * m_playerData.currentEarningMultiplier;
        }

        private void AddExperience(uint _exp)
        {
            m_experienceData.totalExperienceEarned += (uint)(_exp * m_experienceData.currentExperienceMultiplier);
            m_experienceData.currentExperience += _exp;
            if (m_experienceData.currentExperience >= m_experienceData.GetExperienceForNextLevel())
            {
                m_experienceData.playerLevel += 1;
                m_experienceData.currentExperience = 0;
            }
        }

    }
}
