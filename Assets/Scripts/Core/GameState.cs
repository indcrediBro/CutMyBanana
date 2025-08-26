using System;
using UnityEngine;
/// <summary>
/// GameState: runtime wrapper around SaveData to give methods for other systems
/// </summary>

public class GameState : MonoBehaviour
{
    public SaveData data;

    public event Action OnStateChanged;

    private void Awake()
    {
        data = SaveSystem.Load() ?? CreateEmptySave();
        // ensure some sensible defaults
        if (data.walletCapacity <= 0) data.walletCapacity = 100; // small starting wallet
        if (data.perSlice <= 0) data.perSlice = 1.0;
        SaveSystem.Save(data);
    }

    private SaveData CreateEmptySave()
    {
        var s = new SaveData();
        s.currentTier = 1;
        s.walletCapacity = 100;
        s.perSlice = 1.0;
        return s;
    }

    public void AddCurrency(int amt)
    {
        if (amt <= 0) return;
        data.currency += amt;
        data.totalEarned += amt;
        GameEvents.OnBananasEarned?.Invoke(amt);
        SaveSystem.Save(data);
        OnStateChanged?.Invoke();
    }

    public bool SpendCurrency(double amt)
    {
        if (data.currency + 1e-9 < amt) return false;
        data.currency -= amt;
        SaveSystem.Save(data);
        OnStateChanged?.Invoke();
        return true;
    }

    public void AddSlice(double count = 1)
    {
        data.totalSlices += count;
        SaveSystem.Save(data);
        OnStateChanged?.Invoke();
    }

    public void UpdateFromSave(SaveData s)
    {
        data = s;
        OnStateChanged?.Invoke();
    }

    public SaveData GetSave() => data;
}
