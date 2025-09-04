using System;
using System.Collections.Generic;
using UnityEngine;

namespace CMB
{
    [Serializable]
    public class Upgrade
    {
        public string id;
        public string displayName;
        public double baseCost;
        public int level;
        public float multiplier;

        public Upgrade(string id, string displayName, double baseCost, float multiplier)
        {
            this.id = id;
            this.displayName = displayName;
            this.baseCost = baseCost;
            this.multiplier = multiplier;
            this.level = 0;
        }

        public double GetCost()
        {
            // Exponential scaling
            return baseCost * Math.Pow(1.5f, level);
        }
    }

    [Serializable]
    public class ShopManager
    {
        public List<Upgrade> availableUpgrades;

        public ShopManager()
        {
            availableUpgrades = new List<Upgrade>()
            {
                new Upgrade("slice_mult", "Stronger Slices", 10, 0.2f),
                new Upgrade("earn_mult", "Banana Salesman", 20, 0.25f),
                new Upgrade("spawn_rate", "Banana Rain", 50, 0.1f)
            };
        }

        public bool TryPurchase(string upgradeId)
        {
            PlayerData player = GameManager.Instance.m_playerData;
            Upgrade upgrade = availableUpgrades.Find(u => u.id == upgradeId);

            if (upgrade == null) return false;

            double cost = upgrade.GetCost();
            if (player.currentMoney < cost)
            {
                Debug.Log("Not enough money!");
                return false;
            }

            // Deduct money
            player.currentMoney -= cost;
            GameEvents.OnCurrencySpent?.Invoke(cost);

            // Apply upgrade effect
            ApplyUpgrade(player, upgrade);

            upgrade.level++;
            GameEvents.OnUpgradePurchased?.Invoke(upgradeId);
            return true;
        }

        private void ApplyUpgrade(PlayerData player, Upgrade upgrade)
        {
            switch (upgrade.id)
            {
                case "slice_mult":
                    player.currentPerSliceMultiplier += upgrade.multiplier;
                    break;
                case "earn_mult":
                    player.currentEarningMultiplier += upgrade.multiplier;
                    break;
                case "spawn_rate":
                    player.currentBananaSpawnRate = 
                        Mathf.Max(0.5f, player.currentBananaSpawnRate - upgrade.multiplier);
                    break;
            }
        }
    }
}
