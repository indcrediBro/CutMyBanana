using UnityEngine;

[CreateAssetMenu(menuName = "CMB/Task")]
public class TaskSO : ScriptableObject
{
    public string id;
    public int tier = 1;
    public string title;
    [TextArea] public string description;
    public int target = 1;
    public int rewardCurrency = 0;
    public string unlocksAchievementId;
    public bool repeatable = false;
}

[CreateAssetMenu(menuName = "CMB/Achievement")]
public class AchievementSO : ScriptableObject
{
    public string id;
    public string title;
    [TextArea] public string description;
    public string unlocksNarrativeId;
    public string[] unlocksUpgradeIds;
    public int rewardCurrency;
}

[CreateAssetMenu(menuName = "CMB/Upgrade")]
public class UpgradeSO : ScriptableObject
{
    public string id;
    public int tier = 1;
    public string title;
    [TextArea] public string description;

    public long baseCost = 100;
    public float costMultiplier = 1.18f;
    public int maxLevel = 0; // 0 = infinite

    // Effects
    public double addPerSlice = 0;           // manual slice value add
    public double addCps = 0;                // bananas-per-second passive
    public float spawnRateMultiplier = 1f;   // affect spawner speed
    public double offlineWalletIncrease = 0; // add wallet capacity
    public double discountPercent = 0;       // discount contribution (clamped globally)
    public bool isHire = false;
    public int hireLimit = 0;                // when isHire true, max hires allowed for this upgrade

    // Required to advance tier (if > 0)
    public int requiredLevelForTierComplete = 0;

    // initially available for tier 1 items only; other tiers unlocked when player reaches tier
    public bool defaultUnlocked = false;
}

[CreateAssetMenu(menuName = "CMB/Narrative")]
public class NarrativeSO : ScriptableObject
{
    public string id;
    public string title;
    [TextArea(5,12)] public string body;
}