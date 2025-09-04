using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace CMB
{
    public class ShopItemUI : MonoBehaviour
    {
        public Transform shopContainer;
        public GameObject shopItemPrefab;
        [SerializeField] private TMP_Text text;
        [SerializeField] private Button button;
        
        private void Start()
        {
            foreach (var upgrade in GameManager.Instance.m_shopManager.availableUpgrades)
            {
                GameObject go = Instantiate(shopItemPrefab, shopContainer);

                UpdateButtonText(text, upgrade);

                button.onClick.AddListener(() =>
                {
                    if (GameManager.Instance.m_shopManager.TryPurchase(upgrade.id))
                    {
                        UpdateButtonText(text, upgrade);
                    }
                });
            }
        }

        private void UpdateButtonText(TMP_Text _text, Upgrade upgrade)
        {
            _text.text = $"{upgrade.displayName} (Lv {upgrade.level}) - ${upgrade.GetCost():F2}";
        }
        
        public void SellSlices(int amount = -1)
        {
            PlayerData player = GameManager.Instance.m_playerData;

            // if amount = -1 → sell ALL
            int sellAmount = amount < 0 ? (int)player.currentSliceCount : Mathf.Min(amount, (int)player.currentSliceCount);
            if (sellAmount <= 0) return;

            // remove slices
            player.currentSliceCount -= sellAmount;

            // add money
            double earned = sellAmount * player.currentBananaPrice * player.currentEarningMultiplier;
            player.currentMoney += earned;

            GameEvents.OnCurrencyGained?.Invoke();
        }
    }
}