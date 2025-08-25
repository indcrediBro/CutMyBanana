using UnityEngine;

public enum CMBUpgradeKind
{
    KnifePerSlice,       // +per slice
    MonkeyHire,          // +monkey count (auto cps). Max 100
    MonkeyRate,          // speed monkey cps
    SpawnRate,           // spawner faster
    OfflineWallet,       // offline capacity
    OfflineMultiplier,   // offline multiplier
    Discount,            // global cost discount (capped)
    GlobalCps,           // generic cps (factories, portals)
    GlobalPerSliceMult   // multiplier to manual per-slice
}

[CreateAssetMenu(menuName = "CMB/Upgrade")]
public class CMBUpgradeSO : ScriptableObject
{
    public string id;
    public int tier;
    public string title;
    [TextArea] public string description;

    [Header("Cost")]
    public double baseCost = 10;
    public double costMultiplier = 1.15;   // tried-and-true
    public int maxLevel = 0;               // 0 = infinite

    [Header("Effects per Level")]
    public CMBUpgradeKind kind;
    public double valuePerLevel = 0;       // meaning depends on kind

    [Header("Special")]
    public bool isHireMonkey = false;
    public int hireLimit = 0;              // 0 means unlimited, else cap at 100

    public double discountPercentPerLevel = 0; // counts toward a global cap (50%)
}