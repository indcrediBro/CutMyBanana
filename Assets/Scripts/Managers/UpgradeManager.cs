using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// UpgradeManager: cost formula, discount cap, global inflation, unlocking by tier/achievement, hire limits, recompute effects.
/// Uses exponential formula: cost(level) = base * multiplier^level * globalInflation * (1 - discount)
/// globalInflation = (1 + inflationPerPurchase)^totalUpgradePurchases
/// </summary>
public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }
    public List<UpgradeSO> allUpgrades;
    private Dictionary<string, UpgradeSO> lookup= new Dictionary<string, UpgradeSO>();
    public double inflationPerPurchase = 0.005; // 0.5% per purchase
    public double discountCap = 0.35; // max effective discount

    public event Action<UpgradeSO, int> OnUpgradePurchased;

    private void Awake() { Instance = this; }

    public void Initialize()
    {
        lookup = allUpgrades.ToDictionary(u => u.id, u => u);

        var saved = SaveSystem.Load() ?? new SaveData();
        // initialize state entries; default unlocked for tier 1 items
        foreach (var u in allUpgrades)
        {
            var st = saved.upgrades.FirstOrDefault(x => x.id == u.id);
            if (st == null)
            {
                saved.upgrades.Add(new UpgradeState
                {
                    id = u.id,
                    level = 0,
                    unlocked = (u.tier == 1) || u.defaultUnlocked
                });
            }
        }
        SaveSystem.Save(saved);
        RecomputeEffects();
    }

    public IEnumerable<UpgradeSO> GetUpgradesForTier(int tier)
    {
        return allUpgrades.Where(u => u.tier == tier);
    }

    public int GetUpgradeLevel(string id)
    {
        var saved = SaveSystem.Load() ?? new SaveData();
        var st = saved.upgrades.FirstOrDefault(x => x.id == id);
        return st?.level ?? 0;
    }

    public bool IsUpgradeUnlocked(string id)
    {
        if (!lookup.ContainsKey(id)) return false;
        var u = lookup[id];
        var saved = SaveSystem.Load() ?? new SaveData();
        var st = saved.upgrades.FirstOrDefault(x => x.id == id);
        if (st != null && st.unlocked) return true;
        // unlocked if player's tier >= upgrade tier
        int curTier = saved.currentTier;
        return curTier >= u.tier;
    }

    public void UnlockUpgradesForTier(int tier)
    {
        var saved = SaveSystem.Load() ?? new SaveData();
        foreach (var s in saved.upgrades)
        {
            var u = lookup[s.id];
            if (u.tier == tier) s.unlocked = true;
        }
        SaveSystem.Save(saved);
    }

    private int PurchaseCount => (SaveSystem.Load() ?? new SaveData()).totalUpgradePurchases;

    private double GlobalInflationMultiplier => Math.Pow(1.0 + inflationPerPurchase, PurchaseCount);

    private double GetEffectiveDiscount()
    {
        double sum = 0;
        var saved = SaveSystem.Load() ?? new SaveData();
        foreach (var u in allUpgrades)
        {
            var st = saved.upgrades.FirstOrDefault(x => x.id == u.id);
            if (st != null && st.level > 0)
                sum += u.discountPercent * st.level;
        }
        return Math.Min(sum, discountCap);
    }

    public long CostForNextLevel(string id)
    {
        if (!lookup.ContainsKey(id)) return long.MaxValue;
        var u = lookup[id];
        var saved = SaveSystem.Load() ?? new SaveData();
        var st = saved.upgrades.FirstOrDefault(x => x.id == id);
        int level = st?.level ?? 0;
        double baseCost = u.baseCost * Math.Pow(u.costMultiplier, level);
        double inflated = baseCost * GlobalInflationMultiplier;
        double discount = GetEffectiveDiscount();
        double final = Math.Floor(Math.Max(1.0, inflated * (1.0 - discount)));
        return (long) final;
    }

    public bool CanPurchase(string id)
    {
        if (!lookup.ContainsKey(id)) return false;
        if (!IsUpgradeUnlocked(id)) return false;
        var saved = SaveSystem.Load() ?? new SaveData();
        var st = saved.upgrades.FirstOrDefault(x => x.id == id);
        var u = lookup[id];
        if (u.maxLevel > 0 && st.level >= u.maxLevel) return false;
        if (u.isHire && u.hireLimit > 0 && st.level >= u.hireLimit) return false;
        long cost = CostForNextLevel(id);
        return saved.currency + 1e-9 >= cost;
    }

    public bool Purchase(string id)
    {
        if (!lookup.ContainsKey(id)) return false;
        if (!CanPurchase(id)) return false;
        var saved = SaveSystem.Load() ?? new SaveData();
        var st = saved.upgrades.First(x => x.id == id);
        var u = lookup[id];
        long cost = CostForNextLevel(id);

        saved.currency -= cost;
        st.level++;
        saved.totalUpgradePurchases++;
        saved.totalUpgradesPurchasedCount++;
        // if this is a hire-type upgrade, that level indicates number of hires purchased from that upgrade
        SaveSystem.Save(saved);
        RecomputeEffects();
        PopupManager.Instance.Show("Upgrade bought", $"{u.title} purchased (Lv {st.level})");
        OnUpgradePurchased?.Invoke(u, st.level);
        GameEvents.OnUpgradePurchased?.Invoke(id);
        return true;
    }

    public void UnlockUpgradeByAchievement(string id)
    {
        var saved = SaveSystem.Load() ?? new SaveData();
        var st = saved.upgrades.FirstOrDefault(x => x.id == id);
        if (st != null)
        {
            st.unlocked = true;
            SaveSystem.Save(saved);
        }
    }

    // recompute per-slice, cps, walletCapacity
    public void RecomputeEffects()
    {
        var saved = SaveSystem.Load() ?? new SaveData();
        double perSliceAdd = 0;
        double cps = 0;
        double wallet = 0;
        foreach (var u in allUpgrades)
        {
            var st = saved.upgrades.FirstOrDefault(x => x.id == u.id);
            if (st == null) continue;
            perSliceAdd += u.addPerSlice * st.level;
            cps += u.addCps * st.level;
            wallet += u.offlineWalletIncrease * st.level;
        }
        saved.perSlice = 1.0 + perSliceAdd;
        saved.cps = cps;
        // wallet upgrades increase the wallet; base wallet stays as-is but we store walletCapacity
        if (wallet > 0) saved.walletCapacity = wallet;
        SaveSystem.Save(saved);

        var gs = FindFirstObjectByType<GameState>();
        if (gs != null) gs.UpdateFromSave(saved);
    }
}
