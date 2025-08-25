using UnityEngine;

public enum CMBTaskType
{
    SliceBananas,          // count slices
    HireMonkeys,           // upgrade-level sum for monkey hires
    ReachCurrency,         // reach banana amount once
    BuyUpgrades,           // total upgrades purchased
    IncreaseSpawn,         // upgrade milestone specific
    IncreaseOfflineWallet, // wallet upgrade milestone
    IncreaseMonkeyRate     // monkey hit rate upgrade milestone
}

[CreateAssetMenu(menuName = "CMB/Task")]
public class CMBTaskSO : ScriptableObject
{
    public string id;
    public int tier;
    [TextArea] public string description;
    public CMBTaskType type;
    public int target;
    public double rewardCurrency;
    public string achievementOnComplete; // optional
}