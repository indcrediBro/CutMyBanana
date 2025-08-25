using UnityEngine;

[CreateAssetMenu(menuName = "CMB/Achievement")]
public class CMBAchievementSO : ScriptableObject
{
    public string id;
    public string title;
    [TextArea] public string description;
    public double rewardCurrency;
    public string narrativeId;           // show this narrative on unlock
    public string[] unlockUpgradeIds;    // instantly unlock these upgrades (optional)
}