using System;
using UnityEngine;

/// <summary>
/// OfflineManager: records last inactive time and applies offline earnings on resume.
/// Monkeys and cps count toward saved.cps; walletCapacity bounds final award.
/// </summary>
public class OfflineManager : MonoBehaviour
{
    public double maxOfflineHours = 8.0;

    public event Action<double, TimeSpan> OnOfflineAwarded;

    public void RecordInactiveTime()
    {
        var s = SaveSystem.Load() ?? new SaveData();
        s.lastInactiveBinary = DateTime.UtcNow.ToBinary().ToString();
        SaveSystem.Save(s);
    }

    public void ApplyOfflineIfAny()
    {
        var s = SaveSystem.Load();
        if (s == null) return;
        if (string.IsNullOrEmpty(s.lastInactiveBinary) || s.lastInactiveBinary == "0") return;
        try
        {
            long bin = Convert.ToInt64(s.lastInactiveBinary);
            DateTime last = DateTime.FromBinary(bin);
            DateTime now = DateTime.UtcNow;
            if (now <= last) return;
            TimeSpan diff = now - last;
            double seconds = Math.Min(diff.TotalSeconds, maxOfflineHours * 3600.0);
            double production = s.cps;
            double raw = production * seconds;
            double awarded = Math.Floor(Math.Min(raw, s.walletCapacity));
            if (awarded > 0)
            {
                s.currency += awarded;
                SaveSystem.Save(s);
                PopupManager.Instance.Show("Welcome back", $"You earned {awarded} bananas while away ({FormatTimeSpan(diff)})");
                OnOfflineAwarded?.Invoke(awarded, TimeSpan.FromSeconds(seconds));
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[OfflineManager] Apply error: " + ex.Message);
        }
    }

    private string FormatTimeSpan(TimeSpan t)
    {
        if (t.TotalHours >= 1) return $"{(int)t.TotalHours}h {(int)t.Minutes}m";
        if (t.TotalMinutes >= 1) return $"{(int)t.TotalMinutes}m";
        return $"{(int)t.Seconds}s";
    }
    
    private void OnApplicationPause(bool pause)
    {
        if (pause) RecordQuitTime();
    }

    private void OnApplicationQuit()
    {
        RecordQuitTime();
    }

    private void RecordQuitTime()
    {
        var s = SaveSystem.Load() ?? new SaveData();
        s.lastQuitTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        SaveSystem.Save(s);
    }

}