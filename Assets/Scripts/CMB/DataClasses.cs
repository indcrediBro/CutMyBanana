using System;

namespace CMB
{    
    [Serializable]
    public static class GameSettings
    {
        public static uint baseExperiencePerSlice = 5;
        
        public static double baseBananaPrice = 0.5;
        public static float baseBananaSpawnRate = 5.0f;
        
        public static int sliceBaseTarget = 10;
        public static int sliceTargetIncrement = 5;
        public static int sliceBaseReward = 10;
        public static int sliceRewardIncrement = 5;

        public static int upgradeBaseTarget = 1;
        public static int upgradeTargetIncrement = 1;
        public static int upgradeBaseReward = 30;
        public static int upgradeRewardIncrement = 10;
    }
    
    [Serializable]
    public class PlayerData
    {
        public string playerName;
        
        public double currentSliceCount;
        public double currentMoney;
        
        public float currentBananaSpawnRate;
        public double currentBananaPrice;
        public float currentPerSliceMultiplier;
        public float currentEarningMultiplier;
        
        public PlayerData()
        {
            int r = UnityEngine.Random.Range(2, 10001);
            playerName = $"BananaGuy_{r}";
            
            currentSliceCount = 0;
            currentMoney = 0.0;
            currentPerSliceMultiplier = 1.0f;
            currentEarningMultiplier = 1.0f;
            currentBananaSpawnRate = GameSettings.baseBananaSpawnRate;
            currentBananaPrice = GameSettings.baseBananaPrice;
        }
    }
    
    [Serializable]
    public class ExperienceData
    {
        public int playerLevel;
        public uint currentExperience;
        public float currentExperienceMultiplier;
        public uint currentExperiencePerSlice;
        public uint totalExperienceEarned;
        
        public uint GetExperienceForNextLevel()
        {
            return 10 + (uint)((playerLevel - 1) * 15);
        }
        
        public ExperienceData()
        {
            playerLevel = 1;
            currentExperience = 0;
            currentExperienceMultiplier = 1.0f;
            totalExperienceEarned = 0;
            currentExperiencePerSlice = GameSettings.baseExperiencePerSlice;
        }
    }

}