using System;

public static class GameEvents
{
    public static Action<int> OnBananasEarned;   // amount earned
    public static Action<string> OnUpgradePurchased; // upgrade id
}